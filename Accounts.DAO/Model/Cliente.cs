using System;
using System.Collections.Generic;

namespace Accounts.DAO.Model
{
    public partial class Cliente
    {
        public Cliente()
        {
            Cuenta = new HashSet<Cuenta>();
        }

        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Usuario { get; set; }
        public string? Contraseña { get; set; }
        public string? Dni { get; set; }
        public byte[]? Salt { get; set; }

        public virtual ICollection<Cuenta> Cuenta { get; set; }
    }
}
