using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PustokApp.Models.Common;

namespace PustokApp.Models;

public class Book :BaseEntity
{
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }
    [Required]
    [Column (TypeName = "decimal(18,2)")]
    public int DiscountPrcent { get; set; }
    public decimal Price { get; set; }
    public string Code { get; set; }
    public bool InStock { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsNew { get; set; }
    public string? MainImageUrl { get; set; }
    public string? HoverImgeUrl { get; set; }
    public Guid AuthorId { get; set; }
    public Author? Author { get; set; }
    public List<BookTag> BookTags { get; set; }
    public List<BookImage> BookImages { get; set; }
}