using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Context;

namespace DemoExcercise.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var users = _userManager.Users.ToList();
        var userRoles = new Dictionary<string, string>();
        foreach (var user in users)
        {
            var roles = _userManager.GetRolesAsync(user).Result;
            userRoles[user.Id] = string.Join(", ", roles);
        }

        ViewBag.UserRoles = userRoles;
        return View(users);
    }

    public IActionResult Create()
    {
        var roles = _roleManager.Roles.ToList();
        ViewBag.UserRoles = roles;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(IdentityUser user, string password, string role)
    {
        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            if (!await _roleManager.RoleExistsAsync(role)) await _roleManager.CreateAsync(new IdentityRole(role));
            await _userManager.AddToRoleAsync(user, role);
            return RedirectToAction("Index");
        }

        return View(user);
    }

    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        var roles = _userManager.GetRolesAsync(user).Result;
        ViewBag.Role = string.Join(", ", roles);

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, IdentityUser user, string role, string newPassword)
    {
        var existUser = await _userManager.FindByIdAsync(user.Id);
        if (existUser != null)
        {
            // Update user details
            existUser.UserName = user.UserName;
            existUser.Email = user.Email;

            // Generate and reset password
            var token = await _userManager.GeneratePasswordResetTokenAsync(existUser);
            var result = await _userManager.ResetPasswordAsync(existUser, token, newPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
                return View(user);
            }

            // Update user in database
            var updateResult = await _userManager.UpdateAsync(existUser);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors) ModelState.AddModelError("", error.Description);
                return View(user);
            }

            // Assign Role if it doesn't exist
            if (!await _roleManager.RoleExistsAsync(role)) await _roleManager.CreateAsync(new IdentityRole(role));

            // Add user to role
            await _userManager.AddToRoleAsync(existUser, role);

            return RedirectToAction("Index");
        }

        return View(existUser);
    }

    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null) await _userManager.DeleteAsync(user);
        return RedirectToAction("Index");
    }
}