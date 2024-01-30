using Accounts.DAO.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.DAO
{
    public class AccountDao
    {


        private AccountsControlContext _context;
        public AccountDao(AccountsControlContext context)
        {
            this._context = context;
        }


        public void AddAcountInDB(Cuenta cuenta)
        {
            _context.Cuentas.Add(cuenta);
        }

        public void AddClientInDB(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
        }
        public Cliente FindAccountByClient(int? idCliente)
        {
            var cliente = _context.Clientes.FirstOrDefault(c => c.Id == idCliente);

            return cliente;
        }


        public async Task<Cliente> FindClientByUser(string userName)
        {
            var cliente = await this._context.Clientes.FirstOrDefaultAsync(c => c.Usuario == userName);

            return cliente;
        }

        public async Task<Cliente> FindClientByDNI(string DNI)
        {
            var cliente = await this._context.Clientes.FirstOrDefaultAsync(c => c.Dni == DNI);

            return cliente;
        }


        public async Task<bool> validateClientAccount(int id)
        {
            var cuenta = await this._context.Cuentas.AnyAsync(c => c.IdCliente == id);

            return cuenta;
        }

        public Cuenta GetCuenta(string numeroCuenta)
        {
            var cuenta = _context.Cuentas.FirstOrDefault(c => c.NumeroCuenta == numeroCuenta);

            return cuenta;
        }


        public void AccountDeposit(decimal amount, string numeroCuenta)
        {
            var cuenta = _context.Cuentas.FirstOrDefault(c => c.NumeroCuenta == numeroCuenta);

            cuenta.Saldo += amount;
            cuenta.FechaActualizacion = DateTime.Now;
        }

        public void AccountExtract(decimal amount, string numeroCuenta)
        {
            var cuenta = _context.Cuentas.FirstOrDefault(c => c.NumeroCuenta == numeroCuenta);

            cuenta.Saldo -= amount;
            cuenta.FechaActualizacion = DateTime.Now;
        }


      

    }
}
