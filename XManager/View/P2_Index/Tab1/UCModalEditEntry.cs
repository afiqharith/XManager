using XManager.Utils;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using XManager.SystemMessage;

namespace XManager.UI.PageIndex.Tab1
{
    public partial class UCModalEditEntry : UserControl
    {
        private User _user;
        private Entry _entry;
        private Utils.Message _message;
        private Database _database;
        private ViewController _controller;
        internal UCModalEditEntry(ViewController viewController, User user, ref Entry entry)
        {
            _controller = viewController;
            _user = user;
            _entry = entry;
            _message = new Utils.Message();
            _database = new Database();
            InitializeComponent();
            BindComboboxType();
            GroupboxEntry.Text = String.Format("Edit Entry ID{0}", _entry.id);
            TextboxDescription.Text = _entry.description;
            Datetimepicker.Value = Convert.ToDateTime(_entry.date);
            ComboboxType.Text = _entry.type;
            TextboxQuantity.Text = Convert.ToString(_entry.quantity);
            TextboxPrice.Text = Convert.ToString(_entry.price);
        }

        private void BindComboboxType()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("type", typeof(string));

            string[] choice = new string[]
            {
                "-- SELECT --",
                "Commitment",
                "Saving",
                "Desire"
            };

            for (int i = 0; i < choice.Length; i++)
            {
                dt.Rows.Add(choice[i]);
            }

            ComboboxType.DataSource = dt;
            ComboboxType.DisplayMember = "type";
            ComboboxType.ValueMember = "type";
        }

        private void ButtonChooseAttachment_Click(object sender, EventArgs e)
        {
            if (_entry != null)
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.DefaultExt = "png";
                    openFileDialog.Filter = "Image Files (*.gif, *.jpg, *.jpeg, *.bmp, *.wmf, *.png)|*.gif; *.jpg; *.jpeg; *.bmp; *.wmf; *.png";
                    openFileDialog.CheckFileExists = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        BinaryFileInfo binaryFileInfo = new BinaryFileInfo(openFileDialog.FileName);
                        _entry.binaryInfo = binaryFileInfo;
                    }
                }
            }
        }

        private void ButtonUpdate_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(TextboxPrice.Text) &&
                !String.IsNullOrWhiteSpace(ComboboxType.Text) &&
                !ComboboxType.Text.Equals("-- SELECT --") &&
                !String.IsNullOrWhiteSpace(TextboxQuantity.Text))
            {
                int? foreign_id = _database.GetPrimaryIdTableMaster(Convert.ToString(Datetimepicker.Value));

                int[] sqlResponse = new int[] { 0, 0 };
                using (SqlConnection con = Config.SqlConnection)
                using (SqlCommand cmd = new SqlCommand("UPDATE records SET foreign_id = @foreign_id, date = @date, description = @description, type = @type, quantity = @quantity, price = @price WHERE primary_id = @primary_id AND guid = @guid", con))
                {
                    cmd.Parameters.Add(new SqlParameter("@primary_id", _entry.id));
                    cmd.Parameters.Add(new SqlParameter("@guid", _user.Guid));

                    if (foreign_id != null)
                    {
                        cmd.Parameters.Add(new SqlParameter("@foreign_id", foreign_id));
                    }

                    cmd.Parameters.Add(new SqlParameter("@date", Datetimepicker.Value));

                    if (!String.IsNullOrWhiteSpace(TextboxDescription.Text))
                    {
                        cmd.Parameters.Add(new SqlParameter("@description", TextboxDescription.Text));
                    }
                    else
                    {
                        cmd.Parameters.Add(new SqlParameter("@description", DBNull.Value));
                    }

                    cmd.Parameters.Add(new SqlParameter("@type", ComboboxType.Text));
                    cmd.Parameters.Add(new SqlParameter("@quantity", Convert.ToDouble(TextboxQuantity.Text)));
                    cmd.Parameters.Add(new SqlParameter("@price", Convert.ToDouble(TextboxPrice.Text)));

                    con.Open();
                    sqlResponse[0] = cmd.ExecuteNonQuery();
                    con.Close();
                }

                if (_entry.binaryInfo != null)
                {
                    if (_database.IsBinaryDataExist(_entry.id))
                    {
                        using (SqlConnection con = Config.SqlConnection)
                        using (SqlCommand cmd2 = new SqlCommand("UPDATE appdata SET file_name = @file_name, file_type = @file_type, file_bin = @file_bin WHERE foreign_id = @foreign_id AND guid = @guid", con))
                        {
                            cmd2.Parameters.Add(new SqlParameter("@guid", _user.Guid));

                            if (_entry.id != null)
                            {
                                cmd2.Parameters.Add(new SqlParameter("@foreign_id", _entry.id));
                            }

                            cmd2.Parameters.Add(new SqlParameter("@file_name", _entry.binaryInfo.m_FileName));
                            cmd2.Parameters.Add(new SqlParameter("@file_type", _entry.binaryInfo.m_FileType));
                            cmd2.Parameters.Add(new SqlParameter("@file_bin", (object)_entry.binaryInfo.m_Binary));

                            con.Open();
                            sqlResponse[1] = cmd2.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                    else
                    {
                        using (SqlConnection con = Config.SqlConnection)
                        using (SqlCommand cmd2 = new SqlCommand("INSERT INTO appdata (foreign_id, guid, file_name, file_type, file_bin) VALUES (@foreign_id, @guid, @file_name, @file_type, @file_bin)", con))
                        {
                            cmd2.Parameters.Add(new SqlParameter("@guid", _user.Guid));

                            if (_entry.id != null)
                            {
                                cmd2.Parameters.Add(new SqlParameter("@foreign_id", _entry.id));
                            }

                            cmd2.Parameters.Add(new SqlParameter("@file_name", _entry.binaryInfo.m_FileName));
                            cmd2.Parameters.Add(new SqlParameter("@file_type", _entry.binaryInfo.m_FileType));
                            cmd2.Parameters.Add(new SqlParameter("@file_bin", (object)_entry.binaryInfo.m_Binary));

                            con.Open();
                            sqlResponse[1] = cmd2.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }

                if (sqlResponse[0] > 0 || sqlResponse[1] > 0)
                {
                    _message.GetMessageBox(ID.INFO_RECORD_ENTRY_SUCCESS_UPDATED);
                    _controller.Page2.Tab1ResetNewEntrySection();
                    _controller.Page2.Tab1ResetDgvSection();
                    _controller.Page2.Tab1BindDgv();
                    this.Parent.Hide();
                }
                else
                {
                    _message.GetMessageBox(ID.ERROR_RECORD_ENTRY_FAILURE_UPDATED);
                }
            }
            else
            {
                _message.GetMessageBox(ID.WARN_FILL_EMPTY_FIELD);
            }


        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.Parent.Hide();
        }

        private void TextboxDescription_Validating(object sender, CancelEventArgs e)
        {
            TextboxDescription.ForeColor = TextboxDescription.Text != _entry.description ? Color.Red : Color.Black;
        }

        private void Datetimepicker_Validating(object sender, CancelEventArgs e)
        {

        }

        private void ComboboxType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboboxType.ForeColor = ComboboxType.Text != _entry.type ? Color.Red : SystemColors.WindowText;
        }

        private void TextboxQuantity_Validating(object sender, CancelEventArgs e)
        {
            TextboxQuantity.ForeColor = TextboxQuantity.Text != Convert.ToString(_entry.quantity) ? Color.Red : SystemColors.WindowText;
        }

        private void TextboxPrice_Validating(object sender, CancelEventArgs e)
        {
            TextboxPrice.ForeColor = TextboxPrice.Text != Convert.ToString(_entry.price) ? Color.Red : SystemColors.WindowText;
        }
    }
}
