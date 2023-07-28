using Microsoft.AspNetCore.Mvc;
using A_Little_Source_Of_Hope.Data;
namespace A_Little_Source_Of_Hope.Models
{
    public class OrphanageController : Controller
    {
        private readonly AppDbContext _AppDb;
        public OrphanageController(AppDbContext appDb)
        {
            _AppDb = appDb;

        }
        public IActionResult Index()
        {
            IEnumerable<Orphanage> orphanages = _AppDb.Orphanage;
            return View();
        }public IActionResult Create()
        {
            return View();
        }
    }
}
