using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokApp.Data;
using PustokApp.ViewModels;

namespace PustokApp.Controllers;

public class BooksController(AppDbContext  dbContext) : Controller
{
    public IActionResult Details(Guid id)
    {
        var book = dbContext.Books
            .Include(x => x.Author)
            .Include(x => x.BookImages)
            .Include(x => x.BookTags)
            .ThenInclude(x => x.Tag)
            .FirstOrDefault(b => b.Id == id);

        if (book == null)
        {
            return NotFound();
        }

        BookVm bookVm = new()
        {
            Book = book,
            RelatedBooks = dbContext.Books
                .Include(x => x.Author)
                .Include(x => x.BookImages)
                .Include(x => x.BookTags)
                .ThenInclude(x => x.Tag)
                .Where(b => b.AuthorId == book.AuthorId && b.Id != book.Id)
                .Take(4)
                .ToList()
        };
        return View(bookVm);


    }

    public IActionResult BookModal(Guid id)
    {
        var book = dbContext.Books
            .Include(x => x.Author)
            .Include(x => x.BookImages)
            .Include(x => x.BookTags)
            .ThenInclude(x => x.Tag)
            .FirstOrDefault(b => b.Id == id);
        if (book == null)
            return NotFound();
        return PartialView("_BookModal", book);
    }
}