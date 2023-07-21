using A_Little_Source_Of_Hope.Areas.Identity.Data;
using A_Little_Source_Of_Hope.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace A_Little_Source_Of_Hope.Controllers.Admin
{
    [Authorize(Roles = "ProductAdministrators")]
    public class AdminController : Controller
    {
        private readonly ILogger<ShoppingCartController> _logger;
        private readonly AppDbContext _userDb;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AdminController(ILogger<ShoppingCartController> logger, AppDbContext userDb, UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _userDb = userDb;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var sessionHandler = new SessionHandler();
                await sessionHandler.GetSession(HttpContext, _signInManager, _logger);

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
