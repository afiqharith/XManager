using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary
{
    public class FileConverter
    {
        public static byte[] FileToByte(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            long sizeByte = fileInfo.Length;
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (BinaryReader binaryReader = new BinaryReader(fileStream))
            {
                return binaryReader.ReadBytes((int)sizeByte);
            }
        }

        public static bool ByteToFile(byte[] binary, string filePath)
        {
            using (FileStream fileStream = File.Create(filePath))
            {
                try
                {
                    fileStream.WriteAsync(binary, 0, binary.Length);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public static Image ByteToImage(byte[] binary)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.WriteAsync(binary, 0, binary.Length);
                return Image.FromStream(memoryStream);
            }
        }
    }
}
