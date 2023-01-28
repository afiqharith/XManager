using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XManager
{
    internal interface IEntry
    {
        string description { get; set; }
        DateTime date { get; set; }
        string type { get; set; }
        decimal quantity { get; set; }
        decimal price { get; set; }
        //byte[] binary { get; set; }
        BinaryFileInfo binaryInfo { get; set; }
    }
}
