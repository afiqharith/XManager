using XManager.Utils;
using System.IO;
using ServiceLibrary;

namespace XManager
{
    internal class BinaryFileInfo
    {
        public string m_FileName { get; set; }
        public string m_FileType { get; set; }
        public byte[] m_Binary { get; set; }

        public BinaryFileInfo() { }

        public BinaryFileInfo(string filePath)
        {
            m_FileName = Path.GetFileName(filePath);
            m_FileType = Path.GetExtension(filePath);
            m_Binary = FileConverter.FileToByte(filePath);
        }
    }
}
