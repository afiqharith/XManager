using LogLibrary;
using ServiceLibrary;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using XManager.SystemMessage;
using XManager.SystemPage;
using XManager.Utils;

namespace XManager
{
    internal partial class UCPage1 : UserControl
    {
        private User _user;
        private Logger _logger = Program.Logger;
        private Database _database;
        private Utils.Message _message;
        private ViewController _controller;

        public UCPage1(ViewController viewController)
        {
            _controller = viewController;
            _user = viewController.User;
            _database = viewController.Database;
            _message = viewController.Message;
            InitializeComponent();
            LoadLoginCookies();
        }

        private void ButtonLogin_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = Config.SqlConnection)
            using (SqlCommand cmd = new SqlCommand("SELECT guid, username, password, access_level FROM administration WHERE username = @username", con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add(new SqlParameter("@username", TextboxUsername.Text));

                con.Open();
                da.Fill(dt);
                con.Close();
            }

            switch (dt.Rows.Count > 0)
            {
                case true:
                    (string salt, string hashedPassword) = PasswordEncryption.SplitHashedPassword(Convert.ToString(dt.Rows[0]["password"]));

                    if (PasswordEncryption.ValidateHashedPassword(salt, TextboxPassword.Text, hashedPassword))
                    {
                        _user.Guid = Convert.ToString(dt.Rows[0]["guid"]);
                        _user.Username = Convert.ToString(dt.Rows[0]["username"]);
                        _user.AccessLevel = Convert.ToString(dt.Rows[0]["access_level"]);

                        _database.LogRecentLoginTime(_user.Guid);
                        StoreLoginCookies();

                        _controller.iNextPage = Page.INDEX;
                        //_controller.GlobalNavigatePage(ViewController.Page.INDEX);
                        _controller.ClosePage(Page.LOGIN);
                    }
                    else
                    {
                        _message.GetMessageBox(ID.ERROR_USERNAME_OR_PASSWORD_INCORRECT);
                    }
                    break;

                default:
                    _message.GetMessageBox(ID.WARN_USER_NOT_EXIST);
                    break;
            }
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            _controller.ClearCurrentPageContent();
            _user.SessionTimeout();
        }

        private void LinkLabelCreateNewAccount_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _controller.ClosePage(Page.LOGIN);
            _controller.iNextPage = Page.CREATE_ACCOUNT;
        }

        internal void LoadLoginCookies()
        {
            switch (!String.IsNullOrWhiteSpace(Properties.Settings.Default.UsernameCookies) && !String.IsNullOrWhiteSpace(Properties.Settings.Default.PasswordCookies))
            {
                case true:
                    TextboxUsername.Text = Properties.Settings.Default.UsernameCookies;
                    TextboxPassword.Text = Properties.Settings.Default.PasswordCookies;
                    ButtonLogin.Select();
                    _logger.Trace<UCPage1>(String.Format("Finish {0}", System.Reflection.MethodBase.GetCurrentMethod().Name));
                    break;

                default:
                    TextboxUsername.Select();
                    break;
            }
        }

        internal void StoreLoginCookies()
        {
            switch (CheckboxStoreLoginCookies.Checked)
            {
                case true:
                    Properties.Settings.Default.UsernameCookies = TextboxUsername.Text;
                    Properties.Settings.Default.PasswordCookies = TextboxUsername.Text;
                    Properties.Settings.Default.Save();
                    break;

                default:
                    Properties.Settings.Default.UsernameCookies = String.Empty;
                    Properties.Settings.Default.PasswordCookies = String.Empty;
                    Properties.Settings.Default.Save();
                    break;
            }
        }

        internal void ClearComponent()
        {
            TextboxUsername.Clear();
            TextboxPassword.Clear();
            CheckboxStoreLoginCookies.Checked = false;
        }


    }
}
