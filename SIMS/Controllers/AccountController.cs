using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIMS.Data;

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SIMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly SIMSContext _context;


        public AccountController(SIMSContext context)
        {
            _context = context;
            try
            {
                _context.Database.CanConnect();
                // Cơ sở dữ liệu đã kết nối thành công
                Console.WriteLine("Database connection successful.");
            }
            catch (Exception ex)
            {
                // Xử lý khi không kết nối được
                Console.WriteLine($"Database connection failed: {ex.Message}");
            }
        }


        // GET: Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không được để trống.";
                return View();
            }

            // Convert password to MD5 hash

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userName && u.Pass == password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.Role, user.UserRole)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    // Allow refreshing the authentication session.
                    AllowRefresh = true,
                    // The time at which the authentication ticket expires. A 
                    // value set here overrides the ExpireTimeSpan option of 
                    // CookieAuthenticationOptions set with AddCookie.
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20),
                    // The time at which the authentication ticket was issued.
                    IssuedUtc = DateTimeOffset.UtcNow,
                    // Whether the authentication session is persisted across 
                    // multiple requests. Required when setting the 
                    // ExpireTimeSpan option of CookieAuthenticationOptions 
                    // set with AddCookie. Also required when setting 
                    // ExpiresUtc.
                    IsPersistent = true,
                    // The full path or absolute URI to be used as an http 
                    // redirect response value.
                    RedirectUri = "/Users/Index"
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);


                return RedirectToAction("Home", "Admin");
            }
            else
            {
                ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return View();
            }
        }
      

    }

}
