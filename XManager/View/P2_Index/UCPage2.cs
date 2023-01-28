using XManager.UI.PageIndex.Tab1;
using XManager.SystemMessage;
using XManager.SystemTheme;
using XManager.SystemPage;
using XManager.SystemTab;
using XManager.Utils;
using Microsoft.Office.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Cursor = System.Windows.Forms.Cursor;
using LogLibrary;
using ServiceLibrary;

namespace XManager
{
    internal partial class UCPage2 : UserControl
    {
        internal User _user;
        private Logger _logger = Program.Logger;
        private Database _database;
        private Utils.Message _message;
        private ViewController _controller;
        private string SELECT = "-- SELECT ALL --";
 
        public UCPage2(ViewController viewController)
        {
            _controller = viewController;
            _user = viewController.User;
            _database = viewController.Database;
            _message = viewController.Message;
            InitializeComponent();
            LoadApplicationSetting();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ToolStripLabelClock.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void ToolStripDropDownButtonFile_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripDropDownButtonFile.HideDropDown();
            switch (e.ClickedItem.Name)
            {
                case "ToolStripFileMenuItemSignOut":
                    DialogResult dialogResult = _message.GetMessageBox(ID.QUESTION_LOG_OUT);

                    if (dialogResult == DialogResult.Yes)
                    {
                        _controller.ClearCurrentPageContent();
                        _controller.iNextPage = Page.LOGIN;
                        _controller.ClosePage(Page.INDEX);
                    }
                    else
                    {
                        return;
                    }
                    break;

                case "ToolStripFileMenuItemExit":
                    Application.Exit();
                    break;

                default:
                    break;
            }
        }

        private void ToolStripDropDownButtonSetting_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

            switch (e.ClickedItem.Name)
            {
                case "ToolStripSettingMenuItemManageDistAllocation":
                    ToolStripDropDownButtonSetting.HideDropDown();
                    break;

                default:
                    break;
            }
        }

        private void ToolStripSettingMenuItemAccount_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripDropDownButtonSetting.HideDropDown();
            //_controller.GlobalResetPageContent(ViewController.Page.INDEX);
            switch (e.ClickedItem.Name)
            {
                case "ToolStripSettingMenuItemAccountChangeUsername":
                    UCModalUpdateUsername updateUsernameModal = new UCModalUpdateUsername(_controller);
                    updateUsernameModal.Dock = DockStyle.Fill;
                    Form modalWindow = new Form()
                    {
                        MaximizeBox = false,
                        MinimizeBox = false,
                        StartPosition = FormStartPosition.CenterScreen,
                    };
                    modalWindow.Height = updateUsernameModal.Size.Height;
                    modalWindow.Width = updateUsernameModal.Size.Width;
                    modalWindow.MaximumSize = new Size(modalWindow.Width, modalWindow.Height);
                    modalWindow.MinimumSize = new Size(modalWindow.Width, modalWindow.Height);
                    modalWindow.Controls.Add(updateUsernameModal);
                    modalWindow.FormClosing += (_sender, _e) =>
                    {
                        modalWindow.Hide();
                        modalWindow.Dispose();
                    };
                    modalWindow.ShowDialog();
                    break;

                case "ToolStripSettingMenuItemAccountChangePassword":
                    _controller.iNextPage = Page.UPDATE_PASSWORD;
                    break;

                case "ToolStripSettingMenuItemAccountDeactivateAccount":
                    _controller.iNextPage = Page.DEACTIVATE_ACCOUNT;
                    break;

                default:
                    break;
            }
            //_controller.ClosePage(ViewController.Page.INDEX);
        }

        private void ToolStripDropDownButtonTools_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Name)
            {
                case "ToolStripToolsMenuItemExportCumData":
                    bool bExportSuccess = false;
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.DefaultExt = "xlsx";
                        saveFileDialog.AddExtension = true;
                        saveFileDialog.Filter = "Microsoft Excel Workbook (*.xlsx) | *.xlsx |Microsoft Excel 97-2003 Workbook (*.xls) | *.xls ";
                        saveFileDialog.FileName = String.Format("{0}_CumulativeData", DateTime.Now.ToString("yyyyMMdd-HHmmss"));
                        ToolStripDropDownButtonTools.HideDropDown();

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            bExportSuccess = _database.ExportCumulativeDataXls(_user.Guid, saveFileDialog.FileName);
                        }
                        else
                        {
                            return;
                        }

                        if (bExportSuccess)
                        {
                            _message.GetMessageBox(ID.INFO_CUM_DATA_FILE_SUCCESS_EXPORTED);
                        }
                        else
                        {
                            _message.GetMessageBox(ID.ERROR_CUM_DATA_FILE_FAILED_EXPORTED);
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        private void ToolStripToolsMenuItemCustomize_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //ToolStripDropDownButtonSetting.HideDropDown();
            switch (e.ClickedItem.Name)
            {
                case "ToolStripToolsMenuItemShowClock":
                    ToolStripToolsMenuItemShowClock.Checked = !ToolStripToolsMenuItemShowClock.Checked;
                    Properties.Settings.Default.ShowClock = ToolStripToolsMenuItemShowClock.Checked;
                    Properties.Settings.Default.Save();
                    break;

                default:
                    break;
            }
        }

        private void ToolStripToolsMenuItemTheme_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //ToolStripDropDownButtonSetting.HideDropDown();
            switch (e.ClickedItem.Name)
            {
                case "ToolStripToolsMenuItemThemeDarkMode":
                    ToolStripToolsMenuItemThemeDarkMode.Checked = !ToolStripToolsMenuItemThemeDarkMode.Checked;
                    Properties.Settings.Default.Theme = ToolStripToolsMenuItemThemeDarkMode.Checked ? "Dark" : "Light";
                    Properties.Settings.Default.Save();
                    ChangeTheme(Theme.DARK);
                    break;

                case "ToolStripToolsMenuItemThemeLightMode":
                default:
                    ToolStripToolsMenuItemThemeLightMode.Checked = !ToolStripToolsMenuItemThemeLightMode.Checked;
                    Properties.Settings.Default.Theme = ToolStripToolsMenuItemThemeLightMode.Checked ? "Light" : "Dark";
                    Properties.Settings.Default.Save();
                    ChangeTheme(Theme.LIGHT);
                    break;
            }
        }

        private void ToolStripToolsMenuItemShowClock_CheckedChanged(object sender, EventArgs e)
        {
            //ToolStripLabelClock.Visible = ToolStripToolsMenuItemShowClock.Checked;
        }

        private void ToolStripToolsMenuItemExportLog_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            bool bExportSuccess = false;
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.DefaultExt = "zip";
                saveFileDialog.Filter = "Zip Files (*.zip)|*.zip";
                saveFileDialog.FileName = String.Format("{0}_XMLog", DateTime.Now.ToString("yyyyMMdd-HHmmss"));
                ToolStripDropDownButtonTools.HideDropDown();

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    switch (e.ClickedItem.Name)
                    {
                        case "ToolStripToolsMenuItemExportLog2D":
                            bExportSuccess = _logger.ExportLog(Logger.EXPORT_DAY.TWO_DAYS, saveFileDialog.FileName);
                            break;

                        case "ToolStripToolsMenuItemExportLog7D":
                            bExportSuccess = _logger.ExportLog(Logger.EXPORT_DAY.SEVEN_DAYS, saveFileDialog.FileName);
                            break;

                        case "ToolStripToolsMenuItemExportLog1M":
                        default:
                            bExportSuccess = _logger.ExportLog(Logger.EXPORT_DAY.ONE_MONTH, saveFileDialog.FileName);
                            break;
                    }
                }
                else
                {
                    return;
                }

                if (bExportSuccess)
                {
                    _message.GetMessageBox(ID.INFO_LOG_FILE_SUCCESS_EXPORTED, new string[] { "Location:", Path.GetDirectoryName(saveFileDialog.FileName) });
                }
                else
                {
                    _message.GetMessageBox(ID.ERROR_LOG_FILE_FAILED_EXPORTED);
                }
            }
        }

        internal void ChangeTheme(Theme theme)
        {
            switch (theme)
            {
                case Theme.DARK:
                    break;

                case Theme.LIGHT:
                    GetPageControls(this);
                    foreach (KeyValuePair<Control, String> kvp in PageControls)
                    {
                        if (kvp.Value.Contains("ThemeCustomisable:Color1"))
                        {
                            kvp.Key.BackColor = Color.Silver;
                        }
                        else if (kvp.Value.Contains("ThemeCustomisable:Color2"))
                        {
                            kvp.Key.BackColor = Color.Gainsboro;
                        }
                        else if (kvp.Value.Contains("ThemeCustomisable:ColorFont"))
                        {
                            kvp.Key.ForeColor = SystemColors.WindowText;
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        private void LoadApplicationSetting()
        {
            ToolStripToolsMenuItemShowClock.Checked = Properties.Settings.Default.ShowClock;

            if (String.IsNullOrWhiteSpace(Properties.Settings.Default.Theme))
                return;

            switch (Properties.Settings.Default.Theme)
            {
                case "Dark":
                    ChangeTheme(Theme.DARK);
                    break;

                case "Light":
                default:
                    ChangeTheme(Theme.LIGHT);
                    break;
            }

        }

        private void GetPageControls(Control mainControl)
        {
            foreach (Control control in mainControl.Controls)
            {
                if (control.Tag != null)
                {
                    PageControls.Add(control, control.Tag.ToString());
                }
                GetPageControls(control);
            }
        }

        private Dictionary<Control, String> PageControls = new Dictionary<Control, String>();

        #region [Tab 1]
        private void Tab1ButtonClear_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            switch (button.Name)
            {
                case "Tab1ButtonClearTotalSalary":
                    Tab1ResetWageSection();
                    break;

                case "Tab1ButtonClearNewEntry":
                    Tab1ResetNewEntrySection();
                    break;

                case "Tab1ButtonClearDgv":
                    Tab1ResetDgvSection();
                    Tab1ComboboxYearMonth_SelectionChangeCommitted(sender, e);
                    break;

                default:
                    break;
            }
        }

        private void TabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            switch ((P2Tab)TabControl.SelectedIndex)
            {
                case P2Tab.TAB_OVERVIEW:
                    break;

                case P2Tab.TAB_ANALYSIS:
                    e.Cancel = true;
                    break;

                default:
                    break;
            }
        }

        private void Tab1ButtonSubmitTotalSalary_Click(object sender, EventArgs e)
        {

            if (!String.IsNullOrWhiteSpace(Tab1TextboxTotalSalary.Text))
            {
                DataTable dt1 = new DataTable();
                using (SqlConnection con = Config.SqlConnection)
                using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 commitment, saving, desire FROM distributions WHERE guid = @guid", con))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    cmd.Parameters.Add(new SqlParameter("@guid", _user.Guid));

                    con.Open();
                    da.Fill(dt1);
                    con.Close();
                }

                int? response_created = null, response_updated = null;
                if (!_database.IsSalaryDateMasterTableExist(Convert.ToString(Tab1Datetimepicker1.Value)))
                {
                    using (SqlConnection con = Config.SqlConnection)
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO master (guid, date, wages, dist_commitment, dist_saving, dist_desire) VALUES (@guid, @date, @wages, @dist_commitment, @dist_saving, @dist_desire)", con))
                    {
                        cmd.Parameters.Add(new SqlParameter("@guid", _user.Guid));
                        cmd.Parameters.Add(new SqlParameter("@date", Tab1Datetimepicker1.Value));
                        cmd.Parameters.Add(new SqlParameter("@wages", Tab1TextboxTotalSalary.Text));
                        cmd.Parameters.Add(new SqlParameter("@dist_commitment", Convert.ToDouble(Tab1TextboxTotalSalary.Text) * Convert.ToDouble(dt1.Rows[0]["commitment"])));
                        cmd.Parameters.Add(new SqlParameter("@dist_saving", Convert.ToDouble(Tab1TextboxTotalSalary.Text) * Convert.ToDouble(dt1.Rows[0]["saving"])));
                        cmd.Parameters.Add(new SqlParameter("@dist_desire", Convert.ToDouble(Tab1TextboxTotalSalary.Text) * Convert.ToDouble(dt1.Rows[0]["desire"])));

                        con.Open();
                        response_created = cmd.ExecuteNonQuery();
                        con.Close();
                    }

                    if (response_created > 0)
                    {
                        _message.GetMessageBox(ID.INFO_RECORD_TOTAL_SALARY_SUCCESS_CREATED);
                        Tab1ResetWageSection();
                    }
                    else
                    {
                        _message.GetMessageBox(ID.ERROR_RECORD_TOTAL_SALARY_FAILURE_CREATED);
                    }
                }
                else
                {
                    using (SqlConnection con = Config.SqlConnection)
                    using (SqlCommand cmd = new SqlCommand("UPDATE master SET wages = @wages, dist_commitment = @dist_commitment, dist_saving = @dist_saving, dist_desire = @dist_desire WHERE date = @date AND guid = @guid", con))
                    {
                        cmd.Parameters.Add(new SqlParameter("@guid", _user.Guid));
                        cmd.Parameters.Add(new SqlParameter("@date", Tab1Datetimepicker1.Value));
                        cmd.Parameters.Add(new SqlParameter("@wages", Tab1TextboxTotalSalary.Text));
                        cmd.Parameters.Add(new SqlParameter("@dist_commitment", Convert.ToDouble(Tab1TextboxTotalSalary.Text) * Convert.ToDouble(dt1.Rows[0]["commitment"])));
                        cmd.Parameters.Add(new SqlParameter("@dist_saving", Convert.ToDouble(Tab1TextboxTotalSalary.Text) * Convert.ToDouble(dt1.Rows[0]["saving"])));
                        cmd.Parameters.Add(new SqlParameter("@dist_desire", Convert.ToDouble(Tab1TextboxTotalSalary.Text) * Convert.ToDouble(dt1.Rows[0]["desire"])));

                        con.Open();
                        response_updated = cmd.ExecuteNonQuery();
                        con.Close();
                    }

                    if (response_updated > 0)
                    {
                        _message.GetMessageBox(ID.INFO_RECORD_TOTAL_SALARY_SUCCESS_UPDATED);
                        Tab1ResetWageSection();
                    }
                    else
                    {
                        _message.GetMessageBox(ID.ERROR_RECORD_TOTAL_SALARY_FAILURE_UPDATED);
                    }
                }
            }
            else
            {
                _message.GetMessageBox(ID.WARN_FILL_EMPTY_FIELD);
            }
        }

        private void Tab1ButtonSubmitNewEntry_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(Tab1TextboxPrice.Text) && !String.IsNullOrWhiteSpace(Tab1ComboboxType.Text) && !Tab1ComboboxType.Text.Equals(SELECT) && !String.IsNullOrWhiteSpace(Tab1TextboxQuantity.Text))
            {
                int? foreign_id = _database.GetPrimaryIdTableMaster(Convert.ToString(Tab1Datetimepicker2.Value));

                if (foreign_id != null)
                {
                    int[] sqlResponse = new int[] { 0, 0 };
                    using (SqlConnection con = Config.SqlConnection)
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO records (foreign_id, guid, date, description, type, quantity, price) VALUES (@foreign_id, @guid, @date, @description, @type, @quantity, @price)", con))
                    {
                        if (foreign_id != null)
                        {
                            cmd.Parameters.Add(new SqlParameter("@foreign_id", foreign_id));
                        }

                        cmd.Parameters.Add(new SqlParameter("@guid", _user.Guid));

                        cmd.Parameters.Add(new SqlParameter("@date", Tab1Datetimepicker2.Value));

                        if (!String.IsNullOrWhiteSpace(Tab1TextboxDescription.Text))
                        {
                            cmd.Parameters.Add(new SqlParameter("@description", Tab1TextboxDescription.Text));
                        }
                        else
                        {
                            cmd.Parameters.Add(new SqlParameter("@description", DBNull.Value));
                        }

                        cmd.Parameters.Add(new SqlParameter("@type", Tab1ComboboxType.Text));
                        cmd.Parameters.Add(new SqlParameter("@quantity", Convert.ToDouble(Tab1TextboxQuantity.Text)));
                        cmd.Parameters.Add(new SqlParameter("@price", Convert.ToDouble(Tab1TextboxPrice.Text)));

                        con.Open();
                        sqlResponse[0] = cmd.ExecuteNonQuery();
                        con.Close();
                    }

                    //tbd entry is not follow current data
                    if (_bufferEntry != null && _bufferEntry.binaryInfo != null)
                    {
                        if (sqlResponse[0] > 0)
                        {
                            _bufferEntry.id = _database.GetTableRecordLastInsertId();
                        }

                        using (SqlConnection con = Config.SqlConnection)
                        using (SqlCommand cmd = new SqlCommand("INSERT INTO appdata (foreign_id, guid, file_name, file_type, file_bin) VALUES (@foreign_id, @guid, @file_name, @file_type, @file_bin)", con))
                        {
                            cmd.Parameters.Add(new SqlParameter("@guid", _user.Guid));
                            cmd.Parameters.Add(new SqlParameter("@foreign_id", _bufferEntry.id));
                            cmd.Parameters.Add(new SqlParameter("@file_name", _bufferEntry.binaryInfo.m_FileName));
                            cmd.Parameters.Add(new SqlParameter("@file_type", _bufferEntry.binaryInfo.m_FileType));
                            cmd.Parameters.Add(new SqlParameter("@file_bin", (object)_bufferEntry.binaryInfo.m_Binary));

                            con.Open();
                            sqlResponse[1] = cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }

                    if (sqlResponse[0] > 0 || sqlResponse[1] > 0)
                    {
                        _message.GetMessageBox(ID.INFO_RECORD_ENTRY_SUCCESS_CREATED);
                        Tab1ResetNewEntrySection();
                        Tab1ResetDgvSection();
                        Tab1BindDgv();
                    }
                    else
                    {
                        _message.GetMessageBox(ID.ERROR_RECORD_ENTRY_FAILED_CREATED);
                    }
                }
                else
                {
                    _message.GetMessageBox(ID.ERROR_WAGE_MONTH_NOT_FOUND);
                }
            }
            else
            {
                _message.GetMessageBox(ID.WARN_FILL_EMPTY_FIELD);
            }
        }

        private void Tab1ButtonChooseAttachment_Click(object sender, EventArgs e)
        {
            _bufferEntry = null;
            _bufferEntry = new Entry();
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.DefaultExt = "png";
                openFileDialog.Filter = "Image Files (*.gif, *.jpg, *.jpeg, *.bmp, *.wmf, *.png)|*.gif; *.jpg; *.jpeg; *.bmp; *.wmf; *.png";
                openFileDialog.CheckFileExists = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    BinaryFileInfo binaryFileInfo = new BinaryFileInfo(openFileDialog.FileName);
                    _bufferEntry.binaryInfo = binaryFileInfo;
                }
            }
        }

        private void Tab1ComboboxYearMonth_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string query = null;
            if (!Convert.ToString(Tab1ComboboxYear.SelectedValue).Equals(SELECT) && !Tab1ComboboxMonth.Text.Equals(SELECT))
            {
                query = "SELECT primary_id, date AS Date, description AS Description, type AS Type, quantity AS Quantity, price AS Price FROM records WHERE DATENAME(year, Date) = @year AND MONTH(Date) = @month AND guid = @guid ORDER BY Date DESC";
            }
            else if (!Convert.ToString(Tab1ComboboxYear.SelectedValue).Equals(SELECT) && Tab1ComboboxMonth.Text.Equals(SELECT))
            {
                query = "SELECT primary_id, date AS Date, description AS Description, type AS Type, quantity AS Quantity, price AS Price FROM records WHERE DATENAME(year, Date) = @year AND guid = @guid ORDER BY Date DESC";
            }
            else if (Convert.ToString(Tab1ComboboxYear.SelectedValue).Equals(SELECT) && !Tab1ComboboxMonth.Text.Equals(SELECT))
            {
                query = "SELECT primary_id, date AS Date, description AS Description, type AS Type, quantity AS Quantity, price AS Price FROM records WHERE MONTH(Date) = @month AND guid = @guid ORDER BY Date DESC";
            }
            else
            {
                Tab1BindDgv();
            }

            if (!String.IsNullOrWhiteSpace(query))
            {
                DataTable dt = new DataTable();
                using (SqlConnection con = Config.SqlConnection)
                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    cmd.Parameters.Add(new SqlParameter("@guid", _user.Guid));

                    if (!Convert.ToString(Tab1ComboboxYear.Text).Equals(SELECT))
                    {
                        cmd.Parameters.Add(new SqlParameter("@year", Tab1ComboboxYear.SelectedValue));
                    }

                    if (!Tab1ComboboxMonth.Text.Equals(SELECT))
                    {
                        cmd.Parameters.Add(new SqlParameter("@month", Tab1ComboboxMonth.SelectedValue));
                    }

                    con.Open();
                    da.Fill(dt);
                    con.Close();
                }

                Tab1Dgv.DataSource = dt.Rows.Count > 0 ? dt : null;

                if (Tab1Dgv.RowCount > 0)
                {
                    Tab1Dgv.Columns["Date"].DefaultCellStyle.Format = "yyyy-MM-dd";
                    Tab1Dgv.Columns["Price"].DefaultCellStyle.Format = "0.00";
                    Tab1Dgv.Columns["primary_id"].Visible = false;
                }
            }
        }

        private void Tab1Dgv_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex > -1)
            {
                Tab1Dgv.Rows[e.RowIndex].Selected = true;

                if (Tab1Dgv.Focus() && Tab1Dgv.Rows[e.RowIndex].Selected)
                {
                    ContextMenuStrip contexMenu = new ContextMenuStrip();

                    ToolStripItem viewAttachmentToolStripItem = contexMenu.Items.Add("View attachment");
                    viewAttachmentToolStripItem.Enabled = _database.IsBinaryDataExist(Convert.ToInt16(Tab1Dgv.Rows[e.RowIndex].Cells["primary_id"].Value));
                    viewAttachmentToolStripItem.Click += (sender_, e_) => Tab1Dgv_RightClickViewAttachment(sender, e);

                    ToolStripItem editToolStripItem = contexMenu.Items.Add("Edit");
                    editToolStripItem.Click += (sender_, e_) => Tab1Dgv_RightClickEdit(sender, e);

                    ToolStripItem deleteToolStripItem = contexMenu.Items.Add("Delete");
                    deleteToolStripItem.Click += (sender_, e_) => Tab1Dgv_RightClickDelete(sender, e);

                    contexMenu.Show(Cursor.Position.X, Cursor.Position.Y);
                }
            }
        }

        private void Tab1Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Tab1ResetNewEntrySection();
        }

        private Entry _bufferEntry;
        private void Tab1Dgv_RightClickEdit(object sender, DataGridViewCellMouseEventArgs e)
        {
            _bufferEntry = null;
            _bufferEntry = new Entry();
            if (e.RowIndex > -1)
            {
                ////Start Test
                _bufferEntry.id = Convert.ToInt32(Tab1Dgv.Rows[e.RowIndex].Cells["primary_id"].Value);
                _bufferEntry.description = Convert.ToString(Tab1Dgv.Rows[e.RowIndex].Cells["Description"].Value);
                _bufferEntry.date = Convert.ToDateTime(Tab1Dgv.Rows[e.RowIndex].Cells["Date"].Value);
                _bufferEntry.type = Convert.ToString(Tab1Dgv.Rows[e.RowIndex].Cells["Type"].Value);
                _bufferEntry.quantity = Convert.ToDecimal(Tab1Dgv.Rows[e.RowIndex].Cells["Quantity"].Value);
                _bufferEntry.price = Convert.ToDecimal(Tab1Dgv.Rows[e.RowIndex].Cells["Price"].Value);

                UCModalEditEntry editEntryModal = new UCModalEditEntry(_controller, _user, ref _bufferEntry);
                editEntryModal.Dock = DockStyle.Fill;
                Form modalWindow = new Form()
                {
                    Height = editEntryModal.Size.Height,
                    Width = editEntryModal.Size.Width,
                    FormBorderStyle = FormBorderStyle.None
                };
                modalWindow.Icon = new Icon(Path.Combine(Config.AppDataPath, "Image", "002.ico"), new Size(36, 36));
                modalWindow.StartPosition = FormStartPosition.CenterScreen;
                modalWindow.Controls.Add(editEntryModal);
                modalWindow.ShowDialog();
                ////End
            }
        }

        private void Tab1Dgv_RightClickDelete(object sender, DataGridViewCellMouseEventArgs e)
        {
            _bufferEntry = null;
            _bufferEntry = new Entry();
            if (e.RowIndex > -1)
            {
                _bufferEntry.id = Convert.ToInt32(Tab1Dgv.Rows[e.RowIndex].Cells["primary_id"].Value);
            }

            if (_bufferEntry != null)
            {
                DialogResult dialogResult = _message.GetMessageBox(ID.WARN_DELETE_RECORD);
                if (dialogResult == DialogResult.Yes)
                {
                    int sqlResponse = 0;
                    using (SqlConnection con = Config.SqlConnection)
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM records WHERE primary_id = @primary_id AND guid = @guid", con))
                    {
                        cmd.Parameters.Add(new SqlParameter("@primary_id", _bufferEntry.id));
                        cmd.Parameters.Add(new SqlParameter("@guid", _user.Guid));

                        con.Open();
                        sqlResponse = cmd.ExecuteNonQuery();
                        con.Close();
                    }

                    if (sqlResponse > 0)
                    {
                        _message.GetMessageBox(ID.INFO_RECORD_ENTRY_SUCCESS_DELETED);
                        Tab1ResetNewEntrySection();
                        Tab1ResetDgvSection();
                        Tab1BindDgv();
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                _message.GetMessageBox(ID.WARN_SELECT_RECORD_FROM_TABLE_WARNING);
            }
        }

        private void Tab1Dgv_RightClickViewAttachment(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex > -1 && _database.IsBinaryDataExist(Convert.ToInt16(Tab1Dgv.Rows[e.RowIndex].Cells["primary_id"].Value)))
            {
                BinaryFileInfo binaryFileInfo = _database.GetBinaryData(Convert.ToInt16(Tab1Dgv.Rows[e.RowIndex].Cells["primary_id"].Value));
                PictureBox picturebox = new PictureBox()
                {
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Dock = DockStyle.Fill
                };

                if (binaryFileInfo != null)
                {
                    picturebox.Image = FileConverter.ByteToImage(binaryFileInfo.m_Binary);
                }

                Form modalWindow = new Form()
                {
                    Height = picturebox.Size.Height,
                    Width = picturebox.Size.Width,
                    Text = binaryFileInfo.m_FileName,
                    MaximumSize = new Size(Width, Height),
                    MinimumSize = new Size(Width, Height),
                    MaximizeBox = false,
                    MinimizeBox = false,
                    Icon = new Icon(Path.Combine(Config.AppDataPath, "Image", "003.ico"), new Size(36, 36)),
                    StartPosition = FormStartPosition.CenterScreen
                };
                modalWindow.Controls.Add(picturebox);

                modalWindow.FormClosing += (_sender, _e) =>
                {
                    modalWindow.Hide();
                };

                modalWindow.ShowDialog();
            }
        }

        private void Tab1InitializeChart()
        {
            Tab1ChartOverview.Controls.Clear();
            Tab1ChartOverview.Series.Clear();
            Tab1ChartOverview.Titles.Clear();

            Tab1ChartOverview.Titles.Add("Monthly Composition");
            Tab1ChartOverview.Series.Add("Commitment");
            Tab1ChartOverview.Series.Add("Saving");
            Tab1ChartOverview.Series.Add("Desire");
            Tab1ChartOverview.Series.Add("Salary");
            Tab1ChartOverview.Series.Add("Utilization");

            Tab1ChartOverview.Series["Salary"].ChartType = SeriesChartType.FastLine;
            Tab1ChartOverview.Series["Salary"].BorderWidth = 2;
            Tab1ChartOverview.Series["Salary"].Color = Color.Blue;

            Tab1ChartOverview.Series["Utilization"].ChartType = SeriesChartType.FastLine;
            Tab1ChartOverview.Series["Utilization"].BorderWidth = 2;
            Tab1ChartOverview.Series["Utilization"].Color = Color.OrangeRed;

            Tab1ChartOverview.Series["Commitment"].ChartType = SeriesChartType.Column;
            Tab1ChartOverview.Series["Commitment"].SetCustomProperty("PixelPointWidth", "40");
            Tab1ChartOverview.Series["Commitment"].Color = Color.Maroon;

            Tab1ChartOverview.Series["Saving"].ChartType = SeriesChartType.Column;
            Tab1ChartOverview.Series["Saving"].SetCustomProperty("PixelPointWidth", "40");
            Tab1ChartOverview.Series["Saving"].Color = Color.Black;

            Tab1ChartOverview.Series["Desire"].ChartType = SeriesChartType.Column;
            Tab1ChartOverview.Series["Desire"].SetCustomProperty("PixelPointWidth", "40");
            Tab1ChartOverview.Series["Desire"].Color = Color.DimGray;

            Tab1ChartOverview.ChartAreas["ChartArea1"].AxisY.Title = "MYR";

            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            using (SqlConnection con = Config.SqlConnection)
            using (SqlCommand cmd1 = new SqlCommand("SELECT TOP 6 date, wages, dist_commitment, dist_saving, dist_desire FROM master WHERE guid = @guid ORDER BY date DESC", con))
            using (SqlCommand cmd2 = new SqlCommand("SELECT TOP 6 FORMAT(date, 'yyyy-MM') AS yearmonth, SUM(price) AS total FROM records group by FORMAT(date, 'yyyy-MM') ORDER BY FORMAT(date, 'yyyy-MM') DESC", con))
            using (SqlDataAdapter da1 = new SqlDataAdapter(cmd1))
            using (SqlDataAdapter da2 = new SqlDataAdapter(cmd2))
            {
                cmd1.Parameters.Add(new SqlParameter("@guid", _user.Guid));

                con.Open();
                da1.Fill(dt1);
                da2.Fill(dt2);
                con.Close();
            }

            if (dt1.Rows.Count > 0 && dt2.Rows.Count > 0)
            {
                for (int i = dt1.Rows.Count - 1; i > -1; i--)
                {
                    Tab1ChartOverview.Series["Salary"].Points.AddXY(Convert.ToDateTime(dt1.Rows[i]["date"]).ToString("MMM yyyy"), Convert.ToString(dt1.Rows[i]["wages"]));
                    Tab1ChartOverview.Series["Commitment"].Points.AddXY(Convert.ToDateTime(dt1.Rows[i]["date"]).ToString("MMM yyyy"), Convert.ToString(dt1.Rows[i]["dist_commitment"]));
                    Tab1ChartOverview.Series["Saving"].Points.AddXY(Convert.ToDateTime(dt1.Rows[i]["date"]).ToString("MMM yyyy"), Convert.ToString(dt1.Rows[i]["dist_saving"]));
                    Tab1ChartOverview.Series["Desire"].Points.AddXY(Convert.ToDateTime(dt1.Rows[i]["date"]).ToString("MMM yyyy"), Convert.ToString(dt1.Rows[i]["dist_desire"]));
                }

                for (int i = dt2.Rows.Count - 1; i > -1; i--)
                {
                    Tab1ChartOverview.Series["Utilization"].Points.AddXY(Convert.ToDateTime(dt2.Rows[i]["yearmonth"]).ToString("MMM yyyy"), Convert.ToString(dt2.Rows[i]["total"]));
                }
            }
        }

        private void Tab1BindComboboxType()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("type", typeof(string));

            string[] choice = new string[]
            {
                SELECT,
                "Commitment",
                "Saving",
                "Desire"
            };

            for (int i = 0; i < choice.Length; i++)
            {
                dt.Rows.Add(choice[i]);
            }

            Tab1ComboboxType.DataSource = dt;
            Tab1ComboboxType.DisplayMember = "type";
            Tab1ComboboxType.ValueMember = "type";
        }

        private void Tab1BindComboboxYear()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = Config.SqlConnection)
            using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT(FORMAT(date, 'yyyy')) AS year FROM records WHERE guid = @guid ORDER BY year DESC", con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add(new SqlParameter("@guid", _user.Guid));

                con.Open();
                da.Fill(dt);
                con.Close();
            }

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.NewRow();
                dr["year"] = SELECT;
                dt.Rows.InsertAt(dr, 0);

                Tab1ComboboxYear.DataSource = dt;
                Tab1ComboboxYear.DisplayMember = "year";
                Tab1ComboboxYear.ValueMember = "year";
            }
        }

        private void Tab1BindComboboxMonth()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = Config.SqlConnection)
            using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT MONTH(date) AS monthd, FORMAT(date, 'MMMM') months FROM [records] WHERE guid = @guid ORDER BY monthd ASC", con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add(new SqlParameter("@guid", _user.Guid));

                con.Open();
                da.Fill(dt);
                con.Close();
            }

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.NewRow();
                dr["months"] = SELECT;
                dt.Rows.InsertAt(dr, 0);

                Tab1ComboboxMonth.DataSource = dt;
                Tab1ComboboxMonth.DisplayMember = "months";
                Tab1ComboboxMonth.ValueMember = "monthd";
            }
        }

        internal void Tab1BindDgv()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = Config.SqlConnection)
            using (SqlCommand cmd = new SqlCommand("SELECT primary_id, date AS Date, description AS Description, type AS Type, quantity AS Quantity, price AS Price FROM records WHERE guid = @guid ORDER BY Date DESC", con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add(new SqlParameter("@guid", _user.Guid));

                con.Open();
                da.Fill(dt);
                con.Close();
            }
            Tab1Dgv.DataSource = dt.Rows.Count > 0 ? dt : null;

            if (Tab1Dgv.RowCount > 0)
            {
                Tab1Dgv.Columns["Date"].DefaultCellStyle.Format = "yyyy-MM-dd";
                Tab1Dgv.Columns["Price"].DefaultCellStyle.Format = "0.00";
                Tab1Dgv.Columns["primary_id"].Visible = false;
            }
        }

        private void Tab1ResetWageSection()
        {
            Tab1Datetimepicker1.Value = DateTime.Now;
            Tab1TextboxTotalSalary.Clear();
            Tab1InitializeChart();
        }

        internal void Tab1ResetNewEntrySection()
        {
            Tab1TextboxDescription.Clear();
            Tab1Datetimepicker2.Value = DateTime.Now;
            Tab1ComboboxType.SelectedIndex = 0;
            Tab1TextboxQuantity.Clear();
            Tab1TextboxPrice.Clear();
            Tab1ButtonSubmitNewEntry.Text = "Submit";

            if (_bufferEntry != null)
            {
                _bufferEntry = null;
            }
        }

        internal void Tab1ResetDgvSection()
        {
            Tab1ComboboxYear.SelectedIndex = 0;
            Tab1ComboboxMonth.SelectedIndex = 0;
        }

        internal void Tab1InitializeDataBind()
        {
            PanelBase.Show();
            Action[] action = new Action[]
            {
                Tab1InitializeChart,
                Tab1BindDgv,
                Tab1BindComboboxType,
                Tab1BindComboboxYear,
                Tab1BindComboboxMonth
            };

            for (int i = 0; i < action.Length; i++)
            {
                action[i].Invoke();
                _logger.Trace<UCPage2>(String.Format("Finish {0}", action[i].Method.Name));
            }
            _logger.Trace<UCPage2>(String.Format("Finish {0}", System.Reflection.MethodBase.GetCurrentMethod().Name));
        }

        internal void ClearComponent()
        {
            Action[] action = new Action[]
            {
                Tab1ResetWageSection,
                Tab1ResetNewEntrySection,
                Tab1ResetDgvSection
            };

            for (int i = 0; i < action.Length; i++)
            {
                action[i].Invoke();
                _logger.Trace<UCPage2>(String.Format("Finish {0}", action[i].Method.Name));
            }
            _logger.Trace<UCPage2>(String.Format("Finish {0}", System.Reflection.MethodBase.GetCurrentMethod().Name));
        }

        #endregion


    }
}
