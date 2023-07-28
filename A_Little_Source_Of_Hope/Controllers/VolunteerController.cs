using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using A_Little_Source_Of_Hope.Data;
using A_Little_Source_Of_Hope.Areas.Identity.Data;

namespace A_Little_Source_Of_Hope.Controllers
{
    public class VolunteerController : Controller
    {
        private readonly ILogger<VolunteerController> _logger;
        private readonly AppDbContext _AppDb;
        private readonly UserManager<AppUser> _userManager;
        protected IAuthorizationService _AuthorizationService;
        private readonly SignInManager<AppUser> _signInManager;

        public VolunteerController(ILogger<VolunteerController> logger, AppDbContext AppDb, UserManager<AppUser> userManager,
            IAuthorizationService AuthorizationService, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _AppDb = AppDb;
            _userManager = userManager;
            _AuthorizationService = AuthorizationService;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> details(int id)
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
            var applicationFromDb = await _AppDb.Volunteer.FindAsync(id);
            if (applicationFromDb == null)
            {
                return Problem("Application not found.");
            }
            return View(applicationFromDb);
        }
        public async Task<IActionResult> Approve(int id)
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
            var applicationFromDb = await _AppDb.Volunteer.FindAsync(id);
            if (applicationFromDb ==null)
            {
                return Problem("Application not found.");
            }
            applicationFromDb.Status = "Approved";
            _AppDb.Volunteer.Update(applicationFromDb);
            await _AppDb.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Reject(int id)
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
            var applicationFromDb = await _AppDb.Volunteer.FindAsync(id);
            if (applicationFromDb == null)
            {
                return Problem("Application not found.");
            }
            applicationFromDb.Status = "Rejected";
            _AppDb.Volunteer.Update(applicationFromDb);
            await _AppDb.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
