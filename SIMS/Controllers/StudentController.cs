using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIMS;
using SIMS.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Text;
using System.IO;
using NuGet.DependencyResolver;


namespace SIMS.Controllers
{
    [Authorize(Roles = "Admin,Student")]
    public class StudentController : Controller
    {
        private readonly SIMSContext _context;

        public StudentController(SIMSContext context)
        {
            _context = context;
        }

        //Student/Home
        public async Task<IActionResult> Home(int id)
        {
            var student = await _context.Students
                .Include(s => s.Students_Courses) // Include related courses
                .ThenInclude(sc => sc.Course) // Include course details
                .FirstOrDefaultAsync(s => s.Student_ID == id);


            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }


        //Student/Logout
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

       

        // GET: Student/Courses/5
        public async Task<IActionResult> Courses(int id)
        {
            var student = await _context.Students
                .Include(s => s.Students_Courses)
                .ThenInclude(sc => sc.Course)
                .FirstOrDefaultAsync(s => s.Student_ID == id);

            if (student == null)
            {
                return NotFound();
            }

            var availableCourses = await _context.Courses.ToListAsync();
            var enrolledCourseIds = student.Students_Courses.Select(sc => sc.Course_ID).ToList();

            ViewData["Student"] = student;
            ViewData["AvailableCourses"] = availableCourses;
            ViewData["EnrolledCourseIds"] = enrolledCourseIds;

            return View(student);
        }

        // Post: Student/Courses/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Courses(int id, List<int> selectedCourses)
        {
            var student = await _context.Students
                .Include(s => s.Students_Courses)
                .FirstOrDefaultAsync(s => s.Student_ID == id);

            if (student == null)
            {
                return NotFound();
            }

            var studentCourses = new HashSet<int>(student.Students_Courses.Select(c => c.Course_ID));
            foreach (var course in _context.Courses)
            {
                if (selectedCourses.Contains(course.Course_ID))
                {
                    if (!studentCourses.Contains(course.Course_ID))
                    {
                        student.Students_Courses.Add(new Students_Courses { Student_ID = student.Student_ID, Course_ID = course.Course_ID });
                    }
                }
                else
                {
                    if (studentCourses.Contains(course.Course_ID))
                    {
                        Students_Courses courseToRemove = student.Students_Courses.FirstOrDefault(c => c.Course_ID == course.Course_ID);
                        if (courseToRemove != null)
                        {
                            _context.Remove(courseToRemove);
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
           
            if (User.IsInRole("Student"))
            {
                
                return RedirectToAction("Home", "Student", new { id = student.Student_ID });
            }

            return RedirectToAction("Index", "Home");
        }

        


        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Students_Courses)
                .FirstOrDefaultAsync(s => s.Student_ID == id);

            if (student == null)
            {
                return NotFound();
            }

            ViewData["Courses"] = new MultiSelectList(_context.Courses, "Course_ID", "Course_Name", student.Students_Courses.Select(sc => sc.Course_ID));
            return View(student);
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Student_ID == id);
        }



        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Student_ID,Student_Name,DateOfBirth,Address,Phone,Email")] Students student, int[] selectedCourses)
        {
            if (id != student.Student_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();

                    var existingCourses = _context.Students_Courses.Where(sc => sc.Student_ID == student.Student_ID).ToList();
                    _context.Students_Courses.RemoveRange(existingCourses);

                    foreach (var courseId in selectedCourses)
                    {
                        var studentCourse = new Students_Courses
                        {
                            Student_ID = student.Student_ID,
                            Course_ID = courseId
                        };
                        _context.Students_Courses.Add(studentCourse);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Student_ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Home), new { id = student.Student_ID });
            }

            ViewData["Courses"] = new MultiSelectList(_context.Courses, "Course_ID", "Course_Name", selectedCourses);
            return View(student);
        }




        // GET: /Export
        public async Task<IActionResult> Export()
        {
            var students = await _context.Students
                .Include(s => s.Students_Courses)
                    .ThenInclude(sc => sc.Course)
                .Include(s => s.Students_Courses)
                    .ThenInclude(sc => sc.Course)
                .ToListAsync();

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream, Encoding.UTF8);
            var csvContent = new StringBuilder();

            // Write the header
            csvContent.AppendLine("Student_ID,Student_Name,Course_Name,Course_code,Description,Score,Grade");

            // Write the data
            foreach (var student in students)
            {
                foreach (var studentCourse in student.Students_Courses)
                {
                    var course = studentCourse.Course;
                    var score = _context.Scores.FirstOrDefault(s => s.Student_ID == student.Student_ID && s.Course_ID == course.Course_ID);

                    csvContent.AppendLine($"{student.Student_ID},{student.Student_Name},{course.Course_Name},{course.Course_code},{course.Description},{score?.Score},{score?.Grade}");
                }
            }

            writer.Write(csvContent.ToString());
            writer.Flush();
            stream.Position = 0;

            string fileName = "Students.csv";
            string contentType = "text/csv";

            return File(stream, contentType, fileName);
        }

        // GET: Scores
        public async Task<IActionResult> Index(int? id, int studentId)
        {
            var scores = await _context.Scores
                .Include(s => s.Student)
                .Include(s => s.Teacher)
                .Include(s => s.Course)
                .ToListAsync();
            Console.WriteLine("Scores retrieved: " + string.Join(", ", scores.Select(s => s.Score_ID)));
            
            ViewData["StudentId"] = studentId;
            return View(scores);
        }

    }
}
