using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speranza.Database;
using Speranza.Services.Interfaces;
using Speranza.Smtp.Interfaces;

namespace Speranza.Services
{
    public class GalleryService : IGalleryService
    {
        private IDatabaseGateway db;
        private const string GALLERY = "/Gallery";

        public GalleryService(IDatabaseGateway db)
        {
            this.db = db;    
        }
        public List<string> GetFoldersWithImages()
        {

            db.WriteToLog(Directory.GetCurrentDirectory(), new Email());
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
