using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIMS;
using SIMS.Data;
using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace SIMS.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    public class TeachersController : Controller
    {
        private readonly SIMSContext _context;

        public TeachersController(SIMSContext context)
        {
            _context = context;
        }

        //Teachers/Home
        public async Task<IActionResult> Home(int id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Teachers_Courses) 
                .ThenInclude(tc => tc.Course)
                .FirstOrDefaultAsync(t => t.Teacher_ID == id);

            if (teacher == null)
            {
                return NotFound();
            }
            return View(teacher);

        }

        //Teachers/Logout
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }



        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.Teacher_ID == id);
        }


        // GET: Teachers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Teachers == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .Include(t => t.Teachers_Courses) // Ensure that the related data is loaded
                .FirstOrDefaultAsync(t => t.Teacher_ID == id);

            if (teacher == null)
            {
                return NotFound();
            }

            // Ensure that Teachers_Courses is not null
            var selectedCourseIds = teacher.Teachers_Courses?.Select(tc => tc.Course_ID) ?? Enumerable.Empty<int>();

            ViewData["Courses"] = new MultiSelectList(_context.Courses, "Course_ID", "Course_Name", selectedCourseIds);
            return View(teacher);
        }


        // POST: Teachers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Teacher_ID,Teacher_Name,DateOfBirth,Address,Phone,Email")] Teachers teacher, int[] selectedCourses)
        {
            if (id != teacher.Teacher_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacher);
                    await _context.SaveChangesAsync();

                    var existingCourses = _context.Teachers_Courses.Where(tc => tc.Teacher_ID == teacher.Teacher_ID).ToList();
                    _context.Teachers_Courses.RemoveRange(existingCourses);

                    if (selectedCourses != null)
                    {
                        foreach (var courseId in selectedCourses)
                        {
                            var teacherCourse = new Teachers_Courses
                            {
                                Teacher_ID = teacher.Teacher_ID,
                                Course_ID = courseId
                            };
                            _context.Teachers_Courses.Add(teacherCourse);
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.Teacher_ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Home), new { id = teacher.Teacher_ID });
            }

            ViewData["Courses"] = new MultiSelectList(_context.Courses, "Course_ID", "Course_Name", selectedCourses ?? Enumerable.Empty<int>());
            return View(teacher);
        }


    

        // GET: Teachers/Courses/5
        public async Task<IActionResult> Courses(int id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Teachers_Courses) 
                .ThenInclude(tc => tc.Course) 
                .FirstOrDefaultAsync(t => t.Teacher_ID == id);

            if (teacher == null)
            {
                return NotFound();
            }

            var availableCourses = await _context.Courses.ToListAsync();
            var enrolledCourseIds = teacher.Teachers_Courses.Select(tc => tc.Course_ID).ToList();

            ViewData["Teacher"] = teacher;
            ViewData["AvailableCourses"] = availableCourses;
            ViewData["EnrolledCourseIds"] = enrolledCourseIds;

            return View(teacher);
        }


        // POST: Teachers/Courses/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Courses(int id, List<int> selectedCourses)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Teachers_Courses) // Adjust the navigation property name if different
                .FirstOrDefaultAsync(t => t.Teacher_ID == id);

            if (teacher == null)
            {
                return NotFound();
            }

            var teacherCourses = new HashSet<int>(teacher.Teachers_Courses.Select(tc => tc.Course_ID));

            foreach (var courseId in selectedCourses)
            {
                if (!teacherCourses.Contains(courseId))
                {
                    teacher.Teachers_Courses.Add(new Teachers_Courses { Teacher_ID = teacher.Teacher_ID, Course_ID = courseId });
                }
            }

            var coursesToRemove = teacher.Teachers_Courses
                .Where(tc => !selectedCourses.Contains(tc.Course_ID))
                .ToList();

            _context.Teachers_Courses.RemoveRange(coursesToRemove);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Courses), new { id = teacher.Teacher_ID });
        }





        // GET: Users/Export
        public async Task<IActionResult> Export(int? id)
        {
            var students = await _context.Students
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

            string fileName = "Teacher.csv";
            string contentType = "text/csv";

            return File(stream, contentType, fileName);
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

            var students = new List<Students>();
            var courses = new List<Courses>();
            var scores = new List<Scores>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);

                using var reader = new StreamReader(stream);
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

                    // Ensure the CSV has the expected number of columns
                    if (values.Length >= 7)
                    {
                        var studentId = int.Parse(values[0]);
                        var student = students.FirstOrDefault(s => s.Student_ID == studentId) ?? new Students
                        {
                            Student_ID = studentId,
                            Student_Name = values[1]
                        };

                        if (!students.Contains(student))
                        {
                            students.Add(student);
                        }

                        var course = new Courses
                        {
                            Course_Name = values[2],
                            Course_code = values[3],
                            
                        };

                        if (!courses.Contains(course))
                        {
                            courses.Add(course);
                        }

                        var score = new Scores
                        {
                            Student_ID = studentId,
                            Course = course,
                            Score = values[5],
                            Grade = values[6]
                        };

                        scores.Add(score);
                    }
                }
            }

            try
            {
                _context.Students.AddRange(students);
                _context.Courses.AddRange(courses);
                _context.Scores.AddRange(scores);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("file", $"Error uploading file: {ex.Message}");
                return View("Import");
            }
        }

        // GET: Scores
        public async Task<IActionResult> Index(int? id, int teacherId)
        {
            var scores = await _context.Scores
                .Include(s => s.Student)
                .Include(s => s.Teacher)
                .Include(s => s.Course)
                .ToListAsync();
            Console.WriteLine("Scores retrieved: " + string.Join(", ", scores.Select(s => s.Score_ID)));

            ViewData["StudentId"] = teacherId;
            return View(scores);
        }

    }
}