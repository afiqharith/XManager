using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace LogLibrary
{
    public class Logger
    {
        private string _logDir = "Log";

        private string _traceDir = "Trace";
        private string _keyTraceDir = "KeyPress";
        private string _debugDir = "Debug";
        private string _informationDir = "Information";
        private string _warningDir = "Warning";
        private string _errorDir = "Error";
        private string _criticalDir = "Critical";

        private string _logPath;
        private string _tracePath;
        private string _keyTracePath;
        private string _debugPath;
        private string _informationPath;
        private string _warningPath;
        private string _errorPath;
        private string _criticalPath;

        public int InitTotalFilePendingCopy
        {
            get;
            set;
        }

        public int TotalFileCopied
        {
            get;
            set;
        }

        enum LOGGER_TYPE
        {
            TRACE = 0x00,
            KEY_TRACE = 0x01,
            DEBUG = 0x02,
            INFORMATION = 0x03,
            WARNING = 0x04,
            ERROR = 0x05,
            CRITICAL = 0x06,
            NONE = 0x07,
            NUM_OF_TYPE
        }

        public enum EXPORT_DAY
        {
            TWO_DAYS = 0x02,
            SEVEN_DAYS = 0x07,
            ONE_MONTH = 0x1F,
            NUM_OF_EXPORT_DAY
        }

        public Logger(string srcPath, double maxDirectorySize, bool bAutoDownSizeDirectory)
        {
            _logPath = Path.Combine(srcPath, _logDir);
            _tracePath = Path.Combine(_logDir, _traceDir);
            _keyTracePath = Path.Combine(_logDir, _keyTraceDir);
            _debugPath = Path.Combine(_logDir, _debugDir);
            _informationPath = Path.Combine(_logDir, _informationDir);
            _warningPath = Path.Combine(_logDir, _warningDir);
            _errorPath = Path.Combine(_logDir, _errorDir);
            _criticalPath = Path.Combine(_logDir, _criticalDir);

            string[] logChildPath = new string[]
            {
                _tracePath,
                _keyTracePath,
                _debugPath,
                _informationPath,
                _warningPath,
                _errorPath,
                _criticalPath,
            };

            for (int i = 0; i < logChildPath.Length; i++)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(logChildPath[i]);
                if (!directoryInfo.Exists)
                {
                    Directory.CreateDirectory(logChildPath[i]);
                }

                if (bAutoDownSizeDirectory)
                {
                    DownSizeDirectoryReachedMaxSize(logChildPath[i], maxDirectorySize);
                }
            }
        }

        private long GetDirectorySize(string directoryPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);

            long totalDirectorySizeByte = 0;
            totalDirectorySizeByte += directoryInfo.EnumerateFiles().Sum(file => file.Length);
            totalDirectorySizeByte += directoryInfo.EnumerateDirectories().Sum(directory => GetDirectorySize(directory.FullName));

            return totalDirectorySizeByte;
        }

        private bool DownSizeDirectoryReachedMaxSize(string directoryPath, double maxDirectorySize)
        {
            double directorySizeMB = GetDirectorySize(directoryPath) * Math.Pow(10, -6);

            if (Double.IsInfinity(directorySizeMB))
            {
                return true;
            }
            else
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                List<FileInfo> fileInfo = directoryInfo.EnumerateFiles().OrderByDescending(file => file.LastWriteTime).ToList();
                int counter = fileInfo.Count - 1;

                while (directorySizeMB > maxDirectorySize)
                {
                    fileInfo[counter].Delete();
                    counter--;

                    directorySizeMB = GetDirectorySize(directoryPath) * Math.Pow(10, -6);
                    fileInfo = directoryInfo.EnumerateFiles().OrderByDescending(file => file.LastWriteTime).ToList();
                    counter = fileInfo.Count - 1;

                    if (counter == 0)
                    {
                        break;
                    }
                }
                return true;
            }
        }

        public bool ExportLog(EXPORT_DAY dayType, string destinationPath)
        {
            bool bCopyCompleted = false;
            bool bCompressionCompleted = false;
            string tempPath = Path.Combine(DriveInfo.GetDrives()[0].ToString(), "TEMP");
            string tempLogPath = Path.Combine(tempPath, _logDir);

            Directory.CreateDirectory(Path.Combine(tempLogPath));

            string[] tempLogChildPath = new string[]
            {
                Path.Combine(tempLogPath, _traceDir),
                Path.Combine(tempLogPath, _keyTraceDir),
                Path.Combine(tempLogPath, _debugDir),
                Path.Combine(tempLogPath, _informationDir),
                Path.Combine(tempLogPath, _warningDir),
                Path.Combine(tempLogPath, _errorDir),
                Path.Combine(tempLogPath, _criticalDir)
            };

            for (int i = 0; i < tempLogChildPath.Length; i++)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(tempLogChildPath[i]);
                if (!directoryInfo.Exists)
                {
                    Directory.CreateDirectory(tempLogChildPath[i]);
                }
            }

            //Copy all logs
            string[] filePath = GetFileExportPath(dayType);
            bCopyCompleted = CopyLogFile(filePath, tempLogPath);

            //File compression
            if (bCopyCompleted)
            {
                try
                {
                    if (File.Exists(destinationPath))
                    {
                        File.Delete(destinationPath);
                    }
                    ZipFile.CreateFromDirectory(tempLogPath, destinationPath);
                    bCompressionCompleted = true;
                }
                catch (Exception)
                {
                    bCompressionCompleted = false;
                }
            }

            //Delete temp folder
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }

            return (bCopyCompleted && bCompressionCompleted);
        }

        private string[] GetFileExportPath(EXPORT_DAY totalDay)
        {
            List<string> filePathBuffer = new List<string>();
            DirectoryInfo parentDirectoryInfo = new DirectoryInfo(_logPath);
            DirectoryInfo[] childDirectoryInfo = parentDirectoryInfo.GetDirectories();

            for (int i = 0; i < childDirectoryInfo.Length; i++)
            {
                FileInfo[] childFileInfo = childDirectoryInfo[i].GetFiles();

                for (int j = 0; j < childFileInfo.Length; j++)
                {
                    if ((DateTime.UtcNow - childFileInfo[j].CreationTimeUtc) < new TimeSpan((int)totalDay, 0, 0, 0))
                    {
                        filePathBuffer.Add(childFileInfo[j].FullName);
                    }
                }
            }
            InitTotalFilePendingCopy = filePathBuffer.ToArray().Length;
            return filePathBuffer.ToArray();
        }

        private bool CopyLogFile(string[] sourceFilePath, string destinationFolderPath)
        {
            int fileCopied = 0;
            for (int i = 0; i < sourceFilePath.Length; i++)
            {
                string directoryName = String.Empty;
                if (sourceFilePath[i].Contains(_traceDir))
                {
                    directoryName = _traceDir;
                }
                else if (sourceFilePath[i].Contains(_keyTraceDir))
                {
                    directoryName = _keyTraceDir;
                }
                else if (sourceFilePath[i].Contains(_informationDir))
                {
                    directoryName = _informationDir;
                }
                else if (sourceFilePath[i].Contains(_warningDir))
                {
                    directoryName = _warningDir;
                }
                else if (sourceFilePath[i].Contains(_errorDir))
                {
                    directoryName = _errorDir;
                }
                else if (sourceFilePath[i].Contains(_criticalDir))
                {
                    directoryName = _criticalDir;
                }
                else if (sourceFilePath[i].Contains(_debugDir))
                {
                    directoryName = _debugDir;
                }

                try
                {
                    File.Copy(sourceFilePath[i], Path.Combine(destinationFolderPath, directoryName, Path.GetFileName(sourceFilePath[i])));
                    fileCopied++;
                    TotalFileCopied = fileCopied;
                }
                catch (Exception) { }
            }
            return (fileCopied >= sourceFilePath.Length);
        }

        private void GetPrefix(ref string prefix)
        {
            prefix = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff");
        }

        public void Trace<T>(string message)
        {
            string prefix = String.Empty;
            GetPrefix(ref prefix);

            string fullMessage = String.Format("{0} - {1} - {2}", prefix, typeof(T).Name, message);
            AppendText(fullMessage, LOGGER_TYPE.TRACE);
        }

        public void KeyTrace<T>(string message)
        {
            string prefix = String.Empty;
            GetPrefix(ref prefix);

            string fullMessage = String.Format("{0} - {1} - {2}", prefix, typeof(T).Name, message);
            AppendText(fullMessage, LOGGER_TYPE.KEY_TRACE);
        }

        public void Debug<T>(string message)
        {
            string prefix = String.Empty;
            GetPrefix(ref prefix);

            string fullMessage = String.Format("{0} - {1} - {2}", prefix, typeof(T).Name, message);
            AppendText(fullMessage, LOGGER_TYPE.DEBUG);
        }

        public void Information<T>(string message)
        {
            string prefix = String.Empty;
            GetPrefix(ref prefix);

            string fullMessage = String.Format("{0} - {1} - {2}", prefix, typeof(T).Name, message);
            AppendText(fullMessage, LOGGER_TYPE.INFORMATION);
        }

        public void Warning<T>(string message)
        {
            string prefix = String.Empty;
            GetPrefix(ref prefix);

            string fullMessage = String.Format("{0} - {1} - {2}", prefix, typeof(T).Name, message);
            AppendText(fullMessage, LOGGER_TYPE.WARNING);
        }

        public void Error<T>(string message)
        {
            string prefix = String.Empty;
            GetPrefix(ref prefix);

            string fullMessage = String.Format("{0} - {1} - {2}", prefix, typeof(T).Name, message);
            AppendText(fullMessage, LOGGER_TYPE.ERROR);
        }

        public void Critical<T>(string message)
        {
            string prefix = String.Empty;
            GetPrefix(ref prefix);

            string fullMessage = String.Format("{0} - {1} - {2}", prefix, typeof(T).Name, message);
            AppendText(fullMessage, LOGGER_TYPE.CRITICAL);
        }

        private async void AppendText(string message, LOGGER_TYPE loggerType)
        {
            string currentDateHour = DateTime.Now.ToString("yyyy-MM-dd.HH");
            string filePath;

            switch (loggerType)
            {
                case LOGGER_TYPE.TRACE:
                    filePath = Path.Combine(_tracePath, (currentDateHour + ".log"));
                    break;

                case LOGGER_TYPE.KEY_TRACE:
                    filePath = Path.Combine(_keyTracePath, (currentDateHour + ".log"));
                    break;

                case LOGGER_TYPE.DEBUG:
                    filePath = Path.Combine(_debugPath, (currentDateHour + ".log"));
                    break;

                case LOGGER_TYPE.INFORMATION:
                    filePath = Path.Combine(_informationPath, (currentDateHour + ".log"));
                    break;

                case LOGGER_TYPE.WARNING:
                    filePath = Path.Combine(_warningPath, (currentDateHour + ".log"));
                    break;

                case LOGGER_TYPE.ERROR:
                    filePath = Path.Combine(_errorPath, (currentDateHour + ".log"));
                    break;

                case LOGGER_TYPE.CRITICAL:
                    filePath = Path.Combine(_criticalPath, (currentDateHour + ".log"));
                    break;

                default:
                    filePath = Path.Combine(_logPath, (currentDateHour + ".log"));
                    break;
            }

            StreamWriter streamWriter;
            try
            {
                streamWriter = new StreamWriter(filePath, append: true);
            }
            catch (DirectoryNotFoundException)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                streamWriter = new StreamWriter(filePath, append: true);
            }

            using (streamWriter)
            {
                await streamWriter.WriteLineAsync(message);
            }
        }

    }
}
