using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIMS;
using SIMS.Data;

namespace SIMS.Controllers
{
    public class AdminController : Controller
    {
        private readonly SIMSContext _context;

        public AdminController(SIMSContext context)
        {
            _context = context;
        }

        //Admin/Home
        public IActionResult Home()
        {
            return View();
        }
        //Admin/Logout
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }


        public IActionResult Student()
        {
            return View();
        }

        public IActionResult Teacher()
        {
            return View();
        }

        public IActionResult Course()
        {
            return View();
        }

        public IActionResult Score()
        {
            return View();
        }

        public IActionResult Users()
        {
            return View();


        }
    }
}
