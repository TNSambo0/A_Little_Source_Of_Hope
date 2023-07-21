using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using A_Little_Source_Of_Hope.Models;
using A_Little_Source_Of_Hope.Areas.Identity.Data;
using A_Little_Source_Of_Hope.Data;

namespace A_Little_Source_Of_Hope.Controllers
{
    [Authorize(Roles = "ProductAdministrators")]
    public class CategoryController : Controller
    {
        private readonly ILogger<ShoppingCartController> _logger;
        private readonly AppDbContext _userDb;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        protected IAuthorizationService _AuthorizationService;
        public CategoryController(ILogger<ShoppingCartController> logger, AppDbContext userDb, UserManager<AppUser> userManager,
            IAuthorizationService AuthorizationService,SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _userDb = userDb;
            _userManager = userManager;
            _AuthorizationService = AuthorizationService;
            _signInManager = signInManager;
        }
        // GET: CategoryController
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
                IEnumerable<Category> objCategory = _userDb.Category;
                var CategoryFromDb = _userDb.Category.Any();
                if (CategoryFromDb == true) { ViewBag.Category = true; }
                else { ViewBag.Category = false; }
                return View(objCategory);
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

        // GET: CategoryController/Create
        public async Task<ActionResult> Create()
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
                return View();
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

        // POST: CategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Category category)
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
                if (ModelState.IsValid)
                {
                    var CategoryFromDb = _userDb.Category.Contains(category);
                    if (CategoryFromDb != true)
                    {
                        var filename = Path.GetFileName(category.File.FileName);
                        var fileExt = Path.GetExtension(category.File.FileName);
                        string fileNameWithoutPath = Path.GetFileNameWithoutExtension(category.File.FileName);
                        string myfile = fileNameWithoutPath + "_" + category.CategoryName + fileExt;
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Category");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        string fileNameWithPath = Path.Combine(path, myfile);
                        category.Imageurl = fileNameWithPath;
                        category.CreatedDate = DateTime.Now;
                        _userDb.Category.Add(category);
                        _userDb.SaveChanges();
                        using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                        {
                            category.File.CopyTo(stream);
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("CategoryName", "Category already exists.");
                        return View(category);
                    }
                }
                return View(category);
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

        // GET: CategoryController/Edit/5
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
                if (id == null || id == 0)
                {
                    return NotFound();
                }
                var categoryFromDb = _userDb.Category.Find(id);
                if (categoryFromDb == null)
                {
                    return NotFound();
                }
                return View(categoryFromDb);
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

        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
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
                return RedirectToAction(nameof(Index));
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

        // GET: CategoryController/Delete/5
        public async Task<ActionResult> Delete(int? id)
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
                if (id == null || id == 0)
                {
                    return NotFound();
                }
                var categoryFromDb = _userDb.Category.Find(id);
                if (categoryFromDb == null)
                {
                    return NotFound();
                }
                return View(categoryFromDb);
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

        // POST: CategoryController/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(List<int> categoryIds)
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
                var Product = new List<Product>();
                var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, Product, ProductOperations.Delete);
                if (!isAuthorized.Succeeded)
                {
                    var result = new ItemRemoveStatusModel()
                    {
                        Message = "You don't have the permission to Delete a product.",
                        Status = "error",
                        DeleteItemsIds = categoryIds
                    };
                    return Json(result);
                }
                string errorList = "";
                foreach (var ProductId in categoryIds)
                {
                    var productFromDb = await _userDb.Product.FindAsync(ProductId);
                    if (productFromDb == null)
                    {
                        errorList += ProductId.ToString() + " ";
                    }
                    else
                    {
                        Product.Add(productFromDb);
                    }
                }
                if (!String.IsNullOrEmpty(errorList))
                {
                    var result = new ItemRemoveStatusModel()
                    {
                        Message = $"An error ocuured while trying to remove product with productId {errorList}.",
                        Status = "error",
                        DeleteItemsIds = categoryIds
                    };
                    return Json(result);
                }
                _userDb.Product.RemoveRange(Product);
                await _userDb.SaveChangesAsync();
                var results = new ItemRemoveStatusModel()
                {
                    Message = "Product have been deleted successfully.",
                    Status = "success",
                    DeleteItemsIds = categoryIds
                };
                return Json(results);
            }
            catch (Exception ex)
            {
                var result = new ItemRemoveStatusModel()
                {
                    Message = $"An error ocuured while trying to remove category. Error -  {ex}.",
                    Status = "error",
                    DeleteItemsIds = categoryIds
                };
                return Json(result);
            }
        }
    }
}
