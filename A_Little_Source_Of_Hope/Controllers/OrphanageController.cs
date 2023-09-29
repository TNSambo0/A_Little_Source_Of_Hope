using Microsoft.AspNetCore.Mvc;
using A_Little_Source_Of_Hope.Data;
using A_Little_Source_Of_Hope.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace A_Little_Source_Of_Hope.Models
{
    [Authorize(Policy = "RequireAdministratorRole")]
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
                    return RedirectToPage("Login");
                }
                var objOrphanage = from orphanage in _AppDb.Orphanage
                                   join aManager in _AppDb.Users on
                                   orphanage.AppUserId equals aManager.Id
                                   select new Orphanage
                                   {
                                       Id = orphanage.Id,
                                       OrphanageName = orphanage.OrphanageName,
                                       Manager = $"{aManager.FirstName} {aManager.LastName}",
                                       OrphanageEmail = orphanage.OrphanageEmail,
                                       OrphanageAddress = orphanage.OrphanageAddress,
                                       PhoneNumber = orphanage.PhoneNumber,
                                       AppUserId = aManager.Id
                                   };
                if (_AppDb.Orphanage.Any()) { ViewData["orphanages"] = "not null"; }
                else { ViewData["orphanages"] = null; }
                var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, objOrphanage, Operations.Read);
                if (!isAuthorized.Succeeded)
                {
                    TempData["error"] = "You don't have the permission to see orphanges.";
                    return Forbid();
                }
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
                return RedirectToPage("Login");
            }
            var isAuthorized = User.IsInRole(Constants.OrphanageAdministratorsRole);
            if (!isAuthorized)
            {
                TempData["error"] = "You don't have the permission to create orphanages.";
                return RedirectToAction("Index", "Admin");
            }
            var ManagersList = _AppDb.Users.Where(x => x.UserType == "Orphanage Manager");
            var orphanage = new Orphanage
            {
                Managers = ManagersList.Select(x => new SelectListItem { Text = $"{x.FirstName} {x.LastName} ({x.Email})", Value = x.Id }).AsEnumerable(),
                AppUserId = (await ManagersList.FirstAsync()).Id,
            };
            return View(orphanage);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Orphanage orphanage)
        {

            try
            {
                var sessionHandler = new SessionHandler();
                await sessionHandler.GetSession(HttpContext, _signInManager, _logger);
                var ManagersListA = from aManager in _AppDb.Users
                                    join anOrphanage in _AppDb.Orphanage on
                               aManager.Id equals anOrphanage.AppUserId
                                    select aManager;
                var ManagersList = _AppDb.Users.Where(x => x.UserType == "Orphanage Manager");
                orphanage.Managers = ManagersList.Select(x => new SelectListItem { Text = $"{x.FirstName} {x.LastName} ({x.Email})", Value = x.Id }).AsEnumerable();
                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        await sessionHandler.SignUserOut(_signInManager, _logger);
                        return RedirectToPage("Login");
                    }
                    var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, orphanage, Operations.Create);
                    if (!isAuthorized.Succeeded)
                    {
                        TempData["error"] = "You don't have the permission to create an orphanage.";
                        return RedirectToAction("Index");
                    }
                    var selectedManager = await ManagersList.FirstOrDefaultAsync(x => x.Id == orphanage.AppUserId);
                    var orphanageFromDb = await _AppDb.Orphanage.FirstOrDefaultAsync(x => x.AppUserId == orphanage.AppUserId);
                    if (orphanageFromDb != null)
                    {
                        await _AppDb.Orphanage.AddAsync(orphanage);
                        await _AppDb.SaveChangesAsync();
                        var oldRoles = await _userManager.GetRolesAsync(user);
                        var oldRole = oldRoles.Contains("Customer");
                        if (oldRole)
                        {
                            await _userManager.RemoveFromRoleAsync(user, "Customer");
                        }
                        await _userManager.AddToRoleAsync(user, "Orphanage Manager");
                        TempData["success"] = "Orphanage successfully created";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        if (orphanageFromDb.OrphanageName == orphanage.OrphanageName)
                        {
                            ModelState.AddModelError("", "An orphanage with the provided manager already exists.");
                            TempData["error"] = "An orphanage with the provided manager already exists.";
                            return View(orphanage);
                        }
                        ModelState.AddModelError("", "The provided manager is already linked with another orphanage.");
                        TempData["error"] = "The provided manager is already linked with another orphanage.";
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
                    return RedirectToPage("Login");
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
                var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, orphanageFromDb, Operations.Update);
                if (!isAuthorized.Succeeded)
                {
                    TempData["error"] = "You don't have the permission to edit an orphanage.";
                    return Forbid();
                }
                var ManagersList = _AppDb.Users.Where(x => x.UserType == "Orphanage Manager");
                orphanageFromDb.Managers = ManagersList.Select(x => new SelectListItem { Text = $"{x.FirstName} {x.LastName} ({x.Email})", Value = x.Id }).AsEnumerable();
                orphanageFromDb.AppUserId = (await ManagersList.FirstAsync()).Id;
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
                var ManagersList = _AppDb.Users.Where(x => x.UserType == "Orphanage Manager");
                orphanage.Managers = ManagersList.Select(x => new SelectListItem { Text = $"{x.FirstName} {x.LastName} ({x.Email})", Value = x.Id }).AsEnumerable();
                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        await sessionHandler.SignUserOut(_signInManager, _logger);
                        return RedirectToPage("Login");
                    }
                    var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, orphanage, Operations.Update);
                    if (!isAuthorized.Succeeded)
                    {
                        TempData["error"] = "You don't have the permission to edit an orphanage.";
                        return RedirectToAction("Index");
                    }
                    var orphanageFromDb = await _AppDb.Orphanage.ContainsAsync(orphanage);
                    if (!orphanageFromDb)
                    {
                        return NotFound();
                    }
                    _AppDb.Orphanage.Update(orphanage);
                    await _AppDb.SaveChangesAsync();
                    return RedirectToAction("Index");
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
        public async Task<JsonResult> Delete(List<int> OrphanageIds)
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
            if (OrphanageIds.Count == 0)
            {
                results.Message = "An error occured while deleting selected item(s), please try again.";
                results.Status = "error";
                results.DeleteItemsIds = OrphanageIds;
                return Json(JsonConvert.SerializeObject(results));
            }
            Orphanage Orphanage = await _AppDb.Orphanage.FindAsync(OrphanageIds[0]);
            var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, Orphanage, Operations.Delete);
            if (!isAuthorized.Succeeded)
            {
                results.Message = "You don't have the permission to Delete an orphanage.";
                results.Status = "error";
                results.DeleteItemsIds = OrphanageIds;
                return Json(JsonConvert.SerializeObject(results));
            }
            string errorList = "";
            foreach (var OrphanageId in OrphanageIds)
            {
                var OrphanageFromDb = await _AppDb.Orphanage.FindAsync(OrphanageId);
                if (OrphanageFromDb == null)
                {
                    errorList += OrphanageId.ToString() + " ";
                }
                else
                {
                    _AppDb.Orphanage.Remove(OrphanageFromDb);
                }
            }
            if (!String.IsNullOrEmpty(errorList))
            {
                results.Message = $"An error ocuured while trying to remove Orphanage with OrphanageId {errorList}.";
                results.Status = "error";
                results.DeleteItemsIds = OrphanageIds;
                return Json(JsonConvert.SerializeObject(results));
            }
            await _AppDb.SaveChangesAsync();
            results.Message = "Orphanage have been deleted successfully.";
            results.Status = "success";
            results.DeleteItemsIds = OrphanageIds;
            return Json(JsonConvert.SerializeObject(results));
        }
    }
}
 