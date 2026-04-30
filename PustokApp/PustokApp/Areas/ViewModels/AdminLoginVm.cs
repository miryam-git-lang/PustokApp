using System.ComponentModel.DataAnnotations;

namespace PustokApp.Areas.ViewModels;

public class AdminLoginVm
{
    [Required]
    public string UserName { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}