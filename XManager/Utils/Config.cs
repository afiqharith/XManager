using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace XManager.Utils
{
    class Config
    {
        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["local"].ConnectionString;
            }
        }

        public static SqlConnection SqlConnection
        {
            get
            {
                return new SqlConnection(ConnectionString);
            }
        }

        public static bool bAutoCleanUpMaxSizeDirectory
        {
            get
            {
                if (ConfigurationManager.AppSettings["Log.EnableAutoDownSizeDirectory"].ToLower().Contains("y") ||
                    ConfigurationManager.AppSettings["Log.EnableAutoDownSizeDirectory"].ToLower().Equals("yes") ||
                    ConfigurationManager.AppSettings["Log.EnableAutoDownSizeDirectory"].ToLower().Equals("true") ||
                    ConfigurationManager.AppSettings["Log.EnableAutoDownSizeDirectory"].ToLower().Equals("1") ||
                    ConfigurationManager.AppSettings["Log.EnableAutoDownSizeDirectory"].ToLower().Equals(1))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static double MaxLogDirectorySize
        {
            get
            {
                try
                {
                    return Convert.ToDouble(ConfigurationManager.AppSettings["Log.MaxDirectorySize"]);
                }
                catch (Exception)
                {
                    return 10.0;
                }
            }
        }

        public static string NewGuid
        {
            get
            {
                return Convert.ToString(Guid.NewGuid());
            }
        }

        public static string AppDataPath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "AppData");
            }
        }

        public static void IniWriteBool(string keyName, bool value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[keyName].Value = value ? "true" : "false";
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);

        }
    }
}
