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
    internal partial class UCPage4 : UserControl
    {
        private User _user;
        private Logger _logger = Program.Logger;
        private Database _database;
        private Utils.Message _message;
        private ViewController _controller;

        public UCPage4(ViewController viewController)
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
            _controller.iNextPage = Page.INDEX;
            _controller.ClosePage(Page.UPDATE_PASSWORD);
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            TextboxPassword.Clear();
            TextboxRetypePassword.Clear();
            ErrorProvider.Clear();
        }

        private void ButtonUpdate_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(TextboxPassword.Text) && !String.IsNullOrWhiteSpace(TextboxRetypePassword.Text))
            {
                ErrorProvider.Clear();

                if (TextboxPassword.Text != TextboxRetypePassword.Text)
                {
                    _message.GetMessageBox(ID.WARN_PASSWORD_NOT_MATCHED);
                }
                else if (_database.IsPasswordRepeated(_user.Guid, TextboxRetypePassword.Text))
                {
                    _message.GetMessageBox(ID.WARN_SIMILAR_PASSWORD);
                }
                else
                {
                    int response = 0;
                    using (SqlConnection con = Config.SqlConnection)
                    using (SqlCommand cmd = new SqlCommand("UPDATE administration SET password = @password WHERE guid = @guid", con))
                    {
                        cmd.Parameters.Add(new SqlParameter("@guid", _user.Guid));
                        cmd.Parameters.Add(new SqlParameter("@password", PasswordEncryption.GenerateCombinedHashPassword(PasswordEncryption.GetSalt, TextboxRetypePassword.Text)));

                        con.Open();
                        response = cmd.ExecuteNonQuery();
                        con.Close();
                    }

                    if (response > 0)
                    {
                        _controller.ClearCurrentPageContent();
                        LabelUsername.Text = _database.GetUsernameFromGuid(_user.Guid);
                        _message.GetMessageBox(ID.INFO_PASSWORD_ENTRY_SUCCESS_UPDATED);
                    }
                }
            }
            else
            {
                ErrorProvider.SetError(TextboxPassword, "Required");
                ErrorProvider.SetError(TextboxRetypePassword, "Required");
                _message.GetMessageBox(ID.ERROR_EMPTY_FIELD);
            }
        }

        private void TextboxRetypePassword_TextChanged(object sender, EventArgs e)
        {
            string toolTipMsg = (TextboxRetypePassword.Text != TextboxPassword.Text || String.IsNullOrWhiteSpace(TextboxPassword.Text)) ? "Password not match" : String.Empty;
            ErrorProvider.SetError(TextboxRetypePassword, toolTipMsg);
        }

        internal void ClearComponent()
        {
            LabelUsername.Text = "??";
            TextboxPassword.Clear();
            TextboxRetypePassword.Clear();
            ErrorProvider.Clear();
        }
    }
}
