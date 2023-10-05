using Microsoft.AspNetCore.Mvc;

namespace BedAndBreakfastGroup16.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
