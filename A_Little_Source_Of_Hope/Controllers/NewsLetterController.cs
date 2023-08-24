using A_Little_Source_Of_Hope.Areas.Identity.Data;
using A_Little_Source_Of_Hope.Data;
using A_Little_Source_Of_Hope.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace A_Little_Source_Of_Hope.Controllers
{
    public class NewsletterController : Controller
    {
        private readonly ILogger<NewsletterController> _logger;
        private readonly AppDbContext _AppDb;
        private readonly IEmailSender _emailSender;
        public NewsletterController(ILogger<NewsletterController> logger, AppDbContext AppDb, IEmailSender emailSender)
        {
            _logger = logger;
            _AppDb = AppDb;
            _emailSender = emailSender;
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
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
                    await _emailSender.SendEmailAsync(Email,"Welcome to our Newsletter", "You have s" +
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
