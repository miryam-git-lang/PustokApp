using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PustokApp.Models;
using PustokApp.ViewModels.Users;

namespace PustokApp.Controllers;

public class AccountController(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    RoleManager<IdentityRole> roleManager
) : Controller
{
    // GET
    [HttpGet]
    public IActionResult Register()
    {
        var context = HttpContext;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(UserRegisterVm registerVm)
    {
        if (!ModelState.IsValid)
            return View(registerVm);
        var user = await userManager.FindByNameAsync(registerVm.UserName);
        if (user != null)
        {
            ModelState.AddModelError("", "User with this username already exists");
            return View(registerVm);
        }

        user = new AppUser()
        {
            UserName = registerVm.UserName,
            Email = registerVm.Email,
            FullName = registerVm.FullName,
        };
        var result = await userManager.CreateAsync(user, registerVm.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(registerVm);
        }

        await userManager.AddToRoleAsync(user, "User");
        // end email to user
        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(UserLoginVm loginVm, string ReturnUrl = null)
    {
        var user = await userManager.FindByNameAsync(loginVm.UserNameOrEmail);
        if (user == null)
        {
            user = await userManager.FindByEmailAsync(loginVm.UserNameOrEmail);
            if (user == null)
            {
                ModelState.AddModelError("", "User with this username or email not found");
                return View(loginVm);
            }
            
        }
        if (await userManager.IsInRoleAsync(user, "Admin"))
        {
            ModelState.AddModelError("","Admins cannot login here");
            return View(loginVm);
        }

        var result = await signInManager.PasswordSignInAsync(user, loginVm.Password,loginVm.RememberMe, true);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Username or password is incorrect");
            return View(loginVm);
        }
        return ReturnUrl != null ? Redirect(ReturnUrl) : RedirectToAction("Index", "Home");
    }
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
    
}
