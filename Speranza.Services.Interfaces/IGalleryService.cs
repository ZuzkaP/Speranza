using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Services.Interfaces
{
    public interface IGalleryService
    {
      List<string> GetFoldersWithImages();
      List<string> GetPhotosFromFolder(string folderPath);
        string ConvertFolderPathToTag(string foldera);
    }
}
