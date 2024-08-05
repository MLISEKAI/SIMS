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
    public class UsersController : Controller
    {
        private readonly SIMSContext _context;

        public UsersController(SIMSContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
            {
                return _context.Users != null ?
                           View(await _context.Users.ToListAsync()) :
                           Problem("Entity set 'StudentManagementContext.Users'  is null.");
            }

        // GET: Logout
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");

        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var users = await _context.Users
                .FirstOrDefaultAsync(m => m.ID == id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,UserName,Pass,Email,UserRole")] Users users)
        {
            if (ModelState.IsValid)
            {
                _context.Add(users);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(users);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }
            return View(users);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,UserName,Pass,Email,UserRole")] Users users)
        {
            if (id != users.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(users);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsersExists(users.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(users);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var users = await _context.Users
                .FirstOrDefaultAsync(m => m.ID == id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'StudentManagementContext.Users'  is null.");
            }
            var users = await _context.Users.FindAsync(id);
            if (users != null)
            {
                _context.Users.Remove(users);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsersExists(int id)
        {
            return (_context.Users?.Any(e => e.ID == id)).GetValueOrDefault();
        }


        // GET: Users/Import
        public IActionResult Import()
        {
            return View();
        }
        // POST: Users/Import
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("file", "Please upload a file.");
                return View("Import");
            }

            if (!Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("file", "File must be a CSV file.");
                return View("Import");
            }

            var users = new List<Users>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(stream))
                {
                    string line;
                    bool isFirstLine = true;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (isFirstLine)
                        {
                            isFirstLine = false;
                            continue; // Skip header row
                        }

                        var values = line.Split(',');
                        if (values.Length == 5)
                        {
                            var user = new Users
                            {
                                UserName = values[1],
                                Pass = values[2],
                                Email = values[3],
                                UserRole = values[4]
                            };

                            users.Add(user);
                        }
                    }
                }
            }

            try
            {
                _context.Users.AddRange(users);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("file", $"Error uploading file: {ex.Message}");
                return View("Import");
            }
        }


        // GET: Users/Export
        public async Task<IActionResult> Export()
        {
            var users = await _context.Users.ToListAsync();

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream, Encoding.UTF8);
            var csvContent = new StringBuilder();

            // Write the header
            csvContent.AppendLine("ID,UserName,Pass,Email,UserRole");

            // Write the data
            foreach (var user in users)
            {
                csvContent.AppendLine($"{user.ID},{user.UserName},{user.Pass},{user.Email},{user.UserRole}");
            }

            writer.Write(csvContent.ToString());
            writer.Flush();
            stream.Position = 0;

            string fileName = "Users.csv";
            string contentType = "text/csv";

            return File(stream, contentType, fileName);
        }

    }
}