using FinalProject.Data;
using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers
{
    public class CourseController : Controller
    {
        private readonly FinalDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CourseController(FinalDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var myEnrollments = await _context.CourseEnrollments
                .Where(e => e.StudentId == userId && e.Status == "Approved")
                .Include(e => e.Course)
                .ToListAsync();

            var allCourses = await _context.Courses
                .Include(c => c.CourseTeachers)
                    .ThenInclude(ct => ct.Teacher)
                .ToListAsync();

            ViewBag.MyCourses = myEnrollments
                .Select(e => e.Course)
                .Distinct()
                .ToList();

            return View(allCourses);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var teachers = await _userManager.GetUsersInRoleAsync("Teacher");

            var model = new CourseCreateViewModel
            {
                AllTeachers = teachers.Select(t => new SelectListItem
                {
                    Value = t.Id,
                    Text = t.UserName
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CourseCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var teachers = await _userManager.GetUsersInRoleAsync("Teacher");
                model.AllTeachers = teachers.Select(t => new SelectListItem
                {
                    Value = t.Id,
                    Text = t.UserName
                }).ToList();

                return View(model);
            }

            var course = new Course
            {
                Title = model.Title,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                CourseTeachers = model.SelectedTeacherIds.Select(tid => new CourseTeacher
                {
                    TeacherId = tid
                }).ToList()
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Course ID is required.");
            }

            var course = await _context.Courses
                .Include(c => c.LectureSchedules)
                .Include(c => c.CourseTeachers)
                    .ThenInclude(ct => ct.Teacher)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound("Course not found.");
            }

            var approvedEnrollments = await _context.CourseEnrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(e => e.CourseId == id && e.Status == "Approved")
                .ToListAsync();

            ViewBag.ApprovedEnrollments = approvedEnrollments;

            var userId = _userManager.GetUserId(User);
            var enrollment = await _context.CourseEnrollments
                .FirstOrDefaultAsync(e => e.StudentId == userId && e.CourseId == id);

            ViewBag.EnrollmentStatus = enrollment?.Status;
            ViewBag.EnrollmentId = enrollment?.Id;

            ViewBag.ScheduleOptions = new SelectList(course.LectureSchedules, "Id", "DisplayName");

            return View(course);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            var course = await _context.Courses
                .Include(c => c.CourseTeachers)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return NotFound();

            var teachers = await _userManager.GetUsersInRoleAsync("Teacher");

            var model = new CourseCreateViewModel
            {
                Title = course.Title,
                Description = course.Description,
                StartDate = course.StartDate,
                EndDate = course.EndDate,
                SelectedTeacherIds = course.CourseTeachers.Select(ct => ct.TeacherId).ToList(),
                AllTeachers = teachers.Select(t => new SelectListItem
                {
                    Value = t.Id,
                    Text = t.UserName
                }).ToList()
            };

            ViewBag.CourseId = id;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id, CourseCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var teachers = await _userManager.GetUsersInRoleAsync("Teacher");
                model.AllTeachers = teachers.Select(t => new SelectListItem
                {
                    Value = t.Id,
                    Text = t.UserName
                }).ToList();

                ViewBag.CourseId = id;
                return View(model);
            }

            var course = await _context.Courses
                .Include(c => c.CourseTeachers)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return NotFound();

            course.Title = model.Title;
            course.Description = model.Description;
            course.StartDate = model.StartDate;
            course.EndDate = model.EndDate;

            _context.CourseTeachers.RemoveRange(course.CourseTeachers);

            course.CourseTeachers = model.SelectedTeacherIds.Select(tid => new CourseTeacher
            {
                CourseId = id,
                TeacherId = tid
            }).ToList();

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var course = await _context.Courses
                .Include(c => c.CourseTeachers)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return NotFound();

            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var course = await _context.Courses
                .Include(c => c.CourseTeachers)
                .Include(c => c.Enrollments)
                .Include(c => c.LectureSchedules)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
                return NotFound();

            _context.CourseTeachers.RemoveRange(course.CourseTeachers);
            _context.CourseEnrollments.RemoveRange(course.Enrollments);
            _context.LectureSchedules.RemoveRange(course.LectureSchedules);
            _context.Courses.Remove(course);

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> RequestEnrollment(string courseId)
        {
            var userId = _userManager.GetUserId(User);

            var exists = await _context.CourseEnrollments
                .AnyAsync(e => e.StudentId == userId && e.CourseId == courseId);

            if (!exists)
            {
                var enrollment = new CourseEnrollment
                {
                    StudentId = userId,
                    CourseId = courseId,
                    Status = "Pending"
                };
                _context.CourseEnrollments.Add(enrollment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = courseId });
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> RemoveEnrollment(string courseId)
        {
            var userId = _userManager.GetUserId(User);

            var enrollment = await _context.CourseEnrollments
                  .FirstOrDefaultAsync(e => e.CourseId == courseId && e.StudentId == userId);

            if (enrollment != null)
            {
                _context.CourseEnrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
                TempData["Message"] = "You have dropped the course.";
            }
            else
            {
                TempData["Message"] = "Enrollment not found.";
            }

            return RedirectToAction("Details", new { id = courseId });
        }


        //[HttpPost]
        //[Authorize(Roles = "Student")]
        //public async Task<IActionResult> RemoveEnrollment(string courseId)
        //{
        //    var userId = _userManager.GetUserId(User);

        //    var enrollment = await _context.CourseEnrollments
        //          .Include(e => e.CourseId)
        //          .FirstOrDefaultAsync(e => e.CourseId == courseId && e.StudentId == userId);

        //    if (enrollment != null)
        //    {
        //        _context.CourseEnrollments.Remove(enrollment);
        //        await _context.SaveChangesAsync();
        //        TempData["Message"] = "You have dropped the course.";
        //    }
        //    else
        //    {
        //        TempData["Message"] = "Enrollment not found.";
        //    }

        //    return RedirectToAction("Details", new { id = courseId });
        //}

        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> ReviewEnrollmentRequests(string courseId)
        {
            var course = await _context.Courses
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
                return NotFound();

            var pendingEnrollments = course.Enrollments
                .Where(e => e.Status == "Pending")
                .ToList();

            ViewBag.CourseTitle = course.Title;
            ViewBag.CourseId = course.Id;

            return View(pendingEnrollments);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> ApproveEnrollment(string id)
        {
            var enrollment = await _context.CourseEnrollments.FindAsync(id);
            if (enrollment == null)
                return NotFound();

            enrollment.Status = "Approved";
            await _context.SaveChangesAsync();

            return RedirectToAction("ReviewEnrollmentRequests", new { courseId = enrollment.CourseId });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> RejectEnrollment(string id)
        {
            var enrollment = await _context.CourseEnrollments.FindAsync(id);
            if (enrollment == null)
                return NotFound();

            enrollment.Status = "Rejected";
            await _context.SaveChangesAsync();

            return RedirectToAction("ReviewEnrollmentRequests", new { courseId = enrollment.CourseId });
        }

    }
}