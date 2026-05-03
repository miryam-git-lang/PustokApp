using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PustokApp.Data;
using PustokApp.Models;

namespace PustokApp.Areas.Manage.Controllers;
[Area("Manage")]
[Authorize (Roles = "Admin,SuperAdmin")]
public class UserController(UserManager<AppUser> userManager, AppDbContext context) : Controller
{
    public IActionResult Index()
    {
        var users = userManager.Users.ToList();
        return View(users);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(string id)
    {
        var user = context.Users.Find(id);

        if (user == null)
            return NotFound();
        context.Users.Remove(user);
        context.SaveChanges();

        return RedirectToAction("Index", "User", new { area = "Manage" });
    }
}