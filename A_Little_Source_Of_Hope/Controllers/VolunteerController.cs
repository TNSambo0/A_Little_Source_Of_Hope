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
                TempData["error"] = "You don't have the permission to see application details.";
                return RedirectToAction("Index", "Admin");
            }
            var applicationFromDb = await _AppDb.Volunteer.FindAsync(id);
            if (applicationFromDb == null)
            {
                return Problem("Application not found.");
            }
            applicationFromDb = new Volunteer
            {
                Status = applicationFromDb.Status,
                Description = applicationFromDb.Description,
                VolunteerDate = applicationFromDb.VolunteerDate,
                OrphanageName = applicationFromDb.OrphanageName,
                ApplicantFullName = user.FirstName + " "+ user.LastName
            };
            return View(applicationFromDb);
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
            var isAuthorized = User.IsInRole(Constants.ProductAdministratorsRole);
            if (!isAuthorized)
            {
                TempData["error"] = "You don't have the permission to see application details.";
                return RedirectToAction("Index", "Admin");
            }
            var managerFromDb = await _AppDb.Users.FirstOrDefaultAsync(app => app.Id == user.Id);
            var volunteerAppsFromDb = await _AppDb.Volunteer.FirstOrDefaultAsync(app => app.AppUserId == user.Id);
            var orphanageFromDb = await _AppDb.Orphanage.FirstOrDefaultAsync(app => app.AppUserId == managerFromDb.Id);
            if (managerFromDb !=null && volunteerAppsFromDb != null && orphanageFromDb !=null)
            {
                ViewData["VApplications"] = "not null";
            }
            else
            {
                ViewData["VApplications"] = null; ;
            }
            IEnumerable<Volunteer> Applications = _AppDb.Volunteer.Select(x => new Volunteer
            {
                Status = volunteerAppsFromDb.Status,
                Description = volunteerAppsFromDb.Description,
                VolunteerDate = volunteerAppsFromDb.VolunteerDate,
                OrphanageName = orphanageFromDb.OrphanageName,
                OrphanageAddress = orphanageFromDb.OrphanageAddress,
                OrphanageEmail = orphanageFromDb.OrphanageEmail,
                OrphanageManager = managerFromDb.FirstName + " " + managerFromDb.LastName,
                OrphanageContact = orphanageFromDb.CellNumber

            });
            return View(Applications);
        }
        [HttpPost]
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
                TempData["error"] = "You don't have the permission to change application status.";
                return RedirectToAction("Index", "Admin");
            }
            var applicationFromDb = await _AppDb.Volunteer.FindAsync(id);
            if (applicationFromDb == null)
            {
                return Problem("Application not found.");
            }
            applicationFromDb.Status = "Approved";
            _AppDb.Volunteer.Update(applicationFromDb);
            await _AppDb.SaveChangesAsync();
            return RedirectToAction("Index");
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
            var volunteer = new Volunteer {
               OrphanageList = _AppDb.Orphanage.Select(x => new SelectListItem() { Text = x.OrphanageName, Value = x.Id.ToString() }).AsEnumerable(),
               VolunteerDate = DateTime.Now
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
                var OrphanageList = _AppDb.Orphanage.Select(x => new SelectListItem() { Text = x.OrphanageName, Value = x.Id.ToString() }).AsEnumerable();

                if (ModelState.IsValid)
                {
                    var VApplicationFromDb = _AppDb.Volunteer.Contains(VApplication);

                    if (VApplicationFromDb != true)
                    {
                        var selectedOrphanage = OrphanageList.FirstOrDefault(x => x.Value == VApplication.OrphanageId.ToString());
                        VApplication.OrphanageName = selectedOrphanage.Text;
                        await _AppDb.Volunteer.AddAsync(VApplication);
                        await _AppDb.SaveChangesAsync();
                        TempData["success"] = "Volunteering application successfully submitted.";
                        return RedirectToAction("Index");
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

        [HttpPost]
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
                TempData["error"] = "You don't have the permission to change application status.";
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
