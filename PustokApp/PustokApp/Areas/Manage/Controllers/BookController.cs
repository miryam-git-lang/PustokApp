using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokApp.Data;
using PustokApp.Extensions;
using PustokApp.Models;

namespace PustokApp.Areas.Manage.Controllers;

[Area("Manage")]
public class BookController(AppDbContext context) : Controller
{
    // GET
    public IActionResult Index()
    {
        var books = context.Books
            .Include(b => b.Author)
            .Include(b => b.BookTags)
            .ToList();
        return View(books);
    }

    public IActionResult Create()
    {
        ViewBag.Authors = context.Authors.ToList();
        ViewBag.Tags = context.Tags.ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Book book)
    {
        ViewBag.Authors = context.Authors.ToList();
        ViewBag.Tags = context.Tags.ToList();
        if (!ModelState.IsValid)
            return View(book);
        var author = context.Authors.FirstOrDefault(a => a.Id == book.AuthorId);
        if (author == null)
        {
            ModelState.AddModelError("AuthorId", "Author not found");
            return View(book);
        }

        if (book.TagIds != null)
        {
            foreach (var tagId in book.TagIds)
            {
                var tag = context.Tags.FirstOrDefault(t => t.Id == tagId);
                if (tag == null)
                {
                    ModelState.AddModelError("TagId", "Tag not found");
                    return View(book);
                }
            }
        }

        var mainPhoto = book.MainPhoto;
        var hoverPhoto = book.HoverPhoto;
        var photos = book.Photos;

        if (mainPhoto == null)
        {
            ModelState.AddModelError("MainPhoto", "Main photo is required");
            return View(book);
        }

        if (hoverPhoto == null)
        {
            ModelState.AddModelError("HoverPhoto", "Hover photo is required");
            return View(book);
        }

        book.MainImageUrl = mainPhoto.SaveFile("wwwroot/assets/image/bg-images");
        book.HoverImgeUrl = hoverPhoto.SaveFile("wwwroot/assets/image/bg-images");
        if (photos != null)
        {
            foreach (var photo in photos)
            {
                var imageUrl = photo.SaveFile("wwwroot/assets/image/bg-images");
                book.BookImages.Add(new BookImage { ImageUrl = imageUrl });
            }
        }

        if (book.TagIds != null)
        {
            foreach (var tagId in book.TagIds)
            {
                book.BookTags.Add(new BookTag { TagId = tagId });
            }
        }

        context.Books.Add(book);
        context.SaveChanges();
        return RedirectToAction("Index");
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(Guid id)
    {
        var book = context.Books.Find(id);

        if (book == null)
            return NotFound();
        context.Books.Remove(book);
        context.SaveChanges();

        return RedirectToAction("Index", "Book", new { area = "Manage" });
    }
    
    [HttpGet]
    public IActionResult Details(Guid id)
    {
        var book = context.Books
            .FirstOrDefault(a => a.Id == id);

        if (book == null)
            return NotFound();

        return PartialView("_DetailsModalPartial", book);
    }

    public IActionResult Edit(Guid id)
    {
        var book = context.Books
            .Include(b => b.BookTags)
            .Include(b => b.BookImages)
            .FirstOrDefault(b => b.Id == id);
        if (book == null)
            return NotFound();
        ViewBag.Authors = context.Authors.ToList();
        ViewBag.Tags = context.Tags.ToList();
        book.TagIds = book.BookTags.Select(bt => bt.TagId).ToList();
        return View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Book book)
    {
        var existingBook = context.Books
            .Include(b => b.BookTags)
            .Include(b => b.BookImages)
            .FirstOrDefault(b => b.Id == book.Id);
        if (existingBook == null)
            return NotFound();

        ViewBag.Authors = context.Authors.ToList();
        ViewBag.Tags = context.Tags.ToList();
        if (!ModelState.IsValid)
            return View(book);
        var author = context.Authors.FirstOrDefault(a => a.Id == book.AuthorId);
        if (author == null)
        {
            ModelState.AddModelError("AuthorId", "Author not found");
            return View(book);
        }

        if (book.TagIds != null)
        {
            foreach (var tagId in book.TagIds)
            {
                var tag = context.Tags.FirstOrDefault(t => t.Id == tagId);
                if (tag == null)
                {
                    ModelState.AddModelError("TagId", "Tag not found");
                    return View(book);
                }
            }
        }

        existingBook.Name = book.Name;
        existingBook.Description = book.Description;
        existingBook.DiscountPrcent = book.DiscountPrcent;
        existingBook.Price = book.Price;
        existingBook.Code = book.Code;
        existingBook.InStock = book.InStock;
        existingBook.IsFeatured = book.IsFeatured;
        existingBook.IsNew = book.IsNew;
        existingBook.AuthorId = book.AuthorId;

        if (book.TagIds != null)
        {
            existingBook.BookTags.Clear();
        }
        else
        {
            var newBookTags = book.TagIds;
            var removeTags = existingBook.BookTags
                .Where(bt => !newBookTags.Contains(bt.TagId))
                .ToList();
            foreach (var tagId in removeTags)
            {
                existingBook.BookTags.Remove(tagId);
            }

            var currentTagIds = existingBook.BookTags.Select(bt => bt.TagId).ToList();
            foreach (var tagId in newBookTags)
            {
                if (!currentTagIds.Contains(tagId))
                {
                    existingBook.BookTags.Add(new BookTag { TagId = tagId });
                }
            }

            var mainPhoto = book.MainPhoto;
            var hoverPhoto = book.HoverPhoto;
            var photos = book.Photos;
            if (mainPhoto != null)
            {
                existingBook.MainImageUrl = mainPhoto.SaveFile("wwwroot/assets/image/bg-images");
            }

            if (hoverPhoto != null)
            {
                existingBook.HoverImgeUrl = hoverPhoto.SaveFile("wwwroot/assets/image/bg-images");
            }

            if (photos != null)
            {
                foreach (var photo in photos)
                {
                    var imageUrl = photo.SaveFile("wwwroot/assets/image/bg-images");
                    existingBook.BookImages?.Add(new BookImage { ImageUrl = imageUrl });
                }
            }
        }
        
        context.SaveChanges();
        return RedirectToAction("Index", "Book", new { area = "Manage" });
    }

    [HttpGet]
    public IActionResult DeleteImage(Guid id)
    {
        var bookImage = context.BookImages.FirstOrDefault(bi => bi.Id == id);
        if (bookImage == null)
            return NotFound();
        
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/manage/assets/images", bookImage.ImageUrl);
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }
        context.BookImages.Remove(bookImage);
        context.SaveChanges();
        return RedirectToAction("Edit", new { id = bookImage.BookId });
    }
}