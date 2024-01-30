using System;
using System.Collections.Generic;

namespace Accounts.DAO.Model
{
    public partial class Cuenta
    {
        public int Id { get; set; }
        public string? NumeroCuenta { get; set; }
        public decimal? Saldo { get; set; }
        public int? IdCliente { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        public virtual Cliente? IdClienteNavigation { get; set; }
    }
}
