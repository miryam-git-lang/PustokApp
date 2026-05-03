using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PustokApp.Areas.ViewModels;
using PustokApp.Models;
using PustokApp.Services;

namespace PustokApp.Areas.Manage.Controllers;
[Area("Manage")]
[Authorize]
public class AccountController(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    RoleManager<IdentityRole> roleManager
) : Controller
{
    // GET
    [AllowAnonymous]
    public async Task<IActionResult> CreateAdmin()
    {
        var existing = await userManager.FindByNameAsync("admin");
        if (existing != null)
            return Content("Admin already exists");
        
        AppUser admin = new AppUser
        {
            UserName = "admin",
            FullName = "admin adminov",
            Email = "admin@gmail.com",
            EmailConfirmed = true
        };
    
        var result = await userManager.CreateAsync(admin, "Admin123!");
    
        if (!result.Succeeded)
            return Json(result.Errors);
    
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));
    
        await userManager.AddToRoleAsync(admin, "Admin");
    
        return Content("Admin created");
    }
    
    public async Task<IActionResult> CreateRole()
    {
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        

        if (!await roleManager.RoleExistsAsync("User"))
            await roleManager.CreateAsync(new IdentityRole("User"));

        return Content("Roles created");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(AdminLoginVm loginVm)
    {
        if (!ModelState.IsValid)
            return View(loginVm);

        var admin = await userManager.FindByNameAsync(loginVm.UserName);

        if (admin == null)
        {
            ModelState.AddModelError("", "Username or password is incorrect");
            return View(loginVm);
        }
        if (await userManager.IsInRoleAsync(admin, "User"))
        {
            ModelState.AddModelError("","Users cannot login here");
            return View(loginVm);
        }

        var result = await signInManager.PasswordSignInAsync(
            admin,
            loginVm.Password,
            false,
            false
        );
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Username or password is incorrect");
            return View(loginVm);
        }
        return RedirectToAction("Index", "Dashboard", new { area = "Manage" });
    }
    
}