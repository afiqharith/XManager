using LogLibrary;
using XManager.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace XManager
{
    class Program
    {
        public static readonly Logger Logger = new Logger(Path.GetDirectoryName(Application.ExecutablePath), Config.MaxLogDirectorySize, Config.bAutoCleanUpMaxSizeDirectory);
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Logger.Trace<Program>("Startup program");
            ViewController viewController = new ViewController();
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Logger.Error<Program>(e.Exception.Message);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Error<Program>((e.ExceptionObject as Exception).Message);
        }
    }
}
