using Microsoft.AspNetCore.Mvc;

namespace OnlineShoppingSystem_Main.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
