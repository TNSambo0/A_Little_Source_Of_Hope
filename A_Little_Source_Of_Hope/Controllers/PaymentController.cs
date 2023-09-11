using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using A_Little_Source_Of_Hope.Models;
using A_Little_Source_Of_Hope.Data;
using Microsoft.AspNetCore.Identity;
using A_Little_Source_Of_Hope.Areas.Identity.Data;
using System.Security.Cryptography;

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
        public PaymentController(AppDbContext AppDb, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            ILogger<PaymentController> logger, IAuthorizationService AuthorizationService)
        {
            _AppDb = AppDb;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _AuthorizationService = AuthorizationService;
        }
        [Authorize(Policy = "Customers")]
        public async Task<IActionResult> Payment(decimal Amount, string type)
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
                    Amount = Math.Round(Amount, 2),
                    Type = type,
                    AppUserId = user.Id,

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

        public async Task<IActionResult> Transaction()
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
                var TransactionHistory = from transaction in _AppDb.Transactions
                                         join aUser in _AppDb.Users
                                        on transaction.AppUserId equals aUser.Id
                                         select new Transaction
                                         {
                                             Id = transaction.Id,
                                             FirstName = aUser.FirstName,
                                             Type = transaction.Type,
                                             LastName = aUser.LastName,
                                             DateCreated = transaction.DateCreated,
                                             Amount = transaction.Amount,
                                             AppUserId = aUser.Id
                                         };

                if (_AppDb.Transactions.Any())
                {
                    ViewData["TransactionHistory"] = true;
                    return View(TransactionHistory.AsEnumerable());
                }
                ViewData["TransactionHistory"] = null;
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
        [Authorize(Policy = "Customers")]
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
                    Transaction transaction = new()
                    {
                        FirstName = user.FirstName,
                        Type = payment.Type,
                        LastName = user.LastName,
                        DateCreated = DateTime.Now,

                        Amount = payment.Amount,
                        AppUserId = payment.AppUserId,

                    };
                    await _AppDb.Transactions.AddAsync(transaction);
                    await _AppDb.Payments.AddAsync(payment);
                    await _AppDb.SaveChangesAsync();
                    TempData["success"] = "Payment successful";
                    return RedirectToAction("Index","Home");
                }
                ModelState.AddModelError(String.Empty, "Please provide all the required information");
                TempData["error"] = "Please provide all the required information";
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
