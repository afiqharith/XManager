using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XManager
{
    class User : IUser
    {
        public string Username { get; set; }
        public string AccessLevel { get; set; }
        public string Guid { get; set; }

        private DateTime dateTimeStart { get; set; }
        private DateTime dateTimeEnd { get; set; }

        public User()
        {
            dateTimeStart = DateTime.Now;
        }

        public User(string username, string accessLevel, string guid)
        {
            this.Username = username;
            this.AccessLevel = accessLevel;
            this.Guid = guid;
            dateTimeStart = DateTime.Now;
        }

        public bool SessionTimeout()
        {
            this.Username = null;
            this.AccessLevel = null;
            this.Guid = null;

            dateTimeEnd = DateTime.Now;

            return (this.Username == null && this.AccessLevel == null && this.Guid == null);
        }

        private TimeSpan? GetLastSessionTimeSpan()
        {
            if (dateTimeEnd != null)
            {
                return dateTimeEnd.Subtract(dateTimeStart);
            }
            else
            {
                return null;
            }
        }
    }
}
