using System.ComponentModel.DataAnnotations;

namespace PustokApp.Attributes;

public class FileLenghtAttribute : ValidationAttribute
{
    public int Length { get; set; }
    public FileLenghtAttribute(int length)
    {
        Length = length;
    }
    
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var files = new List<IFormFile>();

        if (value is IFormFile file)
        {
            files.Add((file));
        }
        else if (value is IEnumerable<IFormFile> fileCollection)
        {
            files.AddRange(fileCollection);
        }

        foreach (var f in files)
        {
            if (f.Length > Length * 1024 * 1024)
            {
                return new ValidationResult($"File size should not exceed {Length} MB.");
            }
            
        }

        return ValidationResult.Success;
    }
    
}