using A_Little_Source_Of_Hope.Areas.Identity.Data;
using A_Little_Source_Of_Hope.Data;
using A_Little_Source_Of_Hope.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace A_Little_Source_Of_Hope.Controllers.Admin
{
    [Authorize(Policy = "RequireAdministratorRole")]
    public class AdminController : Controller
    {
        private readonly ILogger<ShoppingCartController> _logger;
        private readonly AppDbContext _AppDb;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager; 
        public AdminController(ILogger<ShoppingCartController> logger, AppDbContext AppDb, UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _AppDb = AppDb;
            _userManager = userManager;
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
                var transactions = await _AppDb.Transactions.FirstOrDefaultAsync(x => x.Type == "Donate");
                var amount = transactions == null? 0 : transactions.Amount;
                AdminDashboard adminDashboard = new()
                {
                    NumberofProducts = _AppDb.Product.Count(),
                    NumberofOrders = 0,
                    DonatedAmount = amount,
                    NumberofOrphanages = _AppDb.Orphanage.Count(),
                    SubscribersList = _AppDb.NewsSubscriptions,
                    VolunteerApps = new() 
                    {
                        NumberofApprovedApps = (_AppDb.Volunteer.Select(x => x.Status == "Approved")).Count(),
                        NumberofPendingApps = (_AppDb.Volunteer.Select(x => x.Status == "Pending")).Count(),
                        NumberofRejectedApps = (_AppDb.Volunteer.Select(x => x.Status == "Rejected")).Count(),
                        NumbnerOfApplications = _AppDb.Volunteer.Count()
                    }
                };
                if (adminDashboard.SubscribersList.Any())
                {
                    ViewData["SubscribersList"] = true;
                }
                else
                    ViewData["SubscribersList"] = null;
                return View(adminDashboard); 
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
    }
}
