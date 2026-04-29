namespace PustokApp.Extensions;

public static class FileManager
{
    public static string SaveFile(this IFormFile file,string rootPath)
    {
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var path = Path.Combine(rootPath, fileName);
        using (var stream = new FileStream(path, FileMode.Create))
        {
            file.CopyTo(stream);
        }
        return fileName;
    }
}