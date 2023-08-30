using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using A_Little_Source_Of_Hope.Models;
using A_Little_Source_Of_Hope.Areas.Identity.Data;
using A_Little_Source_Of_Hope.Data;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace A_Little_Source_Of_Hope.Controllers
{
    [Authorize(Policy = "RequireAdministratorRole")]
    public class CategoryController : Controller
    {
        private readonly ILogger<ShoppingCartController> _logger;
        private readonly AppDbContext _AppDb;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        protected IAuthorizationService _AuthorizationService;
        public CategoryController(ILogger<ShoppingCartController> logger, AppDbContext AppDb, UserManager<AppUser> userManager,
            IAuthorizationService AuthorizationService, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _AppDb = AppDb;
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
                    return RedirectToPage("Login");
                }
                IEnumerable<Category> objcategory = _AppDb.Category;
                if (_AppDb.Category.Any()) { ViewData["Category"] = "not null"; }
                else { ViewData["Category"] = null; }
                var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, objcategory, Operations.Read);
                if (!isAuthorized.Succeeded)
                {
                    TempData["error"] = "You don't have the permission to see categories.";
                    return Forbid();
                }
                return View(objcategory);
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

        // GET: categoryController/Create
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
            var isAuthorized = User.IsInRole(Constants.CategoryAdministratorsRole);
            if (!isAuthorized)
            {
                TempData["error"] = "You don't have the permission to create a category.";
                return RedirectToAction("Index", "Admin");
            }
            return View();
        }
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
                    return RedirectToPage("Login");
                }
                if (ModelState.IsValid)
                {
                    var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, category, Operations.Create);
                    if (!isAuthorized.Succeeded)
                    {
                        TempData["error"] = "You don't have the permission to create a category.";
                        return RedirectToAction("Index");
                    }
                    var categoryFromDb = _AppDb.Category.Contains(category);
                    if (categoryFromDb != true)
                    {
                        if (category.File != null && category.File.FileName != null)
                        {
                            var filename = Path.GetFileName(category.File.FileName);
                            var fileExt = Path.GetExtension(category.File.FileName);
                            //string fileNameWithoutPath = Path.GetFileNameWithoutExtension(category.File.FileName);
                            //string myfile = fileNameWithoutPath + "_" + category.CategoryName + fileExt;
                            //var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/category");
                            //if (!Directory.Exists(path))
                            //{
                            //    Directory.CreateDirectory(path);
                            //}
                            //string fileNameWithPath = Path.Combine(path, myfile);
                            category.Imageurl = $"images/Category/{filename}";
                        }
                        category.CreatedDate = DateTime.Now;
                        await _AppDb.Category.AddAsync(category);
                        await _AppDb.SaveChangesAsync();
                        //using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                        //{
                        //    category.File.CopyTo(stream);
                        //}
                        TempData["success"] = "category successfully created";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "category already exists.");
                        TempData["error"] = "category already exists";
                        return View(category);
                    }
                }
                TempData["error"] = "Please fill all required fields";
                return View(category);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View(category);
            }
        }

        // GET: categoryController/Edit/5
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
                var categoryFromDb = await _AppDb.Category.FindAsync(id);
                if (categoryFromDb == null)
                {
                    return NotFound();
                }
                var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, categoryFromDb, Operations.Update);
                if (!isAuthorized.Succeeded)
                {
                    TempData["error"] = "You don't have the permission to edit a category.";
                    return Forbid();
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

        // POST: categoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Category category)
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
                        return RedirectToPage("Login");
                    }
                    var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, category, Operations.Update);
                    if (!isAuthorized.Succeeded)
                    {
                        TempData["error"] = "You don't have the permission to edit a category.";
                        return Forbid();
                    }
                    var categoryFromDb = await _AppDb.Category.ContainsAsync(category);
                    if (!categoryFromDb)
                    {
                        return NotFound();
                    }
                    if (category.File != null && category.File.FileName != null)
                    {
                        var filename = Path.GetFileName(category.File.FileName);
                        var fileExt = Path.GetExtension(category.File.FileName);
                        //string fileNameWithoutPath = Path.GetFileNameWithoutExtension(category.File.FileName);
                        //string myfile = fileNameWithoutPath + "_" + category.CategoryName + fileExt;
                        //var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/category");
                        //if (!Directory.Exists(path))
                        //{
                        //    Directory.CreateDirectory(path);
                        //}
                        //string fileNameWithPath = Path.Combine(path, myfile);
                        category.Imageurl = $"images/Category/{filename}";
                    }
                    _AppDb.Category.Update(category);
                    await _AppDb.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                return View(category);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View(category);
            }
        }
        [HttpPost]
        public async Task<JsonResult> Delete(List<int> CategoryIds)
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
            if (CategoryIds.Count == 0)
            {
                results.Message = "An error occured while deleting selected item, please try again.";
                results.Status = "error";
                results.DeleteItemsIds = CategoryIds;
                return Json(JsonConvert.SerializeObject(results));
            }
            Category category = await _AppDb.Category.FindAsync(CategoryIds[0]);
            var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, category, Operations.Delete);
            if (!isAuthorized.Succeeded)
            {
                results.Message = "You don't have the permission to Delete a Category.";
                results.Status = "error";
                results.DeleteItemsIds = CategoryIds;
                return Json(JsonConvert.SerializeObject(results));
            }
            string errorList = "";
            foreach (var CategoryId in CategoryIds)
            {
                var CategoryFromDb = await _AppDb.Category.FindAsync(CategoryId);
                if (CategoryFromDb == null)
                {
                    errorList += CategoryId.ToString() + " ";
                }
                else
                {
                    _AppDb.Category.Remove(CategoryFromDb);
                }
            }
            if (!String.IsNullOrEmpty(errorList))
            {
                results.Message = $"An error ocuured while trying to remove Category with CategoryId {errorList}.";
                results.Status = "error";
                results.DeleteItemsIds = CategoryIds;
                return Json(JsonConvert.SerializeObject(results));
            }
            await _AppDb.SaveChangesAsync();
            if (await _AppDb.Category.AnyAsync()) { ViewData["Category"] = "not null"; }
            else { ViewData["Category"] = null; }
            results.Message = "Category have been deleted successfully.";
            results.Status = "success";
            results.DeleteItemsIds = CategoryIds;
            return Json(JsonConvert.SerializeObject(results));
        }
    }
}
