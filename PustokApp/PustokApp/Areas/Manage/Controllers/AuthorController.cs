using Microsoft.AspNetCore.Mvc;
using PustokApp.Data;
using PustokApp.Models;

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
            .FirstOrDefault(a => a.Id == id);

        if (author == null)
            return NotFound();

        return PartialView("_DetailsModalPartial", author);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Author author)
    {
        if (!ModelState.IsValid)
            return View(author);
        var existAuthor = context.Authors.FirstOrDefault(a => a.FullName.ToLower() == author.FullName.ToLower());
        if (existAuthor != null)
        {
            ModelState.AddModelError("FullName", "Author with the same name already exists");
            return View(author);
        }

        context.Authors.Add(author);
        context.SaveChanges();

        return RedirectToAction("Index", "Author", new { area = "Manage" });
    }

    [HttpGet]
    public IActionResult Edit(Guid id)
    {
        var author = context.Authors.Find(id);
        if (author == null) return NotFound();
        return View(author);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Author author)
    {
        if (!ModelState.IsValid)
            return View(author);
        var existingAuthorDb = context.Authors.Find(author.Id);
        if (existingAuthorDb == null) return NotFound();
        existingAuthorDb.FullName = author.FullName;
        var existAuthor = context.Authors
            .FirstOrDefault(a => a.FullName.ToLower() == author.FullName.ToLower() && a.Id != author.Id);
        if (existAuthor != null)
        {
            ModelState.AddModelError("FullName", "Author with the same name already exists");
            return View(author);
        }


        context.SaveChanges();
        return RedirectToAction("Index", "Author", new { area = "Manage" });
    }
}