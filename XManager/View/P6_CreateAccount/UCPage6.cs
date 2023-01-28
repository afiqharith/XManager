using XManager.Utils;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using LogLibrary;
using ServiceLibrary;
using XManager.SystemMessage;
using XManager.SystemPage;

namespace XManager
{
    internal partial class UCPage6 : UserControl
    {
        private User _user;
        private Logger _logger = Program.Logger;
        private Database _database;
        private Utils.Message _message;
        private ViewController _controller;

        public UCPage6(ViewController viewController)
        {
            _controller = viewController;
            _user = viewController.User;
            _database = viewController.Database;
            _message = viewController.Message;
            InitializeComponent();
        }

        private void ButtonBack_Click(object sender, EventArgs e)
        {
            _controller.ClearCurrentPageContent();
            _controller.ClosePage(Page.CREATE_ACCOUNT);
            _controller.iNextPage = Page.LOGIN;
        }

        private void ButtonSignUp_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(TextboxUsername.Text) && !String.IsNullOrWhiteSpace(TextboxUsername.Text) && !String.IsNullOrWhiteSpace(TextboxRetypePassword.Text))
            {
                if (_database.IsUsernameExist(TextboxUsername.Text))
                {
                    _message.GetMessageBox(ID.WARN_CHOOSE_DIFFERENT_USERNAME);
                }
                else if (TextboxPassword.Text != TextboxRetypePassword.Text)
                {
                    _message.GetMessageBox(ID.WARN_PASSWORD_NOT_MATCHED);
                }
                else
                {
                    int response = 0;
                    using (SqlConnection con = Config.SqlConnection)
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO administration (guid, username, password, date_joined) VALUES (@guid, @username, @password, @date_joined)", con))
                    {
                        cmd.Parameters.Add(new SqlParameter("@guid", Config.NewGuid));
                        cmd.Parameters.Add(new SqlParameter("@username", TextboxUsername.Text));
                        cmd.Parameters.Add(new SqlParameter("@password", PasswordEncryption.GenerateCombinedHashPassword(PasswordEncryption.GetSalt, TextboxRetypePassword.Text)));
                        cmd.Parameters.Add(new SqlParameter("@date_joined", DateTime.Now));

                        con.Open();
                        response = cmd.ExecuteNonQuery();
                        con.Close();
                    }

                    if (response > 0)
                    {
                        _controller.ClearCurrentPageContent();
                        _controller.iNextPage = Page.LOGIN;
                        _controller.ClosePage(Page.CREATE_ACCOUNT);
                        _message.GetMessageBox(ID.INFO_ACCOUNT_SUCCESS_CREATED);
                    }
                }
            }
            else
            {
                if (String.IsNullOrWhiteSpace(TextboxUsername.Text))
                {
                    ErrorProvider.SetError(TextboxUsername, "Required");
                }
                else
                {
                    ErrorProvider.SetError(TextboxUsername, "");
                }

                if (String.IsNullOrWhiteSpace(TextboxPassword.Text))
                {
                    ErrorProvider.SetError(TextboxPassword, "Required");
                }
                else
                {
                    ErrorProvider.SetError(TextboxPassword, "");
                }

                if (String.IsNullOrWhiteSpace(TextboxRetypePassword.Text))
                {
                    ErrorProvider.SetError(TextboxRetypePassword, "Required");
                }
                else
                {
                    ErrorProvider.SetError(TextboxRetypePassword, "");
                }

                _message.GetMessageBox(ID.WARN_USED_USERNAME);
            }
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            _controller.ClearCurrentPageContent();
        }

        private void TextboxRetypePassword_TextChanged(object sender, EventArgs e)
        {
            string toolTipMsg = (TextboxRetypePassword.Text != TextboxPassword.Text || String.IsNullOrWhiteSpace(TextboxPassword.Text)) ? "Password not match" : String.Empty;
            ErrorProvider.SetError(TextboxRetypePassword, toolTipMsg);
        }

        internal void ClearComponent()
        {
            TextboxUsername.Clear();
            TextboxPassword.Clear();
            TextboxRetypePassword.Clear();
            ErrorProvider.Clear();
        }
    }
}
