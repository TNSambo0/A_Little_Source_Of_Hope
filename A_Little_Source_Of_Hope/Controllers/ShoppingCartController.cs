using A_Little_Source_Of_Hope.Models;
using A_Little_Source_Of_Hope.Areas.Identity.Data;
using A_Little_Source_Of_Hope.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace A_Little_Source_Of_Hope.Controllers
{
    [Authorize(Policy = "Customers")]
    public class ShoppingCartController : Controller
    {
        private readonly ILogger<ShoppingCartController> _logger;
        private readonly AppDbContext _AppDb;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        protected IAuthorizationService _AuthorizationService;

        public ShoppingCartController(ILogger<ShoppingCartController> logger, AppDbContext AppDb, UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, IAuthorizationService AuthorizationService)
        {
            _logger = logger;
            _AppDb = AppDb;
            _userManager = userManager;
            _signInManager = signInManager;
            _AuthorizationService = AuthorizationService;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var sessionHandler = new SessionHandler();
                await sessionHandler.GetSession(HttpContext, _signInManager, _logger);
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    await sessionHandler.SignUserOut(_signInManager, _logger);
                    return RedirectToPage("Login");
                }

                IEnumerable<ShoppingCart> UserCart = from cart in _AppDb.ShoppingCart
                                                     join product in _AppDb.Product
                                                     on cart.ProductId equals product.Id
                                                     where cart.AppUserId == user.Id
                                                     select new ShoppingCart
                                                     {
                                                         Id = cart.Id,
                                                         ProductId = cart.ProductId,
                                                         Quantity = cart.Quantity,
                                                         AvailableQuantity = product.Quantity,
                                                         ImageUrl = product.Imageurl,
                                                         ProductName = product.ProductName,
                                                         Description = product.Description,
                                                         PricePerItem = product.ClaimStatus == true ? 0 : product.Price,
                                                         TotalPerItem = Math.Round(((decimal)(cart.Quantity == null ? 0 : cart.Quantity * (product.ClaimStatus ? 0 : product.Price))), 2),
                                                         AppUserId = cart.AppUserId,
                                                     };
                var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, UserCart, Operations.Read);
                if (!isAuthorized.Succeeded)
                {
                    TempData["error"] = "You don't have the permission to see items on cart.";
                    return Forbid();
                }
                if (!UserCart.Any())
                {
                    return View();
                }
                GetTotal(UserCart);
                ViewData["Cart"] = "Not Null";
                return View(UserCart);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View();
            }
        }
        [HttpPost]
        public async Task<JsonResult> AddToCart(int productID)
        {
            var sessionHandler = new SessionHandler();
            await sessionHandler.GetSession(HttpContext, _signInManager, _logger);
            var user = await _userManager.GetUserAsync(User);
            ItemRemoveStatusModel results = new();
            if (user == null)
            {
                await sessionHandler.SignUserOut(_signInManager, _logger);
                results.Status = "Login";
                results.Message = "User autometically logout due to session end";
                return Json(JsonConvert.SerializeObject(results));
            }

            var ProductFromDb = await _AppDb.Product.FirstOrDefaultAsync(p => p.Id == productID && p.Quantity >= 1 && p.IsActive == true);
            if (ProductFromDb == null)
            {
                results.Status = "error";
                results.Message = "Selected item not found.";
                return Json(JsonConvert.SerializeObject(results));
            }
            var cart = new ShoppingCart
            {
                ProductId = ProductFromDb.Id,
                ProductName = ProductFromDb.ProductName,
                Description = ProductFromDb.Description,
                PricePerItem = ProductFromDb.Price,
                Quantity = ProductFromDb.Quantity,
                ImageUrl = ProductFromDb.Imageurl,
                AppUserId = user.Id
            };
            var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, cart, Operations.Add);
            if (!isAuthorized.Succeeded)
            {
                results.Status = "error";
                results.Message = "You don't have the permission to add items to cart.";
                return Json(JsonConvert.SerializeObject(results));
            }
            if (ProductFromDb.Quantity < 1)
            {
                results.Status = "error";
                results.Message = "Selected quantity is greater then the available quantity.";
                return Json(JsonConvert.SerializeObject(results));
            }
            var addCart = new ShoppingCart();
            var userCartFromDb = await _AppDb.ShoppingCart.FirstOrDefaultAsync(c => c.AppUserId == user.Id && c.ProductId == productID);
            if (userCartFromDb == null)
            {
                addCart.ProductId = productID;
                addCart.Quantity = 1;
                addCart.AppUserId = user.Id;
                await _AppDb.ShoppingCart.AddAsync(addCart);
                await _AppDb.SaveChangesAsync();
                results.Status = "success";
                results.Message = "Item successfully added to cart.";
                return Json(JsonConvert.SerializeObject(results));
            }
            else
            {
                userCartFromDb.Quantity += 1;
                _AppDb.ShoppingCart.Update(userCartFromDb);
                await _AppDb.SaveChangesAsync();
                results.Status = "success";
                results.Message = "Item successfully added to cart.";
                return Json(JsonConvert.SerializeObject(results));
            }
        }
        [HttpPost]
        public async Task<JsonResult> RemoveFromCart(int productId)
        {
            var sessionHandler = new SessionHandler();
            await sessionHandler.GetSession(HttpContext, _signInManager, _logger);
            var user = await _userManager.GetUserAsync(User);
            ItemRemoveStatusModel results = new();
            if (user == null)
            {
                await sessionHandler.SignUserOut(_signInManager, _logger);
                results.Status = "Login";
                results.Message = "User autometically logout due to session end";
                return Json(JsonConvert.SerializeObject(results));
            }
            var cartFromDb = await _AppDb.ShoppingCart.FirstOrDefaultAsync(x => x.AppUserId == user.Id && x.ProductId == productId);
            if (cartFromDb == null)
            {
                results.Status = "error";
                results.Message = "Selected item not found.";
                return Json(JsonConvert.SerializeObject(results));
            }
            var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, cartFromDb, Operations.Delete);
            if (!isAuthorized.Succeeded)
            {
                results.Status = "error";
                results.Message = "You don't have the permission to delete items on cart.";
                return Json(JsonConvert.SerializeObject(results));
            }
            _AppDb.ShoppingCart.Remove(cartFromDb);
            await _AppDb.SaveChangesAsync();
            var UpdatedCartFromDb = await _AppDb.ShoppingCart.AnyAsync(x => x.AppUserId == user.Id);
            results.Status = "success";
            results.Message = "Item successfully removed from cart.";
            return Json(JsonConvert.SerializeObject(results));
        }
        [HttpPost]
        public async Task<JsonResult> ClearCart()
        {
            var sessionHandler = new SessionHandler();
            await sessionHandler.GetSession(HttpContext, _signInManager, _logger);
            var user = await _userManager.GetUserAsync(User);
            ItemRemoveStatusModel results = new();
            if (user == null)
            {
                await sessionHandler.SignUserOut(_signInManager, _logger);
                results.Status = "Login";
                results.Message = "User autometically logout due to session end";
                return Json(JsonConvert.SerializeObject(results));
            }
            var UserCart = from aCart in _AppDb.ShoppingCart where aCart.AppUserId == user.Id select aCart;
            var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, UserCart, Operations.Delete);
            if (!isAuthorized.Succeeded)
            {
                results.Status = "error";
                results.Message = "You don't have the permission to delete items on cart.";
                return Json(JsonConvert.SerializeObject(results));
            }
            foreach (var CartItem in UserCart)
            {
                _AppDb.ShoppingCart.Remove(CartItem);
            }
            await _AppDb.SaveChangesAsync();
            results.Status = "success";
            results.Message = "Cart successfully cleared.";
            return Json(JsonConvert.SerializeObject(results));
        }
        public void GetTotal(IEnumerable<ShoppingCart> userCart)
        {
            var amount = userCart.Sum(item => item.TotalPerItem);
            var amountInDecimal = (decimal)(amount == null ? 0 : amount);
            ViewData["SubTotal"] = Math.Round(amountInDecimal, 2);
            ViewData["VatOfSubTotal"] = Math.Round((amountInDecimal * (decimal)(0.14)), 2);
            ViewData["Total"] = Math.Round((amountInDecimal * (decimal)0.14) + amountInDecimal, 2);
        }
        [HttpPost]
        public async Task<JsonResult> ChangeQuantity(int Id, int quantity)
        {
            ItemRemoveStatusModel results = new();
            var sessionHandler = new SessionHandler();
            await sessionHandler.GetSession(HttpContext, _signInManager, _logger);
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                await sessionHandler.SignUserOut(_signInManager, _logger);
                results.Status = "Login";
                results.Message = "User autometically logout due to session end";
                return Json(JsonConvert.SerializeObject(results));
            }
            var product = await _AppDb.ShoppingCart.FirstOrDefaultAsync(x => x.ProductId == Id && x.AppUserId == user.Id);
            if (product == null)
            {
                results.Message = "Product not found";
                results.Status = "error";
                return Json(JsonConvert.SerializeObject(results));
            }
            var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, product, Operations.Update);
            if (!isAuthorized.Succeeded)
            {
                results.Status = "error";
                results.Message = "You don't have the permission to edit items on cart.";
                return Json(JsonConvert.SerializeObject(results));
            }
            product.Quantity = quantity;
            _AppDb.Update(product);
            await _AppDb.SaveChangesAsync();
            results.Message = "Product quantity have been changed successfully.";
            results.Status = "success";
            return Json(JsonConvert.SerializeObject(results));
        }
    }
}
