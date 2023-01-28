using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32.SafeHandles;
using ServiceLibrary;
using DataTable = System.Data.DataTable;
using Excel = Microsoft.Office.Interop.Excel;

namespace XManager.Utils
{
    class Database : IDisposable
    {
        private SafeHandle _safeHandle = new SafeFileHandle(IntPtr.Zero, true);

        private bool _disposedValue;
        public Database()
        {
            //CreateLocalDatabase();
        }

        public void Dispose() => Dispose(true);
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _safeHandle.Dispose();
                }

                _disposedValue = true;
            }
        }

        public bool IsSalaryDateMasterTableExist(string dateOfSalaryReceived)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = Config.SqlConnection)
            using (SqlCommand cmd = new SqlCommand("SELECT date FROM master", con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                con.Open();
                da.Fill(dt);
                con.Close();
            }

            int dateOccurance = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (Convert.ToDateTime(dt.Rows[i]["date"]).ToString("yyyyMM") == Convert.ToDateTime(dateOfSalaryReceived).ToString("yyyyMM"))
                {
                    dateOccurance += 1;
                }
            }

            return dateOccurance > 0;
        }

        public int? GetPrimaryIdTableMaster(string dateOfSpend)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = Config.SqlConnection)
            using (SqlCommand cmd = new SqlCommand("SELECT primary_id, date FROM master WHERE year(date) = @yearSpend", con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add(new SqlParameter("@yearSpend", Convert.ToDateTime(dateOfSpend).ToString("yyyy")));

                con.Open();
                da.Fill(dt);
                con.Close();
            }

            int? primaryId = null;
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (Convert.ToDateTime(dt.Rows[i]["date"]).ToString("yyyyMM") == Convert.ToDateTime(dateOfSpend).ToString("yyyyMM"))
                    {
                        primaryId = Convert.ToInt32(dt.Rows[i]["primary_id"]);
                        break;
                    }
                }
            }
            return primaryId;
        }

        public bool IsBinaryDataExist(int? foreignId)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = Config.SqlConnection)
            using (SqlCommand cmd = new SqlCommand("SELECT foreign_id FROM appdata WHERE foreign_id = @foreignId", con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add(new SqlParameter("@foreignId", foreignId));

                con.Open();
                da.Fill(dt);
                con.Close();
            }

            return dt.Rows.Count > 0;
        }

        public BinaryFileInfo GetBinaryData(int foreignId)
        {
            DataTable dt = new DataTable();
            BinaryFileInfo cinaryFileInfo = new BinaryFileInfo();
            using (SqlConnection con = Config.SqlConnection)
            using (SqlCommand cmd = new SqlCommand("SELECT file_name, file_type, file_bin FROM appdata WHERE foreign_id = @foreignId", con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add(new SqlParameter("@foreignId", foreignId));

                con.Open();
                da.Fill(dt);
                con.Close();
            }

            if (dt.Rows.Count > 0)
            {
                cinaryFileInfo.m_FileName = Convert.ToString(dt.Rows[0]["file_name"]);
                cinaryFileInfo.m_FileType = Convert.ToString(dt.Rows[0]["file_type"]);
                cinaryFileInfo.m_Binary = (byte[])dt.Rows[0]["file_bin"];
            }
            return cinaryFileInfo;
        }

        public bool IsUsernameExist(string username)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = Config.SqlConnection)
            using (SqlCommand cmd = new SqlCommand("SELECT username FROM administration WHERE username = @username", con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add(new SqlParameter("@username", username));

                con.Open();
                da.Fill(dt);
                con.Close();
            }
            return dt.Rows.Count > 0;
        }

        public string GetUsernameFromGuid(string guid)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = Config.SqlConnection)
            using (SqlCommand cmd = new SqlCommand("SELECT username FROM administration WHERE guid = @guid", con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add(new SqlParameter("@guid", guid));

                con.Open();
                da.Fill(dt);
                con.Close();
            }
            return dt.Rows.Count > 0 ? Convert.ToString(dt.Rows[0]["username"]) : "??";
        }

        public bool IsPasswordRepeated(string guid, string password)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = Config.SqlConnection)
            using (SqlCommand cmd = new SqlCommand("SELECT password FROM administration WHERE guid = @guid", con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add(new SqlParameter("@guid", guid));

                con.Open();
                da.Fill(dt);
                con.Close();
            }
            (string salt, string hashedPassword) = PasswordEncryption.SplitHashedPassword(Convert.ToString(dt.Rows[0]["password"]));
            return PasswordEncryption.ValidateHashedPassword(salt, password, hashedPassword) ? true : false;
        }

        public void LogRecentLoginTime(string guid)
        {
            using (SqlConnection con = Config.SqlConnection)
            using (SqlCommand cmd = new SqlCommand("UPDATE administration SET last_logged = @last_logged WHERE guid = @guid", con))
            {
                cmd.Parameters.Add(new SqlParameter("@guid", guid));
                cmd.Parameters.Add(new SqlParameter("@last_logged", DateTime.Now));

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        public int? GetTableRecordLastInsertId()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = Config.SqlConnection)
            using (SqlCommand cmd = new SqlCommand("SELECT IDENT_CURRENT('dbo.records') AS primary_id;", con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                con.Open();
                da.Fill(dt);
                con.Close();
            }

            if (dt.Rows.Count > 0)
            {
                return Convert.ToInt32(dt.Rows[0]["primary_id"]);
            }
            return null;
        }

        public bool RegisterErrorLogs(Exception ex)
        {
            int response = 0;
            using (SqlConnection con = Config.SqlConnection)
            using (SqlCommand cmd = new SqlCommand("INSERT INTO logs_error (datetime, hresult, error_message, error_full) VALUES (@datetime, @hresult, @error_message, @error_full)", con))
            {
                cmd.Parameters.Add(new SqlParameter("@datetime", DateTime.Now));
                cmd.Parameters.Add(new SqlParameter("@hresult", Convert.ToString(ex.HResult)));
                cmd.Parameters.Add(new SqlParameter("@error_message", Convert.ToString(ex.Message)));
                cmd.Parameters.Add(new SqlParameter("@error_full", Convert.ToString(ex)));

                con.Open();
                response = cmd.ExecuteNonQuery();
                con.Close();
            }

            return response != 0;
        }

        public bool ExportCumulativeDataXls(string guid, string destinationPath)
        {
            SqlCommand[] cmd = new SqlCommand[3];
            SqlDataAdapter[] da = new SqlDataAdapter[3];
            DataTable[] dt = new DataTable[3];

            using (SqlConnection con = Config.SqlConnection)
            using (cmd[0] = new SqlCommand("SELECT commitment, saving, desire FROM distributions WHERE guid = @guid", con))
            using (cmd[1] = new SqlCommand("SELECT primary_id, date, wages, dist_commitment, dist_saving, dist_desire FROM master WHERE guid = @guid", con))
            using (cmd[2] = new SqlCommand("SELECT primary_id, foreign_id, date, description, type, quantity, price FROM records WHERE guid = @guid", con))
            using (da[0] = new SqlDataAdapter(cmd[0]))
            using (da[1] = new SqlDataAdapter(cmd[1]))
            using (da[2] = new SqlDataAdapter(cmd[2]))
            using (dt[0] = new DataTable())
            using (dt[1] = new DataTable())
            using (dt[2] = new DataTable())
            {
                cmd[0].Parameters.Add(new SqlParameter("@guid", guid));
                cmd[1].Parameters.Add(new SqlParameter("@guid", guid));
                cmd[2].Parameters.Add(new SqlParameter("@guid", guid));

                con.Open();
                da[0].Fill(dt[0]);
                da[1].Fill(dt[1]);
                da[2].Fill(dt[2]);
                con.Close();
            }

            if (dt[0].Rows.Count > 0 && dt[1].Rows.Count > 0 && dt[2].Rows.Count > 0)
            {
                Excel.Application excelApp = new Excel.Application();

                if (excelApp == null) { return false; }

                Workbook workbook = excelApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);

                Worksheet worksheetDistribution = workbook.Worksheets[1];
                worksheetDistribution.Name = "Distributions";

                worksheetDistribution.Cells[1, 1] = "Commitment";
                worksheetDistribution.Cells[1, 2] = "Saving";
                worksheetDistribution.Cells[1, 3] = "Desire";

                for (int i = 0; i < dt[0].Rows.Count; i++)
                {
                    worksheetDistribution.Cells[i + 2, 1] = dt[0].Rows[i]["commitment"].ToString();
                    worksheetDistribution.Cells[i + 2, 2] = dt[0].Rows[i]["saving"].ToString();
                    worksheetDistribution.Cells[i + 2, 3] = dt[0].Rows[i]["desire"].ToString();
                }

                Worksheet worksheetComposition = workbook.Worksheets.Add(After: worksheetDistribution);
                worksheetComposition.Name = "Forecast Composition";

                worksheetComposition.Cells[1, 1] = "ID";
                worksheetComposition.Cells[1, 2] = "Date";
                worksheetComposition.Cells[1, 3] = "Salary";
                worksheetComposition.Cells[1, 4] = "Commitment";
                worksheetComposition.Cells[1, 5] = "Saving";
                worksheetComposition.Cells[1, 6] = "Desire";

                for (int i = 0; i < dt[1].Rows.Count; i++)
                {
                    worksheetComposition.Cells[i + 2, 1] = dt[1].Rows[i]["primary_id"].ToString();
                    worksheetComposition.Cells[i + 2, 2] = dt[1].Rows[i]["date"].ToString();
                    worksheetComposition.Cells[i + 2, 3] = dt[1].Rows[i]["wages"].ToString();
                    worksheetComposition.Cells[i + 2, 4] = dt[1].Rows[i]["dist_commitment"].ToString();
                    worksheetComposition.Cells[i + 2, 5] = dt[1].Rows[i]["dist_saving"].ToString();
                    worksheetComposition.Cells[i + 2, 6] = dt[1].Rows[i]["dist_desire"].ToString();
                }

                Worksheet worksheetOverallData = workbook.Worksheets.Add(After: worksheetComposition);
                worksheetOverallData.Name = "Cumulative";

                worksheetOverallData.Cells[1, 1] = "ID";
                worksheetOverallData.Cells[1, 2] = "Forecast Composition ID";
                worksheetOverallData.Cells[1, 3] = "Date";
                worksheetOverallData.Cells[1, 4] = "Description";
                worksheetOverallData.Cells[1, 5] = "Type";
                worksheetOverallData.Cells[1, 6] = "Quantity";
                worksheetOverallData.Cells[1, 7] = "Price";

                for (int i = 0; i < dt[2].Rows.Count; i++)
                {
                    worksheetOverallData.Cells[i + 2, 1] = dt[2].Rows[i]["primary_id"].ToString();
                    worksheetOverallData.Cells[i + 2, 2] = dt[2].Rows[i]["foreign_id"].ToString();
                    worksheetOverallData.Cells[i + 2, 3] = dt[2].Rows[i]["date"].ToString();
                    worksheetOverallData.Cells[i + 2, 4] = dt[2].Rows[i]["description"].ToString();
                    worksheetOverallData.Cells[i + 2, 5] = dt[2].Rows[i]["type"].ToString();
                    worksheetOverallData.Cells[i + 2, 6] = dt[2].Rows[i]["quantity"].ToString();
                    worksheetOverallData.Cells[i + 2, 7] = dt[2].Rows[i]["price"].ToString();
                }

                workbook.SaveAs(destinationPath);
                workbook.Close();

                Marshal.ReleaseComObject(worksheetDistribution);
                Marshal.ReleaseComObject(worksheetComposition);
                Marshal.ReleaseComObject(worksheetOverallData);
                Marshal.ReleaseComObject(workbook);
                Marshal.ReleaseComObject(excelApp);

            }
            return true;
        }

        public int NonQuery(string query, SqlParameter[] param)
        {
            int response = 0;
            using (SqlConnection con = Config.SqlConnection)
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                if (param != null)
                {
                    cmd.Parameters.AddRange(param);
                }
                con.Open();
                response = cmd.ExecuteNonQuery();
                con.Close();
            }
            return response;
        }

        public DataTable Query(string query, SqlParameter[] param)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = Config.SqlConnection)
            using (SqlCommand cmd = new SqlCommand(query, con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                if (param != null)
                {
                    cmd.Parameters.AddRange(param);
                }
                con.Open();
                da.Fill(dt);
                con.Close();
            }
            return dt;
        }

        //TBD
        public bool CreateLocalDatabase()
        {
            string currentDir = Directory.GetCurrentDirectory();

            bool bCreatedMdf = false;
            if (!File.Exists(String.Format("{0}\\ExpenseManagerDb.mdf", currentDir)))
            {
                int response = 0;
                using (SqlConnection con = Config.SqlConnection)
                using (SqlCommand cmd = new SqlCommand(String.Format("IF NOT EXISTS(SELECT [Name] FROM sys.databases WHERE [name] = 'ExpenseManagerDb') CREATE DATABASE ExpenseManagerDb ON PRIMARY (NAME = ExpenseManagerDb_Data, FILENAME = '{0}\\ExpenseManagerDb.mdf', SIZE = 2MB, MAXSIZE = 100MB, FILEGROWTH = 10%) LOG ON (NAME = ExpenseManagerDb_log, FILENAME = '{0}\\ExpenseManagerDbLog.ldf', SIZE = 1MB, MAXSIZE = 50MB, FILEGROWTH = 10%)", currentDir), con))
                {
                    con.Open();
                    response = cmd.ExecuteNonQuery();
                    con.Close();
                }

                if (response != 0)
                {
                    bCreatedMdf = true;
                }


                //FileInfo file = new FileInfo(String.Format("{0}\\AppData\\System\\query.sql", currentDir));
                //string script = file.OpenText().ReadToEnd();
                //using (SqlConnection con = new SqlConnection(m_sConnectionStringMaster))
                //{
                //    Server server = new Server(new ServerConnection(con)); ;
                //    server.ConnectionContext.ExecuteNonQuery(script);
                //}
            }

            return bCreatedMdf;
        }
    }
}
