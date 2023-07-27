using Microsoft.AspNetCore.Mvc;

namespace A_Little_Source_Of_Hope.Controllers
{
    public class VolunteerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult details()
        {
            return View();
        }
    }
}
