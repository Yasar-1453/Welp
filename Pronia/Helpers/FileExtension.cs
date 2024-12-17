using Microsoft.AspNetCore.Routing.Constraints;
using Pronia.Models;

namespace Pronia.Helpers
{
    public static class FileExtension
    {
        public static string Upload(this IFormFile file, string rootPath, string folderName)
        {
            string filname = file.FileName;
            if (filname.Length > 64)
            {
                filname = filname.Substring(filname.Length - 64);
            }
            filname = Guid.NewGuid()+filname;
            string path = Path.Combine(rootPath, folderName, filname);

            using (FileStream stream = new FileStream(path + filname, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return filname;
        }

        public static bool DeleteFile(string rootPath, string folderName, string filename)
        {
            string path = Path.Combine(rootPath, folderName, filename);
            if(!File.Exists(path))
            {
                return false;
            }
            File.Delete(path);
            return true;
        }

    }
}
