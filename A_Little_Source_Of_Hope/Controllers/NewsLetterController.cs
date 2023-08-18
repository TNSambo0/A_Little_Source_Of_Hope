using A_Little_Source_Of_Hope.Areas.Identity.Data;
using A_Little_Source_Of_Hope.Data;
using A_Little_Source_Of_Hope.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace A_Little_Source_Of_Hope.Controllers
{
    public class NewsletterController : Controller
    {
        private readonly ILogger<NewsletterController> _logger;
        private readonly AppDbContext _AppDb;
        public NewsletterController(ILogger<NewsletterController> logger, AppDbContext AppDb)
        {
            _logger = logger;
            _AppDb = AppDb;
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task Subscribe(NewsSubscription subscription)
        {
            try
            {
                var subcribers = await _AppDb.NewsSubscriptions.FirstOrDefaultAsync(x => x.Email == subscription.Email);
                if (subcribers != null)
                {
                    TempData["error"] = "Already subscribed.";
                }
                else
                {
                    subscription.Subscribed = true;
                    subscription.CreatedDate = DateTime.Now;
                    await _AppDb.NewsSubscriptions.AddAsync(subscription);
                    await _AppDb.SaveChangesAsync();
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
