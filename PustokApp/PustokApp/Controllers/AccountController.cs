using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PustokApp.Models;
using PustokApp.Services;
using PustokApp.ViewModels.Users;

namespace PustokApp.Controllers;

public class AccountController(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    RoleManager<IdentityRole> roleManager,
    IEmailService emailService
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

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var link = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);

        await emailService.SendEmailAsync(user.Email, "Confirm your email", $"<h2>Confirm your account</h2><a href='{link}'>Click here to confirm</a>" );

        return View("VerifyEmail", user.Email);
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

        if (!await userManager.IsEmailConfirmedAsync(user))
        {
            ModelState.AddModelError("", "Please confirm your email first");
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
    
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (userId == null || token == null)
            return BadRequest();

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        var result = await userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
            return BadRequest();

        return View();
    }
    
}
