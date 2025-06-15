using FinalProject.Data;
using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FinalProject.Controllers
{
    [Authorize(Roles = "Student,Teacher")]
    public class StudentProfileController : Controller
    {
        private readonly FinalDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public StudentProfileController(FinalDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found or not authenticated.");
                return View();
            }
            //var profile = await _context.StudentProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
            var profile = _context.StudentProfiles
                .Include(sp => sp.User)
                .FirstOrDefault(sp => sp.UserId == user.Id);
            if (profile == null)
            {
                return RedirectToAction("Create");
            }

            return View(profile);
        }

        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found or not authenticated.");
                return View();
            }
            var profile = await _context.StudentProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (profile != null)
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentProfile profile, string Username, IFormFile ProfileImage)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);
                user.UserName = Username; // القيمة القادمة من الفورم
                await _userManager.UpdateAsync(user);

                //profile.ApplicationUserId = userId;

                // حفظ الصورة
                if (ProfileImage != null && ProfileImage.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await ProfileImage.CopyToAsync(ms);
                    //profile.ImageData = ms.ToArray();
                    //profile.ImageMimeType = ProfileImage.ContentType;
                }

                _context.StudentProfiles.Add(profile);
                await _context.SaveChangesAsync();

                // ✅ تحديث UserName في AspNetUsers
                user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.UserName = Username; // القيمة من النموذج
                    var result = await _userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", "Username update failed.");
                        return View(profile);
                    }
                }

                return RedirectToAction("Index", "Home");
            }

            return View(profile);
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(StudentProfile profile, IFormFile ProfileImage)
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null)
        //    {
        //        ModelState.AddModelError("", "User not found or not authenticated.");
        //        return View(profile);
        //    }
        //    profile.UserId = user.Id;

        //    if (ProfileImage != null && ProfileImage.Length > 0)
        //    {
        //        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfileImage.FileName);
        //        var filePath = Path.Combine(_environment.WebRootPath, "images", fileName);
        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await ProfileImage.CopyToAsync(stream);
        //        }
        //        profile.ProfileImagePath = "/images/" + fileName;
        //    }

        //    _context.StudentProfiles.Add(profile);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("Index");
        //}

        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found or not authenticated.");
                return View();
            }
            var profile = await _context.StudentProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (profile == null)
            {
                return NotFound();
            }

            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentProfile profile, IFormFile ProfileImage)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found or not authenticated.");
                return View(profile);
            }
            if (user == null || profile.UserId != user.Id)
            {
                return Unauthorized();
            }

            var existingProfile = await _context.StudentProfiles.FindAsync(user.Id);
            if (existingProfile == null)
            {
                return NotFound();
            }

            existingProfile.FullName = profile.FullName;
            existingProfile.DateOfBirth = profile.DateOfBirth;
            existingProfile.PhoneNumber = profile.PhoneNumber;

            if (ProfileImage != null && ProfileImage.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfileImage.FileName);
                var filePath = Path.Combine(_environment.WebRootPath, "images", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfileImage.CopyToAsync(stream);
                }
                existingProfile.ProfileImagePath = "/images/" + fileName;
            }

            _context.Update(existingProfile);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found or not authenticated.");
                return View();
            }
            var profile = await _context.StudentProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (profile == null)
            {
                return NotFound();
            }

            _context.StudentProfiles.Remove(profile);
            await _context.SaveChangesAsync();

            return RedirectToAction("Create", "StudentProfile", new { area = "Identity" });
        }
    }
}
