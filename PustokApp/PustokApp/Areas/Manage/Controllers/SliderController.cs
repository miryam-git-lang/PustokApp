using Microsoft.AspNetCore.Mvc;
using PustokApp.Data;
using PustokApp.Models;

namespace PustokApp.Areas.Manage.Controllers;

[Area("Manage")]
public class SliderController(AppDbContext context) : Controller
{
    public IActionResult Index()
    {
        var sliders = context.Sliders.ToList();
        return View(sliders);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Slider slider)
    {
        if (!ModelState.IsValid)
            return View(slider);

        if (slider.File != null)
        {
            string fileName = Guid.NewGuid() + Path.GetExtension(slider.File.FileName);

            string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/image/bg-images");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string path = Path.Combine(folder, fileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                slider.File.CopyTo(stream);
            }

            slider.ImageUrl = fileName;
        }

        context.Sliders.Add(slider);
        context.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Delete()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(Guid id)
    {
        var slider = context.Sliders.Find(id);
        if(slider == null) return NotFound();
        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/manage/assets/images");
        if(System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }
        context.Sliders.Remove(slider);
        context.SaveChanges();
        return RedirectToAction("Index");
    }
    
    [HttpGet]
    public IActionResult Edit(Guid id)
    {
        var slider = context.Sliders.Find(id);
        if (slider == null) return NotFound();
        return View(slider);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Slider slider)
    {
        if (!ModelState.IsValid)
            return View(slider);
        


        context.SaveChanges();
        return RedirectToAction("Index", "Slider", new { area = "Manage" });
    }
}