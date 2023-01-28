using LogLibrary;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using XManager.SystemPage;
//using XManager.Theme;

namespace XManager.Utils
{
    internal class ViewController
    {
        public Form MainWindow;

        internal UCPage1 Page1;
        internal UCPage2 Page2;
        internal UCPage3 Page3;
        internal UCPage4 Page4;
        internal UCPage5 Page5;
        internal UCPage6 Page6;

        private Page _iNextPage;
        internal Page iNextPage
        {
            get { return _iNextPage; }
            set
            {
                _iNextPage = value;
                NavigatePage(value);
                string message = String.Empty;
                message += value;
                message += String.Format("(0x{0:x2})", (Int32)value);
                message += " - Page navigation.";

                if (value != 0x00)
                {
                    message += String.Format(" (User: {0})", User.Guid);
                }
                Logger.Trace<ViewController>(message);
            }
        }

        public User User;
        public Logger Logger = Program.Logger;
        public Database Database;
        public Message Message;
        public ViewController()
        {
            User = new User();
            Database = new Database();
            Message = new Message();

            MainWindow = GetMainWindowInstance();
            InitializeComponentPage();
            Application.Run(MainWindow);
        }

        ~ViewController()
        {
            Logger.Trace<ViewController>(String.Format("{0} destructor called ", this.GetType().Name));
            if (MainWindow != null)
            {
                Logger.Trace<ViewController>(String.Format("Disposing {0}", MainWindow.GetType().Name));
                MainWindow.Dispose();
            }

            if (Page1 != null)
            {
                Logger.Trace<ViewController>(String.Format("Disposing {0}", Page1.GetType().Name));
                Page1.Dispose();
            }

            if (Page2 != null)
            {
                Logger.Trace<ViewController>(String.Format("Disposing {0}", Page2.GetType().Name));
                Page2.Dispose();
            }

            if (Page3 != null)
            {
                Logger.Trace<ViewController>(String.Format("Disposing {0}", Page3.GetType().Name));
                Page3.Dispose();
            }

            if (Page4 != null)
            {
                Logger.Trace<ViewController>(String.Format("Disposing {0}", Page4.GetType().Name));
                Page4.Dispose();
            }

            if (Page5 != null)
            {
                Logger.Trace<ViewController>(String.Format("Disposing {0}", Page5.GetType().Name));
                Page5.Dispose();
            }

            if (Page6 != null)
            {
                Logger.Trace<ViewController>(String.Format("Disposing {0}", Page6.GetType().Name));
                Page6.Dispose();
            }

            if (User != null)
            {
                Logger.Trace<ViewController>(String.Format("Null state {0}", User.GetType().Name));
                User = null;
            }

            if (Message != null)
            {
                Logger.Trace<ViewController>(String.Format("Null state {0}", Message.GetType().Name));
                Message = null;
            }

            if (Database != null)
            {
                Logger.Trace<ViewController>(String.Format("Disposing {0}", Database.GetType().Name));
                Database.Dispose();
            }

            if (Logger != null)
            {
                Logger.Trace<ViewController>(String.Format("Disposing {0}", Logger.GetType().Name));
                //Logger.Dispose();
                Logger = null;
            }
            GC.Collect();
        }

        private Form GetMainWindowInstance()
        {
            if (MainWindow != null)
            {
                return MainWindow;
            }

            Form mainWindow = new Form();
            mainWindow.Size = new Size(1280, 800);
            mainWindow.MinimumSize = new Size(mainWindow.Size.Width, mainWindow.Size.Height);
            mainWindow.Text = "Expense Manager";
            mainWindow.StartPosition = FormStartPosition.CenterScreen;
            mainWindow.AutoSizeMode = AutoSizeMode.GrowOnly;
            mainWindow.Icon = new Icon(Path.Combine(Config.AppDataPath, "Image", "001.ico"), new Size(36, 36));
            mainWindow.FormClosed += (_sender, _e) =>
            {
                Application.Exit();
                Logger.Trace<ViewController>("Window closed");
            };
            mainWindow.Load += (_sender, _e) =>
            {
                Logger.Trace<ViewController>("Window loading");
            };
            mainWindow.Shown += (_sender, _e) =>
            {
                Logger.Trace<ViewController>("Window loaded");
            };
            Logger.Trace<ViewController>("Finish window initialization");
            return mainWindow;
        }

        internal void InitializeComponentPage()
        {
            if (Page1 == null)
            {
                Page1 = new UCPage1(this);
                Page1.Dock = DockStyle.Fill;
                MainWindow.Controls.Add(Page1);
                Logger.Trace<ViewController>(String.Format("Finish {0}", Page1.GetType().Name));
            }

            if (Page2 == null)
            {
                Page2 = new UCPage2(this);
                Page2.Dock = DockStyle.Fill;
                MainWindow.Controls.Add(Page2);
                Logger.Trace<ViewController>(String.Format("Finish {0}", Page2.GetType().Name));
            }

            if (Page3 == null)
            {
                Page3 = new UCPage3(this);
                Page3.Dock = DockStyle.Fill;
                MainWindow.Controls.Add(Page3);
                Logger.Trace<ViewController>(String.Format("Finish {0}", Page3.GetType().Name));
            }

            if (Page4 == null)
            {
                Page4 = new UCPage4(this);
                Page4.Dock = DockStyle.Fill;
                MainWindow.Controls.Add(Page4);
                Logger.Trace<ViewController>(String.Format("Finish {0}", Page4.GetType().Name));
            }

            if (Page5 == null)
            {
                Page5 = new UCPage5(this);
                Page5.Dock = DockStyle.Fill;
                MainWindow.Controls.Add(Page5);
                Logger.Trace<ViewController>(String.Format("Finish {0}", Page5.GetType().Name));
            }

            if (Page6 == null)
            {
                Page6 = new UCPage6(this);
                Page6.Dock = DockStyle.Fill;
                MainWindow.Controls.Add(Page6);
                Logger.Trace<ViewController>(String.Format("Finish {0}", Page6.GetType().Name));
            }

            foreach (Page page in Enum.GetValues(typeof(Page)))
            {
                ClosePage(page);
            }
            iNextPage = Page.LOGIN;
            Logger.Trace<ViewController>(String.Format("Finish {0}", System.Reflection.MethodBase.GetCurrentMethod().Name));
        }

        private void NavigatePage(Page page)
        {
            switch (page)
            {
                case Page.LOGIN:
                    Page1.LoadLoginCookies();
                    Page1.Show();
                    break;

                case Page.INDEX:
                    Page2.Tab1InitializeDataBind();
                    Page2.Show();
                    break;

                case Page.UPDATE_USERNAME:
                    Page3.LabelUsername.Text = Database.GetUsernameFromGuid(User.Guid);
                    Page3.Show();
                    break;

                case Page.UPDATE_PASSWORD:
                    Page4.LabelUsername.Text = Database.GetUsernameFromGuid(User.Guid);
                    Page4.Show();
                    break;

                case Page.DEACTIVATE_ACCOUNT:
                    Page5.TextboxUsername.Text = Database.GetUsernameFromGuid(User.Guid);
                    Page5.Show();
                    break;

                case Page.CREATE_ACCOUNT:
                    Page6.Show();
                    break;

                default:
                    break;
            }
        }

        internal void ClosePage(Page page)
        {
            switch (page)
            {
                case Page.LOGIN:
                    Page1.Hide();
                    break;

                case Page.INDEX:
                    Page2.Hide();
                    break;

                case Page.UPDATE_USERNAME:
                    Page3.Hide();
                    break;

                case Page.UPDATE_PASSWORD:
                    Page4.Hide();
                    break;

                case Page.DEACTIVATE_ACCOUNT:
                    Page5.Hide();
                    break;

                case Page.CREATE_ACCOUNT:
                    Page6.Hide();
                    break;

                default:
                    break;
            }
        }

        public void ClearCurrentPageContent()
        {
            switch (iNextPage)
            {
                case Page.LOGIN:
                    Page1.ClearComponent();
                    break;

                case Page.INDEX:
                    Page2.ClearComponent();
                    break;

                case Page.UPDATE_USERNAME:
                    Page3.ClearComponent();
                    break;

                case Page.UPDATE_PASSWORD:
                    Page4.ClearComponent();
                    break;

                case Page.DEACTIVATE_ACCOUNT:
                    Page5.ClearComponent();
                    break;

                case Page.CREATE_ACCOUNT:
                    Page6.ClearComponent();
                    break;

                default:
                    break;
            }
        }

    }
}
