using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speranza.Services.Interfaces;

namespace Speranza.Services
{
    public class GalleryService : IGalleryService
    {
        private const string GALLERY = "/Gallery";

        public List<string> GetFoldersWithImages()
        {
            if (Directory.Exists(GALLERY))
            {
                return Directory.GetDirectories(GALLERY).ToList();
            }
            return null;
        }

        public List<string> GetPhotosFromFolder(string folderPath)
        {
            return Directory.GetFiles(folderPath).ToList();
        }

        public string ConvertFolderPathToTag(string folder)
        {
            return Path.GetFileName(folder);
        }
    }
}
