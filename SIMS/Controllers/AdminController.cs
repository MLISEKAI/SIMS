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
using Microsoft.AspNetCore.Authorization;
using NuGet.DependencyResolver;

namespace SIMS.Controllers
{
    [Authorize(Roles = "Admin,Student")]
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
        

        // GET: Student
        public async Task<IActionResult> St()
        {
            return _context.Students != null ?
                        View(await _context.Students.ToListAsync()) :
                        Problem("Entity set 'SIMSContext.Students' is null.");
        }


        // GET: Students/Details/5
        public async Task<IActionResult> DetailsSt(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Students_Courses)
                    .ThenInclude(sc => sc.Course)
                .FirstOrDefaultAsync(m => m.Student_ID == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }



        // GET: Teachers/Details/5
        public async Task<IActionResult> DetailsTc(int? id)
        {
            if (id == null || _context.Teachers == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .Include(t => t.Teachers_Courses)
                    .ThenInclude(tc => tc.Course)
                .FirstOrDefaultAsync(m => m.Teacher_ID == id);

            if (teacher == null)
            {
                return NotFound();  
            }

            return View(teacher);
        }


        // POST: Teachers/AddCourse
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCourseTc([Bind("Course_ID,Course_Name,Course_code,Description")] Courses course, int teacherId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(course);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Course added successfully!";
                }
                catch (DbUpdateException dbEx) // Bắt lỗi cập nhật cơ sở dữ liệu
                {
                    TempData["Error"] = $"Failed to add course. Database error: {dbEx.InnerException?.Message}";
                }
                catch (Exception ex) // Bắt tất cả các lỗi khác
                {
                    TempData["Error"] = $"Failed to add course. Error: {ex.Message}";
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["Error"] = $"Failed to add course. Errors: {string.Join(", ", errors)}";
            }
            return RedirectToAction(nameof(CoursesTc), new { id = teacherId });
        }

        // GET: Teachers/EditCourse/5
        public async Task<IActionResult> EditCourseTc(int? id, int teacherId)
        {
            if (id == null)
            {
                return NotFound(); // Trả về NotFound nếu id là null
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound(); // Trả về NotFound nếu khóa học không tồn tại
            }
            ViewData["TeacherId"] = teacherId;
            return View(course);
        }


        // POST: Teachers/EditCourse/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourseTc(int id, [Bind("Course_ID,Course_Name,Course_code,Description")] Courses course, int teacherId)
        {
            if (id != course.Course_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Course updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Course_ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(CoursesTc), new { id = teacherId });
            }
            ViewData["TeacherId"] = teacherId;
            return View(course);
        }


        // GET: Teachers/DeleteCourse/5
        public async Task<IActionResult> DeleteCourseTc(int? id, int teacherId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.Course_ID == id);
            if (course == null)
            {
                return NotFound();
            }

            ViewData["TeacherId"] = teacherId;
            return View(course);
        }


        // POST: Teachers/DeleteCourse/5
        [HttpPost, ActionName("DeleteCourseTc")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTc(int id, int teacherId)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound(); // Trả về NotFound nếu khóa học không tồn tại
            }

            try
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Course deleted successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                {
                    return NotFound(); // Trả về NotFound nếu khóa học không còn tồn tại
                }
                else
                {
                    throw; // Ném lại ngoại lệ nếu khóa học vẫn tồn tại
                }
            }

            return RedirectToAction(nameof(CoursesTc), new { id = teacherId });
        }


        // GET: Teachers/Courses/5
        public async Task<IActionResult> CoursesTc(int id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Teachers_Courses) // Adjust the navigation property name if different
                .ThenInclude(tc => tc.Course) // Adjust the navigation property name if different
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
        public async Task<IActionResult> CoursesTc(int id, List<int> selectedCourses)
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

            return RedirectToAction(nameof(DetailsTc), new { id = teacher.Teacher_ID });
        }





        // GET: Student/Courses/5
        public async Task<IActionResult> CoursesSt(int id)
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
        public async Task<IActionResult> CoursesSt(int id, List<int> selectedCourses)
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

                return RedirectToAction(nameof(DetailsSt), new { id = student.Student_ID });
        }

        // Post: Student/Courses/AddCourses

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCourseSt([Bind("Course_ID,Course_Name,Course_code,Description")] Courses course, int studentId)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(course);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Course added successfully!";

                }
                catch (Exception ex) // Catch all other exceptions
                {
                    TempData["Error"] = $"Failed to add course. Error: {ex.Message}";
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["Error"] = $"Failed to add course. Errors: {string.Join(", ", errors)}";
            }
            return RedirectToAction(nameof(CoursesSt), new { id = studentId });
        }

        //// Get: Student/Courses/EditCourses
        public async Task<IActionResult> EditCourseSt(int? id, int studentId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            ViewData["StudentId"] = studentId;
            return View(course);
        }

        // Post: Student/Courses/EditCourses
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourseSt(int id, [Bind("Course_ID,Course_Name,Course_code,Description")] Courses course, int studentId)
        {
            if (id != course.Course_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Course updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Course_ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(CoursesSt), new { id = studentId });
            }
            ViewData["StudentId"] = studentId;
            return View(course);
        }


        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Course_ID == id);
        }

        // Get: Student/Courses/DeleteCourses
        public async Task<IActionResult> DeleteCourseSt(int? id, int studentId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.Course_ID == id);
            if (course == null)
            {
                return NotFound();
            }

            ViewData["StudentId"] = studentId;
            return View(course);
        }


        // POST: Student/Courses/DeleteCourses
        [HttpPost, ActionName("DeleteCourseSt")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourseSt(int id, int studentId)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound(); // Course not found
            }

            try
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Course deleted successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency exception
                if (!CourseExists(id))
                {
                    return NotFound(); // Course no longer exists
                }
                else
                {
                    throw; // Rethrow the exception if the course still exists
                }
            }

            return RedirectToAction(nameof(CoursesSt), new { id = studentId });
        }






        // GET: Students/Edit/5
        public async Task<IActionResult> EditSt(int? id)
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


        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSt(int id, [Bind("Student_ID,Student_Name,DateOfBirth,Address,Phone,Email")] Students student, int[] selectedCourses)
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
                return RedirectToAction(nameof(St));
            }

            ViewData["Courses"] = new MultiSelectList(_context.Courses, "Course_ID", "Course_Name", selectedCourses);
            return View(student);
        }



        // GET: Students/Delete/5
        public async Task<IActionResult> DeleteSt(int? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.Student_ID == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }



        // POST: Students/Delete/5
        [HttpPost, ActionName("DeleteSt")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSt(int id)
        {
            if (_context.Students == null)
            {
                return Problem("Entity set 'SIMSContext.Students' is null.");
            }

            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(St));
        }



        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Student_ID == id);
        }



        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.Teacher_ID == id);
        }


      


        // GET: Teachers
        public async Task<IActionResult> Tc()
        {
            return _context.Teachers != null ?
                        View(await _context.Teachers.ToListAsync()) :
                        Problem("Entity set 'SIMSContext.Teachers' is null.");
        }


        // GET: Teachers/Edit/5
        public async Task<IActionResult> EditTc(int? id)
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
        public async Task<IActionResult> EditTc(int id, [Bind("Teacher_ID,Teacher_Name,DateOfBirth,Address,Phone,Email")] Teachers teacher, int[] selectedCourses)
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
                return RedirectToAction(nameof(Tc));
            }

            ViewData["Courses"] = new MultiSelectList(_context.Courses, "Course_ID", "Course_Name", selectedCourses ?? Enumerable.Empty<int>());
            return View(teacher);
        }


        // GET: Teachers/Delete/5
        public async Task<IActionResult> DeleteTc(int? id)
        {
            if (id == null || _context.Teachers == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(m => m.Teacher_ID == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTc(int id)
        {
            if (_context.Teachers == null)
            {
                return Problem("Entity set 'SIMSContext.Teachers' is null.");
            }
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher != null)
            {
                _context.Teachers.Remove(teacher);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Tc));
        }




        // GET: Users/Export
        public async Task<IActionResult> ExportData(int? id)
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

            string fileName = "Students.csv";
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
                            Description = values[4]
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



















    }
}
