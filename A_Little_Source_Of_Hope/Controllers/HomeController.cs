using A_Little_Source_Of_Hope.Models;
using A_Little_Source_Of_Hope.Areas.Identity.Data;
using A_Little_Source_Of_Hope.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace A_Little_Source_Of_Hope.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _Userdb;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailSender _emailSender;
        public HomeController(ILogger<HomeController> logger, AppDbContext Userdb, UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, IEmailSender emailSender)
        {
            _logger = logger;
            _Userdb = Userdb;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }
        [AllowAnonymous]
        public IActionResult About()
        {
            try
            {
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
        [AllowAnonymous]
        public IActionResult Contact()
        {
            try
            {
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(Contact contact)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    await _emailSender.SendEmailAsync(contact.Email,contact.Subject,contact.Message);
                    await _Userdb.Contact.AddAsync(contact);
                    await _Userdb.SaveChangesAsync();
                    TempData["success"] = "Thank you, query successfully submitted. Will get back to you soon.";
                    return View();
                }
                TempData["error"] = "Please fill all the required fields.";
                return View(contact);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View(contact);
            }
        }
        [AllowAnonymous]
        public IActionResult Donate()
        {
            try
            {
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
        [AllowAnonymous]
        public IActionResult Index()
        {
            try
            {
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
        public async Task<IActionResult> MarketPlace()
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
                if (_Userdb.Product == null)
                {
                    return Problem("No items found");
                }
                var Product = from s in _Userdb.Product select s;
                Product = Product.Where(s => s.IsActive == true);
                if (Product == null)
                {
                    return Problem("No items found");
                }
                if (user.UserType == "Orphanage Manager")
                {
                    var ProductToClaim = Product.Where(s => s.ClaimStatus == true);
                    if (ProductToClaim != null)
                    {
                        foreach (var product in Product)
                        {
                            if (product.ClaimStatus == true)
                            {
                                product.Price = (decimal)0.00;
                            }
                        }
                    }
                }
                else { Product = Product.Where(s => s.ClaimStatus != true); }
                if (Product.Any()) { ViewData["Shop"] = "not null"; }
                else { ViewData["Shop"] = null; }
                return View(Product.AsEnumerable<Product>());
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}