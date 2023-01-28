using XManager.Utils;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using LogLibrary;
using ServiceLibrary;
using XManager.SystemMessage;
using XManager.SystemPage;

namespace XManager
{
    internal partial class UCPage5 : UserControl
    {
        private User _user;
        private Logger _logger = Program.Logger;
        private Database _database;
        private Utils.Message _message;
        private ViewController _controller;
        public UCPage5(ViewController viewController)
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
            _controller.ClosePage(Page.DEACTIVATE_ACCOUNT);
        }

        private void ButtonDeactivate_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = _message.GetMessageBox(ID.WARN_DEACTIVATE_ACCOUNT);

            if (dialogResult == DialogResult.Yes)
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = Config.SqlConnection)
                using (SqlCommand cmd = new SqlCommand("SELECT guid, username, password FROM administration WHERE username = @username", con))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    cmd.Parameters.Add(new SqlParameter("@username", TextboxUsername.Text));

                    con.Open();
                    da.Fill(dt);
                    con.Close();
                }

                if (dt.Rows.Count > 0)
                {
                    (string salt, string hashedPassword) = PasswordEncryption.SplitHashedPassword(Convert.ToString(dt.Rows[0]["password"]));

                    if (PasswordEncryption.ValidateHashedPassword(salt, TextboxUsername.Text, hashedPassword) && Convert.ToString(dt.Rows[0]["username"]).Equals(TextboxUsername.Text) && _user.Guid.Equals(Convert.ToString(dt.Rows[0]["guid"])))
                    {
                        int? response = null;
                        string query = "" +
                            "DELETE FROM administration WHERE guid = @guid;" +
                            "DELETE FROM distributions WHERE guid = @guid;" +
                            "DELETE FROM master WHERE guid = @guid;" +
                            "DELETE FROM records WHERE guid = @guid;";

                        using (SqlConnection con = Config.SqlConnection)
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.Add(new SqlParameter("@guid", _user.Guid));

                            con.Open();
                            response = cmd.ExecuteNonQuery();
                            con.Close();
                        }

                        if (response > 0)
                        {
                            _controller.ClosePage(Page.DEACTIVATE_ACCOUNT);
                            _controller.ClearCurrentPageContent();
                            _controller.InitializeComponentPage();
                            _message.GetMessageBox(ID.INFO_ACCOUNT_SUCCESS_DEACTIVATED);
                        }
                    }
                    else
                    {
                        _message.GetMessageBox(ID.ERROR_USERNAME_OR_PASSWORD_INCORRECT);
                    }
                }
                else
                {
                    _message.GetMessageBox(ID.WARN_USER_NOT_EXIST);
                }
            }
        }

        internal void ClearComponent()
        {
            TextboxUsername.Clear();
            TextboxPassword.Clear();
        }
    }
}
