using System.ComponentModel.DataAnnotations;
using PustokApp.Models.Common;

namespace PustokApp.Models;

public class Author : BaseEntity
{
    [Required(ErrorMessage = "FullName is required")]
    [MaxLength(20, ErrorMessage = "FullName must be at most 20 characters")]
    public string FullName { get; set; }
    public List<Book>? Books { get; set; }
}