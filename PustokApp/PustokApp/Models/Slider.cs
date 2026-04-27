using System.ComponentModel.DataAnnotations;
using PustokApp.Models.Common;

namespace PustokApp.Models;

public class Slider : BaseEntity
{
    [Required]
    public string Title { get; set; }
    public string Description { get; set; }
    public string? ImageUrl { get; set; }
    public string ButtonText { get; set; }
    public string ButtonUrl { get; set; }
    public int Order { get; set; }
}