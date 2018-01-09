using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChordPlay
{
    public class Record
    {
        public string Note { get; set; }
        public string FilePath { get; set; }

        public DateTime DateTime
        {
            get
            {
                return DateTime.Parse(Path.GetFileNameWithoutExtension(FilePath));
            }
        }
    }
}
