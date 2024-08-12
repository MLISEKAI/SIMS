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
        public async Task<IActionResult> Login(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = "Username or password cannot be left blank.";
                return View();
            }

            // Convert password to MD5 hash

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userName && u.Pass == password);

            if (user != null) // Check if user is found
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, user.UserRole)
            };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20),
                    IssuedUtc = DateTimeOffset.UtcNow,
                    IsPersistent = true,
                    RedirectUri = "/Users/Index"
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Redirect based on user role
                if (user.UserRole == "Student")
                {
                    // Redirect to the student account page
                    var student = await _context.Students.FirstOrDefaultAsync(s => s.Student_ID == user.ID);
                    if (student != null)
                    {
                        return RedirectToAction("Home", "Student", new { id = student.Student_ID });
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Student account not found.");
                    }
                    
                }
                else if (user.UserRole == "Teacher")
                {
                    var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Teacher_ID == user.ID);
                    if (teacher != null)
                    {
                        return RedirectToAction("Home", "Teachers", new { id = teacher.Teacher_ID });
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Teacher account not found.");
                    }
                }
                else if (user.UserRole == "Admin")
                {
                    return RedirectToAction("Home", "Admin");
                }
            }

            // If user is not found or role is not matched
            ViewBag.ErrorMessage = "Username or password is incorrect.";
            return View(); // Ensure a return statement is present
        }
    }
}
