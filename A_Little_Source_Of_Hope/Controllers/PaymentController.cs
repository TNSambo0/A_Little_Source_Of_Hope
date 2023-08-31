using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using A_Little_Source_Of_Hope.Models;
using A_Little_Source_Of_Hope.Data;
using Microsoft.AspNetCore.Identity;
using A_Little_Source_Of_Hope.Areas.Identity.Data;

namespace A_Little_Source_Of_Hope.Controllers
{
    //[Authorize(Policy = "Customers")]
    public class PaymentController : Controller
    {
        private readonly AppDbContext _AppDb;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<PaymentController> _logger;
        protected IAuthorizationService _AuthorizationService;
        public PaymentController(AppDbContext AppDb, UserManager<AppUser> userManager ,SignInManager<AppUser> signInManager, 
            ILogger<PaymentController> logger, IAuthorizationService AuthorizationService)
        {
            _AppDb = AppDb;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _AuthorizationService = AuthorizationService;
        }
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<IActionResult> Payment(decimal Amount)
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
                Payment paymentAmount = new()
                {
                    Amount = Math.Round(Amount, 2)
                };
                var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, paymentAmount, Operations.Read);
                if (!isAuthorized.Succeeded)
                {
                    TempData["error"] = "You don't have the permission to see items on cart.";
                    return Forbid();
                }
                return View(paymentAmount);
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
        
        public async Task <IActionResult> CashDonation()
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
                //var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, payment, Operations.Create);
                //if (!isAuthorized.Succeeded)
                //{
                //    TempData["error"] = "You don't have the permission to see submit a payment.";
                //    return Forbid();
                //}
                var CashDonationHistory = _AppDb.CashDonations;
                if( _AppDb.CashDonations.Any())
                {
                    ViewData["CashDonationHistory"] = true;
                    return View(CashDonationHistory.AsEnumerable());
                }
                ViewData["CashDonationHistory"] = null;
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
        [Authorize(Policy = "RequireAdministratorRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Payment(Payment payment)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var sessionHandler = new SessionHandler();
                    await sessionHandler.GetSession(HttpContext, _signInManager, _logger);
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        await sessionHandler.SignUserOut(_signInManager, _logger);
                        return RedirectToPage("Login");
                    }
                    var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, payment, Operations.Create);
                    if (!isAuthorized.Succeeded)
                    {
                        TempData["error"] = "You don't have the permission to see submit a payment.";
                        return Forbid();
                    }
                    return View();
                }
                return View(payment);
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
