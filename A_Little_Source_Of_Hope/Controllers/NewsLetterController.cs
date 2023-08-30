using A_Little_Source_Of_Hope.Areas.Identity.Data;
using A_Little_Source_Of_Hope.Data;
using A_Little_Source_Of_Hope.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json;

namespace A_Little_Source_Of_Hope.Controllers
{
    [Authorize(Policy = "RequireAdministratorRole")]
    public class NewsletterController : Controller
    {
        private readonly ILogger<NewsletterController> _logger;
        private readonly AppDbContext _AppDb;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        protected IAuthorizationService _AuthorizationService;
        public NewsletterController(ILogger<NewsletterController> logger, AppDbContext AppDb, IEmailSender emailSender, UserManager<AppUser> userManager,
            IAuthorizationService AuthorizationService, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _AppDb = AppDb;
            _emailSender = emailSender;
            _userManager = userManager;
            _AuthorizationService = AuthorizationService;
            _signInManager = signInManager;
        }
        // GET: NewsController
        public async Task<ActionResult> Index()
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
                IEnumerable<News> objNews = _AppDb.News;
                if (_AppDb.News.Any()) { ViewData["News"] = "not null"; }
                else { ViewData["News"] = null; }
                var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, objNews, Operations.Read);
                if (!isAuthorized.Succeeded)
                {
                    TempData["error"] = "You don't have the permission to see Newsletters.";
                    return Forbid();
                }
                return View(objNews);
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

        // GET: NewsController/Create
        public async Task<ActionResult> Create()
        {
            var sessionHandler = new SessionHandler();
            await sessionHandler.GetSession(HttpContext, _signInManager, _logger);
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                await sessionHandler.SignUserOut(_signInManager, _logger);
                return RedirectToPage("Login");
            }
            var isAuthorized = User.IsInRole(Constants.NewsLetterAdministratorsRole);
            if (!isAuthorized)
            {
                TempData["error"] = "You don't have the permission to create a Newsletter.";
                return RedirectToAction("Index", "Admin");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(News news)
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
                if (ModelState.IsValid)
                {
                    var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, news, Operations.Create);
                    if (!isAuthorized.Succeeded)
                    {
                        TempData["error"] = "You don't have the permission to create a Newsletter.";
                        return RedirectToAction("Index");
                    }
                    var NewsFromDb = _AppDb.News.Contains(news);
                    if (NewsFromDb != true)
                    {
                        await _AppDb.News.AddAsync(news);
                        await _AppDb.SaveChangesAsync();
                        TempData["success"] = "Newsletter successfully created";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Newsletter already exists.");
                        TempData["error"] = "Newsletter already exists";
                        return View(news);
                    }
                }
                TempData["error"] = "Please fill all required fields";
                return View(news);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View(news);
            }
        }

        // GET: NewsController/Edit/5
        public async Task<ActionResult> Edit(int? id)
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
                if (id is null or 0)
                {
                    return NotFound();
                }
                var NewsFromDb = await _AppDb.News.FindAsync(id);
                if (NewsFromDb == null)
                {
                    return NotFound();
                }
                var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, NewsFromDb, Operations.Update);
                if (!isAuthorized.Succeeded)
                {
                    TempData["error"] = "You don't have the permission to edit a Newsletter.";
                    return Forbid();
                }
                return View(NewsFromDb);
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

        // POST: NewsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(News news)
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
                        return RedirectToPage("Login");
                    }
                    var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, news, Operations.Update);
                    if (!isAuthorized.Succeeded)
                    {
                        TempData["error"] = "You don't have the permission to edit a Newsletter.";
                        return Forbid();
                    }
                    var NewsFromDb = await _AppDb.News.ContainsAsync(news);
                    if (!NewsFromDb)
                    {
                        return NotFound();
                    }
                    _AppDb.News.Update(news);
                    await _AppDb.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                return View(news);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    ViewData["error"] = ex.ToString();
                }
                return View(news);
            }
        }
        [HttpPost]
        public async Task<JsonResult> Delete(List<int> NewsletterIds)
        {
            ItemRemoveStatusModel results = new();
            var sessionHandler = new SessionHandler();
            await sessionHandler.GetSession(HttpContext, _signInManager, _logger);
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                await sessionHandler.SignUserOut(_signInManager, _logger);
                results.Status = "Login";
                results.Message = "User autometically logout due to session end";
                return Json(JsonConvert.SerializeObject(results));
            }
            if (NewsletterIds.Count == 0)
            {
                results.Message = "An error occured while deleting selected item, please try again.";
                results.Status = "error";
                results.DeleteItemsIds = NewsletterIds;
                return Json(JsonConvert.SerializeObject(results));
            }
            News news = await _AppDb.News.FindAsync(NewsletterIds[0]);
            var isAuthorized = await _AuthorizationService.AuthorizeAsync(User, news, Operations.Delete);
            if (!isAuthorized.Succeeded)
            {
                results.Message = "You don't have the permission to Delete a newsletter.";
                results.Status = "error";
                results.DeleteItemsIds = NewsletterIds;
                return Json(JsonConvert.SerializeObject(results));
            }
            string errorList = "";
            foreach (var NewsletterId in NewsletterIds)
            {
                var NewsletterFromDb = await _AppDb.News.FindAsync(NewsletterId);
                if (NewsletterFromDb == null)
                {
                    errorList += NewsletterId.ToString() + " ";
                }
                else
                {
                    _AppDb.News.Remove(NewsletterFromDb);
                }
            }
            if (!String.IsNullOrEmpty(errorList))
            {
                results.Message = $"An error ocuured while trying to remove newsletter with newsletterId {errorList}.";
                results.Status = "error";
                results.DeleteItemsIds = NewsletterIds;
                return Json(JsonConvert.SerializeObject(results));
            }
            await _AppDb.SaveChangesAsync();
            if (await _AppDb.News.AnyAsync()) { ViewData["News"] = "not null"; }
            else { ViewData["News"] = null; }
            results.Message = "News have been deleted successfully.";
            results.Status = "success";
            results.DeleteItemsIds = NewsletterIds;
            return Json(JsonConvert.SerializeObject(results));
        }
        public async Task Subscribe(string Email)
        {
            ItemRemoveStatusModel results = new();
            try
            {
                var subcriber = await _AppDb.NewsSubscriptions.FirstOrDefaultAsync(x => x.Email == Email);
                if (subcriber != null)
                {
                    TempData["error"] = "Already subscribed.";
                }
                else
                {
                    var subscribe = new NewsSubscription
                    {
                        Email = Email,
                        Subscribed = true,
                        CreatedDate = DateTime.Now
                    };
                    await _AppDb.NewsSubscriptions.AddAsync(subscribe);
                    await _AppDb.SaveChangesAsync();
                    await _emailSender.SendEmailAsync(Email, "Welcome to our Newsletter", "You have s" +
                        "uccessfully subscribed to our news letter.");
                    TempData["success"] = "Successfully subscribed to our news letter.";
                }
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    TempData["error"] = "An error occured please try again.";
                    ViewData["error"] = ex.ToString();
                }
            }
        }
    }
}
