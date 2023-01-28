using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Interface;

namespace UserManagement
{
    public class User : IUser
    {
        public string Username { get; set; }
        public string AccessLevel { get; set; }
        public string Guid { get; set; }

        public User(string username, string accessLevel, string guid)
        {
            Username = username;
            AccessLevel = accessLevel;
            Guid = guid;
        }

        public bool CreateUser(SqlConnection sqlConnection, string guid, string username, string encryptedPassword)
        {
            int response = 0;
            using (sqlConnection)
            using (SqlCommand cmd = new SqlCommand("INSERT INTO administration (guid, username, password, date_joined) VALUES (@guid, @username, @password, @date_joined)", sqlConnection))
            {
                cmd.Parameters.Add(new SqlParameter("@guid", guid));
                cmd.Parameters.Add(new SqlParameter("@username", username));
                cmd.Parameters.Add(new SqlParameter("@password", encryptedPassword));
                cmd.Parameters.Add(new SqlParameter("@date_joined", DateTime.Now));

                sqlConnection.Open();
                response = cmd.ExecuteNonQuery();
                sqlConnection.Close();
            }

            return response > 0;
        }

        public bool UpdateUsername(SqlConnection sqlConnection, string newUsername)
        {
            int response = 0;
            using (sqlConnection)
            using (SqlCommand cmd = new SqlCommand("UPDATE administration SET username = @username WHERE guid = @guid", sqlConnection))
            {
                cmd.Parameters.Add(new SqlParameter("@guid", Guid));
                cmd.Parameters.Add(new SqlParameter("@username", newUsername));

                sqlConnection.Open();
                response = cmd.ExecuteNonQuery();
                sqlConnection.Close();
            }
            return response > 0;
        }

        public bool UpdatePassword(SqlConnection sqlConnection, string encryptedPassword)
        {
            int response = 0;
            using (sqlConnection)
            using (SqlCommand cmd = new SqlCommand("UPDATE administration SET password = @password WHERE guid = @guid", sqlConnection))
            {
                cmd.Parameters.Add(new SqlParameter("@guid", Guid));
                cmd.Parameters.Add(new SqlParameter("@password", encryptedPassword));

                sqlConnection.Open();
                response = cmd.ExecuteNonQuery();
                sqlConnection.Close();
            }
            return response > 0;
        }

        public bool DeactivateAccount(SqlConnection sqlConnection)
        {
            int response = 0;
            string query = "DELETE FROM administration WHERE guid = @guid;" +
                           "DELETE FROM distributions WHERE guid = @guid;" +
                           "DELETE FROM master WHERE guid = @guid;" +
                           "DELETE FROM records WHERE guid = @guid;";

            using (sqlConnection)
            using (SqlCommand cmd = new SqlCommand(query, sqlConnection))
            {
                cmd.Parameters.Add(new SqlParameter("@guid", Guid));

                sqlConnection.Open();
                response = cmd.ExecuteNonQuery();
                sqlConnection.Close();
            }

            return response > 0;
        }
    }
}
