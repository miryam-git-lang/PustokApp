using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PustokApp.Models;

namespace PustokApp.Areas.Manage.Controllers;
[Area("Manage")]
[Authorize (Roles = "Admin,SuperAdmin")]
public class UserController(UserManager<AppUser> userManager) : Controller
{
    public IActionResult Index()
    {
        var users = userManager.Users.ToList();
        return View(users);
    }
}