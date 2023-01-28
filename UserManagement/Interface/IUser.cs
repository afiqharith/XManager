using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.Interface
{
    internal interface IUser
    {
        string Username { get; set; }
        string AccessLevel { get; set; }
        string Guid { get; set; }

        bool CreateUser(SqlConnection sqlConnection, string guid, string username, string encryptedPassword);
        bool UpdatePassword(SqlConnection sqlConnection, string encryptedPassword);
        bool UpdateUsername(SqlConnection sqlConnection, string username);
        bool DeactivateAccount(SqlConnection sqlConnection);
    }
}
