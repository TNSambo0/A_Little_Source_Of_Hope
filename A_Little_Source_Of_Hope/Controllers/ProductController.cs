using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using A_Little_Source_Of_Hope.Data;
using A_Little_Source_Of_Hope.Models;
using Microsoft.AspNetCore.Identity;
using A_Little_Source_Of_Hope.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
//using System.Web.Mvc;

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

        public ProductController(ILogger<ShoppingCartController> logger, AppDbContext AppDb, UserManager<AppUser> userManager,
            IAuthorizationService AuthorizationService, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _AppDb = AppDb;
            _AuthorizationService = AuthorizationService;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        // GET: ProductController
        public async Task<ActionResult> Index()
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
        public async Task<ActionResult> Create()
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
        public async Task<ActionResult> Create(Product product)
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
                            var filename = Path.GetFileName(product.File.FileName);
                            var fileExt = Path.GetExtension(product.File.FileName);
                            string fileNameWithoutPath = Path.GetFileNameWithoutExtension(product.File.FileName);
                            string myfile = fileNameWithoutPath + "_" + product.ProductName + fileExt;
                            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Product");
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            string fileNameWithPath = Path.Combine(path, myfile);
                            product.Imageurl = $"images/Product/{myfile}";
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
                        //using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                        //{
                        //    product.File.CopyTo(stream);
                        //}
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
        public async Task<ActionResult> Edit(int? id)
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
        public async Task<ActionResult> Edit(Product product)
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
                        var filename = Path.GetFileName(product.File.FileName);
                        var fileExt = Path.GetExtension(product.File.FileName);
                        string fileNameWithoutPath = Path.GetFileNameWithoutExtension(product.File.FileName);
                        string myfile = fileNameWithoutPath + "_" + product.ProductName + fileExt;
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Product");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        string fileNameWithPath = Path.Combine(path, myfile);
                        product.Imageurl = $"images/Product/{myfile}";
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
            if (await _AppDb.Product.AnyAsync()) { ViewData["Product"] = "not null"; }
            else { ViewData["Product"] = null; }
            results.Message = "Product have been deleted successfully.";
            results.Status = "success";
            results.DeleteItemsIds = ProductIds;
            return Json(JsonConvert.SerializeObject(results));
        }
    }
}

