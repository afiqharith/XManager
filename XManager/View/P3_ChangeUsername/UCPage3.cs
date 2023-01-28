using XManager.Utils;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using LogLibrary;
using ServiceLibrary;
using XManager.SystemMessage;
using XManager.SystemPage;

namespace XManager
{
    internal partial class UCPage3 : UserControl
    {
        private User _user;
        private Logger _logger = Program.Logger;
        private Database _database;
        private Utils.Message _message;
        private ViewController _controller;

        public UCPage3(ViewController viewController)
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
            _controller.ClosePage(Page.UPDATE_USERNAME);
        }

        private void ButtonUpdate_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(TextboxNewUsername.Text) && !String.IsNullOrWhiteSpace(TextboxPassword.Text))
            {
                ErrorProvider.Clear();
                if (LabelUsername.Text == TextboxNewUsername.Text)
                {
                    _message.GetMessageBox(ID.WARN_SIMILAR_USERNAME);
                }
                else if (_database.IsUsernameExist(TextboxNewUsername.Text))
                {
                    _message.GetMessageBox(ID.WARN_USED_USERNAME);
                }
                else
                {
                    DataTable dt = new DataTable();
                    using (SqlConnection con = Config.SqlConnection)
                    using (SqlCommand cmd = new SqlCommand("SELECT password FROM administration WHERE guid = @guid", con))
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        cmd.Parameters.Add(new SqlParameter("@guid", _user.Guid));

                        con.Open();
                        da.Fill(dt);
                        con.Close();
                    }

                    (string salt, string hashed_password) = PasswordEncryption.SplitHashedPassword(Convert.ToString(dt.Rows[0]["password"]));

                    if (PasswordEncryption.ValidateHashedPassword(salt, TextboxPassword.Text, hashed_password))
                    {
                        int response = 0;
                        using (SqlConnection con = Config.SqlConnection)
                        using (SqlCommand cmd = new SqlCommand("UPDATE administration SET username = @username WHERE guid = @guid", con))
                        {
                            cmd.Parameters.Add(new SqlParameter("@guid", _user.Guid));
                            cmd.Parameters.Add(new SqlParameter("@username", TextboxNewUsername.Text));

                            con.Open();
                            response = cmd.ExecuteNonQuery();
                            con.Close();
                        }

                        if (response > 0)
                        {
                            _controller.ClearCurrentPageContent();
                            _user.Username = _database.GetUsernameFromGuid(_user.Guid);
                            LabelUsername.Text = _user.Username;
                            _message.GetMessageBox(ID.INFO_USERNAME_ENTRY_SUCCESS_UPDATED);
                        }
                    }
                    else
                    {
                        _message.GetMessageBox(ID.ERROR_PASSWORD_INCCORECT);
                    }
                }
            }
            else
            {
                if (String.IsNullOrWhiteSpace(TextboxNewUsername.Text))
                {
                    ErrorProvider.SetError(TextboxNewUsername, "Required");
                }

                if (String.IsNullOrWhiteSpace(TextboxPassword.Text))
                {
                    ErrorProvider.SetError(TextboxPassword, "Required");
                }
                _message.GetMessageBox(ID.ERROR_EMPTY_FIELD);
            }
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            TextboxNewUsername.Clear();
            TextboxPassword.Clear();
            ErrorProvider.Clear();
        }

        private void TextboxNewUsername_Validating(object sender, CancelEventArgs e)
        {
            string toolTipMsg = LabelUsername.Text == TextboxNewUsername.Text ? "Username cannot be similar with the previous registered username" : String.Empty;
            ErrorProvider.SetError(TextboxNewUsername, toolTipMsg);
        }

        internal void ClearComponent()
        {
            LabelUsername.Text = "??";
            TextboxNewUsername.Clear();
            TextboxPassword.Clear();
            ErrorProvider.Clear();
        }
    }
}
