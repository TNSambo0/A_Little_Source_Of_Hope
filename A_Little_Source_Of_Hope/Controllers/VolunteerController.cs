using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using A_Little_Source_Of_Hope.Data;
using A_Little_Source_Of_Hope.Models;
using A_Little_Source_Of_Hope.Areas.Identity.Data;

namespace A_Little_Source_Of_Hope.Controllers
{
    //[Authorize(Roles = "ProductAdministrators")]
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
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int Id, string userID)
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
                TempData["error"] = "You don't have the permission to change application status.";
                return RedirectToAction("Index", "Admin");
            }
            var applicationFromDb = await _AppDb.Volunteer.FirstOrDefaultAsync(x => x.Id == Id && x.AppUserId == userID);
            if (applicationFromDb == null)
            {
                return Problem("Application not found.");
            }
            applicationFromDb.Status = "Approved";
            _AppDb.Volunteer.Update(applicationFromDb);
            await _AppDb.SaveChangesAsync();
            return RedirectToAction("Details",new { id = Id, UserID = userID });
        }
        public async Task<IActionResult> Details(int id, string UserID)
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
                TempData["error"] = "You don't have the permission to see application details.";
                return RedirectToAction("Index", "Admin");
            }
            var UserDb = await _AppDb.Users.FindAsync(UserID);
            if (UserDb == null)
            {
                return Problem("An error occured, try refreshing the page.");
            }
            var applicationFromDb = from volunteer in _AppDb.Volunteer
                                    join orphanage in _AppDb.Orphanage
                                    on volunteer.OrphanageId equals orphanage.Id
                                    where volunteer.Id == id && UserDb.Id == volunteer.AppUserId
                                    select new VolunteerApp
                                    {
                                        Id = id,
                                        Status = volunteer.Status,
                                        Description = volunteer.Description,
                                        VolunteerDate = (DateTime)volunteer.VolunteerDate,
                                        OrphanageName = orphanage.OrphanageName,
                                        ApplicantFullName = UserDb.FirstName + " " + UserDb.LastName,
                                        AppUserId = UserDb.Id
                                    };
            if (!await applicationFromDb.AnyAsync())
            {
                return View();
            }
            ViewData["VApplications"] = "Not Null";
            var app = await applicationFromDb.SingleOrDefaultAsync();
            var application = new VolunteerApp
            {
                Id = id,
                Status = app.Status,
                Description = app.Description,
                VolunteerDate = app.VolunteerDate,
                OrphanageName = app.OrphanageName,
                ApplicantFullName = app.ApplicantFullName,
                AppUserId = app.AppUserId
            };
            return View(application);
        }
        public async Task<IActionResult> Index()
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
                TempData["error"] = "You don't have the permission to see application details.";
                return RedirectToAction("Index", "Admin");
            }
            var applicationFromDb = from volunteer in _AppDb.Volunteer
                                    join orphanage in _AppDb.Orphanage
                                    on volunteer.OrphanageId equals orphanage.Id
                                    join manager in _AppDb.Users
                                    on orphanage.AppUserId equals manager.Id
                                    select new VolunteerApp
                                    {
                                        Id = volunteer.Id,
                                        AppUserId = volunteer.AppUserId,
                                        Status = volunteer.Status,
                                        Description = volunteer.Description,
                                        VolunteerDate = (DateTime)volunteer.VolunteerDate,
                                        OrphanageName = orphanage.OrphanageName,
                                        OrphanageAddress = orphanage.OrphanageAddress,
                                        OrphanageEmail = orphanage.OrphanageEmail,
                                        OrphanageManager = manager.FirstName + " " + manager.LastName,
                                        OrphanageContact = orphanage.CellNumber
                                    };
            if (!await applicationFromDb.AnyAsync())
            {
                return View();
            }
            ViewData["VApplications"] = "Not Null";
            return View(applicationFromDb);
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int Id, string userID)
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
                TempData["error"] = "You don't have the permission to change application status.";
                return RedirectToAction("Index", "Admin");
            }
            var applicationFromDb = await _AppDb.Volunteer.FirstOrDefaultAsync(x => x.Id == Id && x.AppUserId == userID);
            if (applicationFromDb == null)
            {
                return Problem("Application not found.");
            }
            applicationFromDb.Status = "Rejected";
            _AppDb.Volunteer.Update(applicationFromDb);
            await _AppDb.SaveChangesAsync();
            return RedirectToAction("Details", new { id = Id, UserID = userID });
        }
        public async Task<ActionResult> VolunteerApplication()
        {
            var sessionHandler = new SessionHandler();
            await sessionHandler.GetSession(HttpContext, _signInManager, _logger);
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                await sessionHandler.SignUserOut(_signInManager, _logger);
                return Problem("Please try login in again.");
            }
            var volunteer = new Volunteer
            {
                OrphanageList = _AppDb.Orphanage.Select(x => new SelectListItem() { Text = x.OrphanageName, Value = x.Id.ToString() }).AsEnumerable(),
                VolunteerDate = DateTime.Now,
                Status = "Pending",
                AppUserId = user.Id
            };
            return View(volunteer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VolunteerApplication(Volunteer VApplication)
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
                if (!await _AppDb.Orphanage.AnyAsync())
                {
                    return Problem("Try refreshing the page.");
                }
                VApplication.OrphanageList = _AppDb.Orphanage.Select(x => new SelectListItem() { Text = x.OrphanageName, Value = x.Id.ToString() }).AsEnumerable();
                VApplication.OrphanageId = int.Parse(VApplication.OrphanageID);
                if (ModelState.IsValid)
                {
                    var VApplicationFromDb = _AppDb.Volunteer.Contains(VApplication);
                    if (VApplicationFromDb != true)
                    {
                        await _AppDb.Volunteer.AddAsync(VApplication);
                        await _AppDb.SaveChangesAsync();
                        TempData["success"] = "Volunteering application successfully submitted.";
                        return RedirectToAction("ViewVolunteeringApplication");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Volunteering application for specified orphanage already exists.");
                        TempData["error"] = "Volunteering application for specified orphanage already exists";
                        return View(VApplication);
                    }
                }
                ModelState.AddModelError("", "Fill all field.");
                TempData["error"] = "Please fill all the required fields";

                return View(VApplication);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                TempData["error"] = "An error occured, try to refresh the page";

                return View(VApplication);
            }
        }
        public async Task<IActionResult> ViewVolunteeringApplication()
        {
            var sessionHandler = new SessionHandler();
            await sessionHandler.GetSession(HttpContext, _signInManager, _logger);
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                await sessionHandler.SignUserOut(_signInManager, _logger);
                return Problem("Please try login in again.");
            }
            var Applications = from volunteer in _AppDb.Volunteer
                               join orphanage in _AppDb.Orphanage
                               on volunteer.OrphanageId equals orphanage.Id
                               join manager in _AppDb.Users
                               on orphanage.AppUserId equals manager.Id
                               where user.Id == volunteer.AppUserId
                               select new VolunteerApp
                               {
                                   Status = volunteer.Status,
                                   Description = volunteer.Description,
                                   VolunteerDate = (DateTime)volunteer.VolunteerDate,
                                   OrphanageName = orphanage.OrphanageName,
                                   OrphanageAddress = orphanage.OrphanageAddress,
                                   OrphanageEmail = orphanage.OrphanageEmail,
                                   OrphanageManager = manager.FirstName + " " + manager.LastName,
                                   OrphanageContact = orphanage.CellNumber
                               };
            if (!await Applications.AnyAsync())
            {
                return View();
            }
            ViewData["VApplications"] = "Not Null";
            return View(Applications);
        }
    }
}
