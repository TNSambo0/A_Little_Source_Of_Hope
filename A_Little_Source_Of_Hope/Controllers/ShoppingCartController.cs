using A_Little_Source_Of_Hope.Models;
using A_Little_Source_Of_Hope.Areas.Identity.Data;
using A_Little_Source_Of_Hope.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace A_Little_Source_Of_Hope.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly ILogger<ShoppingCartController> _logger;
        private readonly AppDbContext _Userdb;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public ShoppingCartController(ILogger<ShoppingCartController> logger, AppDbContext Userdb, UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _Userdb = Userdb;
            _userManager = userManager;
            _signInManager = signInManager;
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
                    return Problem("Please try login in again.");
                }

                IEnumerable<ShoppingCart> UserCart = from cart in _Userdb.ShoppingCart
                                              join product in _Userdb.Product
                                              on cart.ProductId equals product.ProductId
                                              where cart.AppUserId == user.Id
                                              select new ShoppingCart
                                              {
                                                  ShoppingCartId = cart.ShoppingCartId,
                                                  ProductId = cart.ProductId,
                                                  Quantity = cart.Quantity,
                                                  AvailableQuantity = product.Quantity,
                                                  ImageUrl = product.Imageurl,
                                                  ProductName = product.ProductName,
                                                  Description = product.Description,
                                                  PricePerItem = product.ClaimStatus == true ? 0 : product.Price,
                                                  TotalPerItem = Math.Round(((decimal)(cart.Quantity == null ? 0 : cart.Quantity * (product.ClaimStatus ? 0 : product.Price))),2),
                                                  AppUserId = cart.AppUserId,
                                              };
                if (!UserCart.Any())
                {
                    return View();
                }
                GetTotal(UserCart);
                ViewBag.Cart = "Not Null";
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

        public async Task<IActionResult> AddToCart(int productID, bool IsCartUrl, int quantity = 1)
        {
            try
            {
                var sessionHandler = new SessionHandler();
                //var hf = HttpContext.User.Identity.IsAuthenticated;
                await sessionHandler.GetSession(HttpContext, _signInManager, _logger);
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    await sessionHandler.SignUserOut(_signInManager, _logger);
                    return Problem("Please try login in again.");
                }

                var ProductFromDb = await _Userdb.Product.FirstOrDefaultAsync(p => p.ProductId == productID && p.Quantity >= 1 && p.IsActive == true);
                if (ProductFromDb == null)
                {
                    TempData["error"] = "Selected item not found";
                    return RedirectToAction("MarketPlace", "Home");
                }
                if (ProductFromDb.Quantity < quantity)
                {
                    TempData["error"] = "Selected quantity is greater then the available items";
                    return RedirectToAction("Index");
                }
                var addCart = new ShoppingCart();
                var userCartFromDb = await _Userdb.ShoppingCart.FirstOrDefaultAsync(c => c.AppUserId == user.Id && c.ProductId == productID);
                if (userCartFromDb == null)
                {
                    addCart.ProductId = productID;
                    addCart.Quantity = quantity;
                    addCart.AppUserId = user.Id;
                    await _Userdb.ShoppingCart.AddAsync(addCart);
                    await _Userdb.SaveChangesAsync();
                    return RedirectToAction("MarketPlace", "Home");
                }
                else
                {
                    userCartFromDb.Quantity += quantity;
                    _Userdb.ShoppingCart.Update(userCartFromDb);
                    await _Userdb.SaveChangesAsync();
                    if (IsCartUrl) { return RedirectToAction("Index"); }
                    return RedirectToAction("MarketPlace", "Home");
                }
            }
            catch (Exception ex)
            {
                ViewData["error"] = ex.ToString();
                TempData["error"] = "An error occured, please try agin";
                if (IsCartUrl) { return RedirectToAction("Index"); }
                return RedirectToAction("MarketPlace", "Home");
            }
        }
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            try
            {
                var sessionHandler = new SessionHandler();
                await sessionHandler.GetSession(HttpContext, _signInManager, _logger);
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    await sessionHandler.SignUserOut(_signInManager, _logger);
                    return Problem("Please try login in again.");
                }

                var cartFromDb = await _Userdb.ShoppingCart.FirstOrDefaultAsync(x => x.AppUserId == user.Id && x.ProductId == productId);
                if (cartFromDb == null)
                {
                    return RedirectToAction("Index");
                }
                _Userdb.ShoppingCart.Remove(cartFromDb);
                await _Userdb.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return RedirectToAction("Index");
            }
        }
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var sessionHandler = new SessionHandler();
                await sessionHandler.GetSession(HttpContext, _signInManager, _logger);
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    await sessionHandler.SignUserOut(_signInManager, _logger);
                    return Problem("Please try login in again.");
                }

                var UserCart = _Userdb.ShoppingCart.Where(cart => cart.AppUserId == user.Id);
                foreach (var CartItem in UserCart)
                {
                    _Userdb.ShoppingCart.Remove(CartItem);
                }
                await _Userdb.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return RedirectToAction("Index");
            }
        }
        public void GetTotal(IEnumerable<ShoppingCart> userCart)
        {
            var amount = userCart.Sum(item => item.TotalPerItem);
            var amountInDecimal = (decimal)(amount == null ? 0 : amount);
            ViewData["SubTotal"] = Math.Round(amountInDecimal, 2);
            ViewData["VatOfSubTotal"] = Math.Round((amountInDecimal*(decimal)(0.14)), 2); 
            ViewData["Total"] = Math.Round((amountInDecimal*(decimal)0.14)+amountInDecimal, 2);
        }
    }
}
