using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountsProyect.BE
{
    public class LoggedUser
    {
        public int idCliente {  get; set; }
        public string? User { get; set; }
        public string? Token { get; set; }   

    }
}
