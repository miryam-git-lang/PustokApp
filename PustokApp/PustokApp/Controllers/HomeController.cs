using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokApp.Data;
using PustokApp.Models;
using PustokApp.ViewModels;

namespace PustokApp.Controllers;

public class HomeController(AppDbContext dbContext) : Controller
{
    public IActionResult Index()
    {
        HomeVm homeVm = new HomeVm
        {
            Sliders = dbContext.Sliders.ToList(),

            FeaturedBooks = dbContext.Books
                .Include(x => x.Author)
                .Where(x => x.IsFeatured).ToList(),
            
            NewBooks = dbContext.Books
                .Include(x => x.Author)
                .Where(x => x.IsNew).ToList(),
            
            DiscountedBooks = dbContext.Books
                .Include(x => x.Author)
                .Where(x => x.DiscountPrcent > 0).ToList(),
        };
        return View(homeVm);
    }
}