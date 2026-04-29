using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokApp.Data;

namespace PustokApp.Areas.Manage.Controllers;

[Area("Manage")]
public class AuthorController(AppDbContext context) : Controller
{
    public IActionResult Index()
    {
        return View(context.Authors.ToList());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(Guid id)
    {
        var author = context.Authors.Find(id);

        if (author == null)
            return NotFound();
        context.Authors.Remove(author);
        context.SaveChanges();

        return RedirectToAction("Index", "Author", new { area = "Manage" });
    }

    [HttpGet]
    public IActionResult Details(Guid id)
    {
        var author = context.Authors
            .Include(a => a.Books)
            .FirstOrDefault(a => a.Id == id);

        if (author == null)
            return NotFound();

        return PartialView("_DetailsModalPartial", author);
    }
}