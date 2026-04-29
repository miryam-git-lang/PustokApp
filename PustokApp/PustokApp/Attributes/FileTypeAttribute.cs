using System.ComponentModel.DataAnnotations;

namespace PustokApp.Attributes;

public class FileTypeAttribute : ValidationAttribute
{
    private readonly string[] _allowedTypes;

    public FileTypeAttribute(params string[] allowedTypes)
    {
        _allowedTypes = allowedTypes;
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
        if (files != null)
        {
            foreach (var f in files)
            {
                if (!_allowedTypes.Contains(f.ContentType))
                {
                    return new ValidationResult($"Only the following file types are allowed: {string.Join(", ", _allowedTypes)}");
                }
            }
        }

        return ValidationResult.Success;
    }
}