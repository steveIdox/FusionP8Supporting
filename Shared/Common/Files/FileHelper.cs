using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Files
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.IO;
    using System.Runtime.InteropServices.ComTypes;

    namespace idox.eim.fusionp8
    {
        public class FileHelper
        {
            public static void DeleteFile(string filePath)
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            public static string SaveToTempFile(Stream inputStream, string fileName)
            {
                // Create a unique temporary file path
                string tempFilePath = Path.Combine(Path.GetTempPath(), fileName);

                // Write the stream content to the temp file
                using (FileStream fileStream = File.Create(tempFilePath))
                {
                    inputStream.CopyTo(fileStream);
                }

                return tempFilePath; // Return the path for later use
            }
            public static string SaveToTempFile(byte[] bytes, string fileName)
            {
                // Create a unique temporary file path
                string tempFilePath = Path.Combine(Path.GetTempPath(), fileName);

                // Write the stream content to the temp file
                using (FileStream fileStream = File.Create(tempFilePath))
                {
                    fileStream.Write(bytes, 0, bytes.Length);
                }

                return tempFilePath; // Return the path for later use
            }
            public static bool SaveTo(byte[] bytes, string fileName)
            {
                // Write the stream content to the temp file
                using (FileStream fileStream = File.Create(fileName))
                {
                    fileStream.Write(bytes, 0, bytes.Length);
                }

                return true; // Return the path for later use
            }
            public static string ReadStream(Stream stream)
            {
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }
                using (var reader = new System.IO.StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            public static string AddToFileName(string inputpath, string addToName)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(inputpath);
                string newFileName = fi.Name + addToName + fi.Extension;
                return Path.Combine(fi.DirectoryName, newFileName);
            }
            public static string GetTempFilePathWithExtension(string extension)
            {
                string tempPath = Path.GetTempPath();
                string fileName = Path.ChangeExtension(Path.GetRandomFileName(), extension);
                string tempFilePath = Path.Combine(Path.GetTempPath(), fileName);
                return tempFilePath;
            }
            public static string GetTempFilePathFromInput(string filepath)
            {
                if (System.IO.File.Exists(filepath))
                {
                    string ext = new FileInfo(filepath).Extension;
                    string fileName = Path.ChangeExtension(Path.GetRandomFileName(), ext);
                    string tempFilePath = Path.Combine(Path.GetTempPath(), fileName);

                    return tempFilePath;
                }
                if(!String.IsNullOrEmpty(filepath))
                {
                    //  we have a filename but no path
                    string tempFilePath = Path.Combine(Path.GetTempPath(), filepath);
                    return tempFilePath;
                }
                return String.Empty;
            }
            public static bool IsImageFile(string fileName)
            {
                string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp" };
                string ext = Path.GetExtension(fileName)?.ToLowerInvariant();
                return Array.Exists(imageExtensions, e => e == ext);
            }
            public static byte[] bytesFromFile(string path)
            {
                if(File.Exists(path))
                    return File.ReadAllBytes(path);

                return null;
            }
        }
    }

}
