using FinalProject.Data;
using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly FinalDbContext finalDbContext;

    public UsersController(UserManager<ApplicationUser> userManager, FinalDbContext context)
    {
        _userManager = userManager;
        finalDbContext = context;
    }

    [Authorize]
    public async Task<IActionResult> Index(string searchTerm = "")
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var currentRoles = await _userManager.GetRolesAsync(currentUser);
        var currentRole = currentRoles.FirstOrDefault();

        var users = _userManager.Users.ToList();
        var userViewModels = new List<UserViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            var userDetails = finalDbContext.UserDetails.FirstOrDefault(u => u.UserId == user.Id);

            if (currentRole == "Admin" ||
                (currentRole == "Teacher" && (role == "Student" || role == "Teacher")) ||
                (currentRole == "Student" && role == "Student"))
            {
                if (string.IsNullOrEmpty(searchTerm) ||
                    user.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    user.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                {
                    userViewModels.Add(new UserViewModel
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        Role = role,
                        EmailConfirmed = user.EmailConfirmed,
                        PhoneNumber = user.PhoneNumber,
                        CanEdit = currentRole == "Admin" || currentRole == "Teacher",
                        CanDelete = currentRole == "Admin",
                        CanApprove = currentRole == "Admin" || currentRole == "Teacher",
                    });
                }
            }
        }

        return View(userViewModels);
    }

    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault();

        var model = new UserViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = role
        };

        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Edit(UserViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null) return NotFound();

        user.UserName = model.UserName;
        user.Email = model.Email;
        user.PhoneNumber = model.PhoneNumber;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        if (User.IsInRole("Admin") && !string.IsNullOrEmpty(model.Role))
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, model.Role);
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var deleteResult = await _userManager.DeleteAsync(user);

        if (!deleteResult.Succeeded)
        {
            TempData["Error"] = "Failed to delete user.";
            return RedirectToAction(nameof(Index));
        }

        return RedirectToAction(nameof(Index));
    }
}

//using FinalProject.Data;
//using FinalProject.Models;
//using FinalProject.Models.ViewModels;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;

//[Authorize]
//public class UsersController : Controller
//{
//    private readonly UserManager<IdentityUser> _userManager;
//    private readonly FinalDbContext finalDbContext;

//    public UsersController(UserManager<IdentityUser> userManager, FinalDbContext context)
//    {
//        _userManager = userManager;
//        finalDbContext = context;
//    }
//    public UsersController(UserManager<IdentityUser> userManager)
//    {
//        _userManager = userManager;
//    }

//    public async Task<IActionResult> Index(string searchTerm = "")
//    {
//        var currentUser = await _userManager.GetUserAsync(User);
//        var currentRoles = await _userManager.GetRolesAsync(currentUser);
//        var currentRole = currentRoles.FirstOrDefault();

//        var users = _userManager.Users.ToList();
//        var userViewModels = new List<UserViewModel>();

//        foreach (var user in users)
//        {
//            var roles = await _userManager.GetRolesAsync(user);
//            var role = roles.FirstOrDefault();

//            // استبدل هذا بالقراءة الحقيقية من قاعدة البيانات
//            var isApproved = true;

//            if (currentRole == "Admin" ||
//                (currentRole == "Teacher" && (role == "Student" || role == "Teacher")) ||
//                (currentRole == "Student" && role == "Student"))
//            {
//                if (string.IsNullOrEmpty(searchTerm) ||
//                    user.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
//                    user.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
//                {
//                    userViewModels.Add(new UserViewModel
//                    {
//                        Id = user.Id,
//                        UserName = user.UserName,
//                        Email = user.Email,
//                        Role = role,
//                        EmailConfirmed = user.EmailConfirmed,
//                        PhoneNumber = user.PhoneNumber,
//                        CanEdit = currentRole == "Admin" || currentRole == "Teacher",
//                        CanDelete = currentRole == "Admin",
//                        CanApprove = currentRole == "Admin" || currentRole == "Teacher",
//                        IsApproved = isApproved
//                    });
//                }
//            }
//        }

//        return View(userViewModels);
//    }

//    [Authorize(Roles = "Admin,Teacher")]
//    public async Task<IActionResult> Edit(Guid id)
//    {
//        var user = await _userManager.FindByIdAsync(id.ToString());
//        if (user == null) return NotFound();

//        var roles = await _userManager.GetRolesAsync(user);
//        var role = roles.FirstOrDefault();

//        var model = new UserViewModel
//        {
//            Id = user.Id,
//            UserName = user.UserName,
//            Email = user.Email,
//            PhoneNumber = user.PhoneNumber,
//            Role = role
//        };

//        return View(model);
//    }

//    [HttpPost]
//    [Authorize(Roles = "Admin,Teacher")]
//    public async Task<IActionResult> Edit(UserViewModel model)
//    {
//        var user = await _userManager.FindByIdAsync(model.Id);
//        if (user == null) return NotFound();

//        user.UserName = model.UserName;
//        user.Email = model.Email;
//        user.PhoneNumber = model.PhoneNumber;

//        await _userManager.UpdateAsync(user);

//        if (User.IsInRole("Admin") && !string.IsNullOrEmpty(model.Role))
//        {
//            var currentRoles = await _userManager.GetRolesAsync(user);
//            await _userManager.RemoveFromRolesAsync(user, currentRoles);
//            await _userManager.AddToRoleAsync(user, model.Role);
//        }

//        return RedirectToAction(nameof(Index));
//    }

//    [Authorize(Roles = "Admin")]
//    public async Task<IActionResult> Delete(string id)
//    {
//        var user = await _userManager.FindByIdAsync(id);
//        if (user == null) return NotFound();

//        await _userManager.DeleteAsync(user);
//return RedirectToAction(nameof(Index));
//    }

//    [Authorize(Roles = "Admin,Teacher")]
//    public async Task<IActionResult> Approve(string id)
//    {
//        // نفّذ هنا منطق الموافقة على المستخدم (تحديث الجدول أو الخاصية المناسبة)
//        TempData["Message"] = "User approved successfully.";
//        return RedirectToAction("Index");
//    }

//    [Authorize(Roles = "Admin,Teacher")]
//    public async Task<IActionResult> Reject(string id)
//    {
//        // نفّذ هنا منطق الرفض (حذف أو تعطيل)
//        TempData["Message"] = "User rejected.";
//        return RedirectToAction("Index");
//    }
//}
