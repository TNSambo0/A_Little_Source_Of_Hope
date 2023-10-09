using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using A_Little_Source_Of_Hope.Data;
using A_Little_Source_Of_Hope.Models;
using Microsoft.AspNetCore.Identity;
using A_Little_Source_Of_Hope.Services.Abstract;
using A_Little_Source_Of_Hope.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace A_Little_Source_Of_Hope.Controllers
{
    [Authorize(Policy = "RequireAdministratorRole")]
    public class ProductController : Controller
    {
        private readonly ILogger<ShoppingCartController> _logger;
        private readonly AppDbContext _AppDb;
        protected IAuthorizationService _AuthorizationService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IImageService _imageService;

        public ProductController(ILogger<ShoppingCartController> logger, AppDbContext AppDb, UserManager<AppUser> userManager,
            IAuthorizationService AuthorizationService, IImageService imageService, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _AppDb = AppDb;
            _AuthorizationService = AuthorizationService;
            _userManager = userManager;
            _signInManager = signInManager;
            _imageService = imageService;
        }
        // GET: ProductController
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
                IEnumerable<Product> objProduct = _AppDb.Product;
                if (_AppDb.Product.Any()) { ViewData["Product"] = "not null"; }
                else { ViewData["Product"] = null; }
                var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, objProduct, Operations.Read);
                if (!isAuthorized.Succeeded)
                {
                    TempData["error"] = "You don't have the permission to see Products.";
                    return Forbid();
                }
                return View(objProduct);
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

        // GET: ProductController/Create
        public async Task<IActionResult> Create()
        {
            var sessionHandler = new SessionHandler();
            await sessionHandler.GetSession(HttpContext, _signInManager, _logger);
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                await sessionHandler.SignUserOut(_signInManager, _logger);
                return RedirectToPage("Login");
            }
            var isAuthorized = User.IsInRole(Constants.ProductAdministratorsRole);
            if (!isAuthorized)
            {
                TempData["error"] = "You don't have the permission to create a product.";
                return RedirectToAction("Index", "Admin");
            }
            var product = new Product
            {
                CategoryNames = _AppDb.Category.Select(x => new SelectListItem() { Text = x.CategoryName, Value = x.Id.ToString() }).AsEnumerable(),
                CategoryId = 1
            };
            return View(product);
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
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
                product.CategoryNames = _AppDb.Category.Select(x => new SelectListItem() { Text = x.CategoryName, Value = x.Id.ToString() }).AsEnumerable();
                if (ModelState.IsValid)
                {
                    var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, product, Operations.Create);
                    if (!isAuthorized.Succeeded)
                    {
                        TempData["error"] = "You don't have the permission to create a product.";
                        return RedirectToAction("Index");
                    }
                    var productFromDb = _AppDb.Product.Contains(product);
                    if (productFromDb != true)
                    {
                        if (product.File != null && product.File.FileName != null)
                        {
                            product.Imageurl = _imageService.uploadImageToAzure(product.File);
                        }
                        product.CreatedDate = DateTime.Now;
                        product.CategoryNames = _AppDb.Category.Select(x => new SelectListItem() { Text = x.CategoryName, Value = x.Id.ToString() }).AsEnumerable();
                        var selectedProduct = product.CategoryNames.FirstOrDefault(x => x.Value == product.CategoryId.ToString());
                        if (selectedProduct == null)
                        {
                            return View(product);
                        }
                        if (product.CategoryId.ToString() == null)
                        {
                            product.CategoryId = int.Parse(selectedProduct.Value);
                        }
                        else
                        {
                            if (selectedProduct != null && selectedProduct.Value != product.CategoryId.ToString())
                            {
                                product.CategoryId = int.Parse(selectedProduct.Value);
                            }
                        }
                        await _AppDb.Product.AddAsync(product);
                        await _AppDb.SaveChangesAsync();
                        TempData["success"] = "Product successfully created";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Product already exists.");
                        TempData["error"] = "Product already exists";
                        return View(product);
                    }
                }
                return View(product);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View(product);
            }
        }

        // GET: ProductController/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
                if (id is null or 0)
                {
                    return NotFound();
                }
                var productFromDb = await _AppDb.Product.FindAsync(id);
                if (productFromDb == null)
                {
                    return NotFound();
                }
                var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, productFromDb, Operations.Update);
                if (!isAuthorized.Succeeded)
                {
                    TempData["error"] = "You don't have the permission to edit a product.";
                    return RedirectToAction("Index");
                }
                productFromDb.CategoryNames = _AppDb.Category.Select(x => new SelectListItem() { Text = x.CategoryName, Value = x.Id.ToString() }).AsEnumerable();
                return View(productFromDb);
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

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            try
            {
                var sessionHandler = new SessionHandler();
                await sessionHandler.GetSession(HttpContext, _signInManager, _logger);
                product.CategoryNames = _AppDb.Category.Select(x => new SelectListItem() { Text = x.CategoryName, Value = x.Id.ToString() }).AsEnumerable();
                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        await sessionHandler.SignUserOut(_signInManager, _logger);
                        return RedirectToPage("Login");
                    }
                    var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, product, Operations.Update);
                    if (!isAuthorized.Succeeded)
                    {
                        TempData["error"] = "You don't have the permission to edit a product.";
                        return RedirectToAction("Index");
                    }
                    var productFromDb = await _AppDb.Product.ContainsAsync(product);
                    if (!productFromDb)
                    {
                        return NotFound();
                    }
                    var selectedCategory = product.CategoryNames.FirstOrDefault(x => x.Value == product.Id.ToString());
                    if (selectedCategory == null)
                    {
                        return View(product);
                    }
                    if (product.CategoryId.ToString() == null)
                    {
                        product.CategoryId = int.Parse(selectedCategory.Value);
                    }
                    else
                    {
                        if (selectedCategory != null && selectedCategory.Value != product.CategoryId.ToString())
                        {
                            product.CategoryId = int.Parse(selectedCategory.Value);
                        }
                    }
                    if (product.File != null && product.File.FileName != null)
                    {
                        product.Imageurl = _imageService.uploadImageToAzure(product.File);
                    }
                    _AppDb.Product.Update(product);
                    await _AppDb.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                return View(product);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View(product);
            }
        }
        [HttpPost]
        public async Task<JsonResult> Delete(List<int> ProductIds)
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
            if (ProductIds.Count == 0)
            {
                results.Message = "An error occured while deleting selected item, please try again.";
                results.Status = "error";
                results.DeleteItemsIds = ProductIds;
                return Json(JsonConvert.SerializeObject(results));
            }
            Product Product = await _AppDb.Product.FindAsync(ProductIds[0]);
            var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, Product, Operations.Delete);
            if (!isAuthorized.Succeeded)
            {
                results.Message = "You don't have the permission to Delete a product.";
                results.Status = "error";
                results.DeleteItemsIds = ProductIds;
                return Json(JsonConvert.SerializeObject(results));
            }
            string errorList = "";
            foreach (var ProductId in ProductIds)
            {
                var productFromDb = await _AppDb.Product.FindAsync(ProductId);
                if (productFromDb == null)
                {
                    errorList += ProductId.ToString() + " ";
                }
                else
                {
                    _AppDb.Product.Remove(productFromDb);
                    _imageService.deleteImageFromAzure(productFromDb.Imageurl);
                }
            }
            if (!String.IsNullOrEmpty(errorList))
            {
                results.Message = $"An error ocuured while trying to remove product with productId {errorList}.";
                results.Status = "error";
                results.DeleteItemsIds = ProductIds;
                return Json(JsonConvert.SerializeObject(results));
            }
            await _AppDb.SaveChangesAsync();
            results.Message = "Product have been deleted successfully.";
            results.Status = "success";
            results.DeleteItemsIds = ProductIds;
            return Json(JsonConvert.SerializeObject(results));
        }
    }
}

