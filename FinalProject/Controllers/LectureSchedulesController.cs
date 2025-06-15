using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Controllers
{
    using FinalProject.Data;
    using FinalProject.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using SQLitePCL;

    [Authorize(Roles = "Admin")]
    public class LectureSchedulesController : Controller
    {
        private readonly FinalDbContext _context;

        public LectureSchedulesController(FinalDbContext context)
        {
            _context = context;
        }

        public IActionResult Create(string courseId)
        {
            var model = new LectureSchedule
            {
                CourseId = courseId
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LectureSchedule lectureSchedule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lectureSchedule);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Course", new { id = lectureSchedule.CourseId });
            }
            return View(lectureSchedule);
        }
    }
}
