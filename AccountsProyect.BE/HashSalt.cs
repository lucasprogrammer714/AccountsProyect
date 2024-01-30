using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsProyect.BE
{
    public class HashSalt
    {  
        public string Hash { get; set; }
        public byte[] Salt { get; set; }
    }
}
