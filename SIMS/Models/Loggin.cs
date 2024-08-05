using Microsoft.AspNetCore.Mvc;

namespace SIMS.Models
{
    public class Loggin : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
