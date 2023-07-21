using Microsoft.AspNetCore.Mvc;
using A_Little_Source_Of_Hope.Models;

namespace A_Little_Source_Of_Hope.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Payment(decimal? Amount)
        {
            decimal OrderAmount = (decimal)(Amount == null? 0 : Amount);
            Payment paymentAmount = new()
            {
                Amount = Math.Round(OrderAmount, 2)
            };
            return View(paymentAmount);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Payment(Payment payment)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            return View(payment);
        }
    }
}
