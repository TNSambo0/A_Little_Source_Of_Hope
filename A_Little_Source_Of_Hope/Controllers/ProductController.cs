using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using A_Little_Source_Of_Hope.Data;
using A_Little_Source_Of_Hope.Models;
using Microsoft.AspNetCore.Identity;
using A_Little_Source_Of_Hope.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace A_Little_Source_Of_Hope.Controllers
{
    [Authorize(Roles = "ProductAdministrators")]
    public class ProductController : Controller
    {
        private readonly ILogger<ShoppingCartController> _logger;
        private readonly AppDbContext _userDb;
        protected IAuthorizationService _AuthorizationService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public ProductController(ILogger<ShoppingCartController> logger, AppDbContext userDb, UserManager<AppUser> userManager,
            IAuthorizationService AuthorizationService, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _userDb = userDb;
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
                    return Problem("Please try login in again.");
                }
                var isAuthorized = User.IsInRole(Constants.ProductAdministratorsRole);
                if (!isAuthorized)
                {
                    TempData["error"] = "You don't have the permission to see Product.";
                    return RedirectToAction("Index", "Admin");
                }
                IEnumerable<Product> objProduct = _userDb.Product;
                if (_userDb.Product.Any()) { ViewBag.Product = true; }
                else { ViewBag.Product = false; }
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
                return Problem("Please try login in again.");
            }
            Product product = new();
            var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, product, ProductOperations.Create);
            if (!isAuthorized.Succeeded)
            {
                TempData["error"] = "You don't have the permission to create a product.";
                return RedirectToAction("Index");
            }
            return View();
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
                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        await sessionHandler.SignUserOut(_signInManager, _logger);
                        return Problem("Please try login in again.");
                    }
                    var productFromDb = _userDb.Product.Contains(product);
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
                            product.Imageurl = fileNameWithPath;
                        }
                        product.CreatedDate = DateTime.Now;
                        product.IsActive = Producttatus.IsProductActive(product);
                        await _userDb.Product.AddAsync(product);
                        await _userDb.SaveChangesAsync();
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
                    return Problem("Please try login in again.");
                }
                if (id is null or 0)
                {
                    return NotFound();
                }
                var productFromDb = await _userDb.Product.FindAsync(id);
                if (productFromDb == null)
                {
                    return NotFound();
                }
                var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, productFromDb, ProductOperations.Update);
                if (!isAuthorized.Succeeded)
                {
                    TempData["error"] = "You don't have the permission to edit a product.";
                    return RedirectToAction("Index");
                }
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

                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        await sessionHandler.SignUserOut(_signInManager, _logger);
                        return Problem("Please try login in again.");
                    }
                    var productFromDb = await _userDb.Product.FindAsync(product.ProductId);
                    if (productFromDb == null)
                    {
                        return NotFound();
                    }
                    productFromDb.IsActive = Producttatus.IsProductActive(productFromDb);
                    //_userDb.Attach(productFromDb).State = EntityState.Modified;
                    _userDb.Product.Update(productFromDb);
                    await _userDb.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
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

        // POST : ProductController/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(List<int> ProductIds)
        {

            var sessionHandler = new SessionHandler();
            await sessionHandler.GetSession(HttpContext, _signInManager, _logger);
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                await sessionHandler.SignUserOut(_signInManager, _logger);
                return Problem("Please try login in again.");
            }
            Product Product = new();
            var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, Product, ProductOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                var result = new ItemRemoveStatusModel()
                {
                    Message = "You don't have the permission to Delete a product.",
                    Status = "error",
                    DeleteItemsIds = ProductIds
                };
                return Json(result);
            }
            string errorList = "";
            foreach (var ProductId in ProductIds)
            {
                var productFromDb = await _userDb.Product.FindAsync(ProductId);
                if (productFromDb == null)
                {
                    errorList += ProductId.ToString() + " ";
                }
                else
                {
                    _userDb.Product.Remove(productFromDb);
                }
            }
            if (!String.IsNullOrEmpty(errorList))
            {
                var result = new ItemRemoveStatusModel()
                {
                    Message = $"An error ocuured while trying to remove product with productId {errorList}.",
                    Status = "error",
                    DeleteItemsIds = ProductIds
                };
                return Json(result);
            }
            await _userDb.SaveChangesAsync();
            var results = new ItemRemoveStatusModel()
            {
                Message = "Product have been deleted successfully.",
                Status = "success",
                DeleteItemsIds = ProductIds
            };
            return Json(results);

        }
    }
}

