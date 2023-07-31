using Microsoft.AspNetCore.Mvc;
using A_Little_Source_Of_Hope.Data;
using A_Little_Source_Of_Hope.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace A_Little_Source_Of_Hope.Models
{
    [Authorize(Roles = "ProductAdministrators")]
    public class OrphanageController : Controller
    {
        private readonly ILogger<OrphanageController> _logger;
        private readonly AppDbContext _AppDb;
        private readonly UserManager<AppUser> _userManager;
        protected IAuthorizationService _AuthorizationService;
        private readonly SignInManager<AppUser> _signInManager;

        public OrphanageController(ILogger<OrphanageController> logger, AppDbContext AppDb, UserManager<AppUser> userManager,
            IAuthorizationService AuthorizationService, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _AppDb = AppDb;
            _userManager = userManager;
            _AuthorizationService = AuthorizationService;
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
                var isAuthorized = User.IsInRole(Constants.ProductAdministratorsRole);
                if (!isAuthorized)
                {
                    TempData["error"] = "You don't have the permission to see orphanages.";
                    return RedirectToAction("Index", "Admin");
                }
                IEnumerable<Orphanage> objOrphanage = _AppDb.Orphanage;
                if (_AppDb.Orphanage.Any()) { ViewData["orphanages"] = "not null"; }
                else { ViewData["orphanages"] = null; }
                return View(objOrphanage);
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
        public async Task<IActionResult> Create()
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
                TempData["error"] = "You don't have the permission to create orphanages.";
                return RedirectToAction("Index", "Admin");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Orphanage orphanage)
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
                    var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, orphanage, ProductOperations.Create);
                    if (!isAuthorized.Succeeded)
                    {
                        TempData["error"] = "You don't have the permission to create an orphanage.";
                        return RedirectToAction("Index");
                    }
                    var orphanageFromDb = _AppDb.Orphanage.Contains(orphanage);
                    if (orphanageFromDb != true)
                    {
                        await _AppDb.Orphanage.AddAsync(orphanage);
                        await _AppDb.SaveChangesAsync();
                        TempData["success"] = "Orphanage successfully created";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Orphanage already exists.");
                        TempData["error"] = "Orphanage already exists";
                        return View(orphanage);
                    }
                }
                return View(orphanage);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View(orphanage);
            }
        }
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
                var isAuthorized = User.IsInRole(Constants.ProductAdministratorsRole);
                if (!isAuthorized)
                {
                    TempData["error"] = "You don't have the permission to edit an orphanage.";
                    return RedirectToAction("Index");
                }

                if (id is null or 0)
                {
                    return NotFound();
                }
                var orphanageFromDb = await _AppDb.Orphanage.FindAsync(id);
                if (orphanageFromDb == null)
                {
                    return NotFound();
                }
                return View(orphanageFromDb);
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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Orphanage orphanage)
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
                    var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, orphanage, ProductOperations.Update);
                    if (!isAuthorized.Succeeded)
                    {
                        TempData["error"] = "You don't have the permission to edit an orphanage.";
                        return RedirectToAction("Index");
                    }
                    var orphanageFromDb = await _AppDb.Orphanage.FindAsync(orphanage.Id);
                    if (orphanageFromDb == null)
                    {
                        return NotFound();
                    }
                    _AppDb.Orphanage.Update(orphanageFromDb);
                    await _AppDb.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(orphanage);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View(orphanage);
            }
        }
        [HttpPost]
        public async Task<JsonResult> Delete(List<int> orphanageIds)
        {
            ItemRemoveStatusModel results = new();
            var sessionHandler = new SessionHandler();
            await sessionHandler.GetSession(HttpContext, _signInManager, _logger);
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                await sessionHandler.SignUserOut(_signInManager, _logger);
            }
            if (orphanageIds.Count == 0)
            {
                results.Message = "An error occured while deleting selected item, please try again.";
                results.Status = "error";
                results.DeleteItemsIds = orphanageIds;
                return Json(JsonConvert.SerializeObject(results));
            }
            Orphanage Orphanage = await _AppDb.Orphanage.FindAsync(orphanageIds[0]);
            var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, Orphanage, ProductOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                results.Message = "You don't have the permission to Delete an orphanage.";
                results.Status = "error";
                results.DeleteItemsIds = orphanageIds;
                return Json(JsonConvert.SerializeObject(results));
            }
            string errorList = "";
            foreach (var OrphanageId in orphanageIds)
            {
                var orphanageFromDb = await _AppDb.Orphanage.FindAsync(OrphanageId);
                if (orphanageFromDb == null)
                {
                    errorList += OrphanageId.ToString() + " ";
                }
                else
                {
                    _AppDb.Orphanage.Remove(orphanageFromDb);
                }
            }
            if (!String.IsNullOrEmpty(errorList))
            {
                results.Message = $"An error ocuured while trying to remove Orphanage with OrphanageId {errorList}.";
                results.Status = "error";
                results.DeleteItemsIds = orphanageIds;
                return Json(JsonConvert.SerializeObject(results));
            }
            await _AppDb.SaveChangesAsync();
            if (await _AppDb.Orphanage.AnyAsync()) { ViewData["orphanages"] = "not null"; }
            else { ViewData["orphanages"] = null; }
            results.Message = "Product have been deleted successfully.";
            results.Status = "success";
            results.DeleteItemsIds = orphanageIds;
            return Json(JsonConvert.SerializeObject(results));
        }
    }
}
