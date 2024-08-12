using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIMS;
using SIMS.Data;

namespace SIMS.Controllers
{
    public class ScoresController : Controller
    {
        private readonly SIMSContext _context;

        public ScoresController(SIMSContext context)
        {
            _context = context;
        }

        // GET: Scores
        public async Task<IActionResult> Index()
        {
            var sIMSContext = _context.Scores.Include(s => s.Course).Include(s => s.Student).Include(s => s.Teacher);
            return View(await sIMSContext.ToListAsync());
        }

        // GET: Scores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Scores == null)
            {
                return NotFound();
            }

            var scores = await _context.Scores
                .Include(s => s.Course)
                .Include(s => s.Student)
                .Include(s => s.Teacher)
                .FirstOrDefaultAsync(m => m.Score_ID == id);
            if (scores == null)
            {
                return NotFound();
            }

            return View(scores);
        }

        // GET: Scores/Create
        public IActionResult Create()
        {
            ViewData["Course_ID"] = new SelectList(_context.Courses, "Course_ID", "Course_Name");
            ViewData["Student_ID"] = new SelectList(_context.Students, "Student_ID", "Student_Name");
            ViewData["Teacher_ID"] = new SelectList(_context.Teachers, "Teacher_ID", "Teacher_Name");
            
            return View();
        }

        // POST: Scores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Score_ID,Student_ID,Teacher_ID,Course_ID,Score,Grade")] Scores scores)
        {
            if (ModelState.IsValid)
            {
                _context.Add(scores);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Course_ID"] = new SelectList(_context.Courses, "Course_ID", "Course_Name", scores.Course_ID);
            ViewData["Student_ID"] = new SelectList(_context.Students, "Student_ID", "Student_Name", scores.Student_ID);
            ViewData["Teacher_ID"] = new SelectList(_context.Teachers, "Teacher_ID", "Teacher_Name", scores.Student_ID);
            return View(scores);
        }

        // GET: Scores/Edit/5
        public async Task<IActionResult> Edit()
        {
            //if (id == null || _context.Scores == null)
            //{
            //    return NotFound();
            //}

            //var scores = await _context.Scores.FindAsync(id);
            //if (scores == null)
            //{
            //    return NotFound();
            //}
            //ViewData["Course_ID"] = new SelectList(_context.Courses, "Course_ID", "Course_Name", scores.Course_ID);
            //ViewData["Student_ID"] = new SelectList(_context.Students, "Student_ID", "Address", scores.Student_ID);
            //ViewData["Teacher_ID"] = new SelectList(_context.Teachers, "Teacher_ID", "Address", scores.Teacher_ID);
            return View();
        }

        // POST: Scores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Score_ID,Student_ID,Teacher_ID,Course_ID,Score,Grade")] Scores scores)
        {
           

            if (ModelState.IsValid)
            {
               
                    _context.Update(scores);
                    await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
                

                
            }
            ViewData["Course_ID"] = new SelectList(_context.Courses, "Course_ID", "Course_Name", scores.Course_ID);
            ViewData["Student_ID"] = new SelectList(_context.Students, "Student_ID", "Address", scores.Student_ID);
            ViewData["Teacher_ID"] = new SelectList(_context.Teachers, "Teacher_ID", "Address", scores.Teacher_ID);
            return View(scores);
        }

        // GET: Scores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Scores == null)
            {
                return NotFound();
            }

            var scores = await _context.Scores
                .Include(s => s.Course)
                .Include(s => s.Student)
                .Include(s => s.Teacher)
                .FirstOrDefaultAsync(m => m.Score_ID == id);
            if (scores == null)
            {
                return NotFound();
            }

            return View(scores);
        }

        // POST: Scores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Scores == null)
            {
                return Problem("Entity set 'SIMSContext.Scores'  is null.");
            }
            var scores = await _context.Scores.FindAsync(id);
            if (scores != null)
            {
                _context.Scores.Remove(scores);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScoresExists(int id)
        {
          return (_context.Scores?.Any(e => e.Score_ID == id)).GetValueOrDefault();
        }
    }
}
