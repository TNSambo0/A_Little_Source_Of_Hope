using Microsoft.AspNetCore.Mvc;
using A_Little_Source_Of_Hope.Models;

namespace A_Little_Source_Of_Hope.Controllers
{
    public class PaymentController : Controller
    {
        //[HttpGet]
        public IActionResult Payment(decimal Amount)
        {
            Payment paymentAmount = new()
            {
                Amount = Math.Round(Amount, 2)
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
        public IActionResult CashDonation()
        {

            return View();
        }
    }
}
