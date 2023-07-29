using Microsoft.AspNetCore.Mvc;
using A_Little_Source_Of_Hope.Data;
using A_Little_Source_Of_Hope.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace A_Little_Source_Of_Hope.Models
{
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
                    TempData["error"] = "You don't have the permission to see Product.";
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
            Orphanage orphanage = new();
            var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, orphanage, ProductOperations.Create);
            if (!isAuthorized.Succeeded)
            {
                TempData["error"] = "You don't have the permission to create a product.";
                return RedirectToAction("Index");
            }
            return View();
        }public IActionResult Orphanage_edit()
        {
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

    }
}
