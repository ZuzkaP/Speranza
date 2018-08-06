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
        private const string GALLERY = ROOT + @"\Gallery";
        private const string ROOT = @"D:\Inetpub\webs\12700885_web\subdoms\treningy";

        //private const string GALLERY = "C:\\Users\\zuzana.papalova\\Desktop\\SperanzaGit\\Speranza\\Speranza\\Gallery";

        public GalleryService(IDatabaseGateway db)
        {
            this.db = db;    
        }
        public List<string> GetFoldersWithImages()
        {
            //db.WriteToLog(Directory.GetCurrentDirectory(), new Email());
            //db.WriteToLog(System.Web.HttpContext.Current.Server.MapPath("~"), new Email());
            if (Directory.Exists(GALLERY))
            {
                return Directory.GetDirectories(GALLERY).ToList();
            }
            return null;
        }

        public List<string> GetPhotosFromFolder(string folderPath)
        {
            var output = new List<string>();
            var photos = Directory.GetFiles(folderPath).ToList();
            foreach (var photo in photos)
            {
                output.Add(photo.Replace(ROOT, "http://treningy.speranza.sk"));
               // output.Add(photo.Replace("C:\\Users\\zuzana.papalova\\Desktop\\SperanzaGit\\Speranza\\Speranza", "http://localhost:49871"));
            }
            return output;
            return photos;
        }

        public string ConvertFolderPathToTag(string folder)
        {
            return Path.GetFileName(folder);
        }
    }
}
