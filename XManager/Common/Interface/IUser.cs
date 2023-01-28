using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XManager
{
    internal interface IUser
    {
        string Username { get; set; }
        string AccessLevel { get; set; }
        string Guid { get; set; }
        bool SessionTimeout();
    }
}
