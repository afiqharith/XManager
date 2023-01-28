using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XManager
{
    internal class Entry : IEntry
    {
        public int? id { get; set; }
        public string description { get; set; }
        public DateTime date { get; set; }
        public string type { get; set; }
        public decimal quantity { get; set; }
        public decimal price { get; set; }
        //public byte[] binary { get; set; }
        public BinaryFileInfo binaryInfo { get; set; }
    }
}
