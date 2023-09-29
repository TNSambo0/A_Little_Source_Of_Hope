using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using A_Little_Source_Of_Hope.Models;
using A_Little_Source_Of_Hope.Data;
using Microsoft.AspNetCore.Identity;
using A_Little_Source_Of_Hope.Areas.Identity.Data;
using System.Security.Cryptography;

namespace A_Little_Source_Of_Hope.Controllers
{
    [Authorize(Policy = "Customers")]
    public class OrderController : Controller
    {
        private readonly AppDbContext _AppDb;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<OrderController> _logger;
        protected IAuthorizationService _AuthorizationService;
        public OrderController(AppDbContext AppDb, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            ILogger<OrderController> logger, IAuthorizationService AuthorizationService)
        {
            _AppDb = AppDb;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _AuthorizationService = AuthorizationService;
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
    }
}
