
using Accounts.DAO;
using Accounts.DAO.Model;
using AccountsProyect.BE;
using AccountsProyect.Security;
using CuentasProyect.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AccountsProyectBLL
{
    public class CuentaManager
    {
        private AccountDao  _accountsDAO;
        private IConfiguration _configuration;
        private AccountsControlContext _context;
        private TokenJwtGenerator _tokenJwtGenerator;
        private IOptions<AudienceModel> _settings;

        public CuentaManager(AccountsControlContext context, IConfiguration configuration, IOptions<AudienceModel> settings) 
        {
             this._accountsDAO = new AccountDao(context);
             this._context = context;
             this._settings = settings;
            this._configuration = configuration;
            this._tokenJwtGenerator = new TokenJwtGenerator(this._settings);
        } 

      
        public async Task<Response> SignIn(Login login)
        {
            var cliente =  await this._accountsDAO.FindClientByUser(login.userName);

            if (cliente == null)
            {
                return new Response((int)SystemEnums.ResponseCode.Error, "Usuario incorrecto");
            }

            var encryption = new Encryption();
            if (!encryption.VerifyPassword(login.Password, cliente.Salt, cliente.Contraseña))
            {
                return new Response((int)SystemEnums.ResponseCode.Error, "Contraseña incorrecta");
            }

            var response = new Response((int)SystemEnums.ResponseCode.Ok, "Usuario logueado");

            var token =  this._tokenJwtGenerator.Generate(cliente.Id, cliente.Usuario, "user");

            var loggedUser = new LoggedUser();
            loggedUser.Token = token;
            loggedUser.User = cliente.Usuario;
            loggedUser.idCliente = cliente.Id;
          
             response.Data = loggedUser;

            return new Response((int)SystemEnums.ResponseCode.Ok, "Usuario logeado", loggedUser);
        }

        public async Task<Response> CreateAccount(Cuenta cuenta, string token)
        {
            var loggedUser = this._tokenJwtGenerator.GetUserIdFromToken(token);
            if (string.IsNullOrEmpty(loggedUser))
            {
                return new Response((int)SystemEnums.ResponseCode.Error, "No se encontro el usuario de sesión");
            }

            var cliente = _accountsDAO.FindAccountByClient(cuenta.IdCliente);
            var FindCuenta = _accountsDAO.GetCuenta(cuenta.NumeroCuenta);

            if (cliente == null)
            {
                return new Response((int)SystemEnums.ResponseCode.Error, "No se encuentra cliente para asociar nueva cuenta");

            }

            if(cuenta.Saldo < 0)
            {
                return new Response((int)SystemEnums.ResponseCode.Error, "No se puede ingresar un saldo de cuenta negativo");
            }

            if(FindCuenta != null)
            {
                return new Response((int)SystemEnums.ResponseCode.Error, "Ya existe el numero de cuenta registrado");
            }

            _accountsDAO.AddAcountInDB(cuenta);

            return new Response((int)SystemEnums.ResponseCode.Ok, "Cuenta creada exitosamente");
        }

        public async Task<Response> CreateCliente(Cliente cliente, string token)
        {
            var loggedUser = this._tokenJwtGenerator.GetUserIdFromToken(token);
            if (string.IsNullOrEmpty(loggedUser))
            {
                return new Response((int)SystemEnums.ResponseCode.Error, "No se encontro el usuario de sesión");
            }

            if (string.IsNullOrEmpty(cliente.Apellido))
            {
                return new Response((int)SystemEnums.ResponseCode.Error, "Campo apellido vacio");
            }
            if (string.IsNullOrEmpty(cliente.Nombre))
            {
                return new Response((int)SystemEnums.ResponseCode.Error, "Campo nombre vacio");
            }

            if (string.IsNullOrEmpty(cliente.Usuario))
            {
                return new Response((int)SystemEnums.ResponseCode.Error, "Campo usuario vacio");
            }
            if (string.IsNullOrEmpty(cliente.Dni))
            {
                return new Response((int)SystemEnums.ResponseCode.Error, "Campo DNI vacio");
            }

            var clienteExists = await this._accountsDAO.FindClientByDNI(cliente.Dni);

            if(clienteExists != null)
            {
                return new Response((int)SystemEnums.ResponseCode.Error, "El cliente ya se encuentra registrado");
            }

            var encryption = new Encryption();
            var hashSalt = encryption.EncryptPassword(cliente.Contraseña);
            cliente.Contraseña = hashSalt.Hash;
            cliente.Salt = hashSalt.Salt;


            _accountsDAO.AddClientInDB(cliente);

            return new Response((int)SystemEnums.ResponseCode.Ok, "Cliente registrado exitosamente");
        }

        public async Task<Response> AccountDeposit(decimal amount, string numeroCuenta, string token)
        {

            var loggedUser = this._tokenJwtGenerator.GetUserIdFromToken(token);
            if (string.IsNullOrEmpty(loggedUser))
            {
                return new Response((int)SystemEnums.ResponseCode.Error, "No se encontro el usuario de sesión");
            }

            bool validateClientAccount = await this._accountsDAO.validateClientAccount(Convert.ToInt32(loggedUser));


            var FindCuenta = _accountsDAO.GetCuenta(numeroCuenta);

            if (FindCuenta == null)
            {
                return new Response((int)SystemEnums.ResponseCode.Error, "No se encuentra cuenta solicitada");
            }

            if(validateClientAccount == true)
            {
                return new Response((int)SystemEnums.ResponseCode.Error, "La cuenta no pertenece a usuario");
            }

            if(amount <= 0)
            {
                return new Response((int)SystemEnums.ResponseCode.Error, "Ingrese un monto valido (mayor a 0) para su deposito");
            }

            _accountsDAO.AccountDeposit(amount, numeroCuenta);

            return new Response((int)SystemEnums.ResponseCode.Ok, "Deposito realizado correctamente");

        }

        
        public async Task<Response> AccountExtract(decimal amount, string numeroCuenta, string token)
        {

            var loggedUser = this._tokenJwtGenerator.GetUserIdFromToken(token);
            if (string.IsNullOrEmpty(loggedUser))
            {
                return new Response((int)SystemEnums.ResponseCode.Error, "No se encontro el usuario de sesión");
            }

            bool validateClientAccount = await this._accountsDAO.validateClientAccount(Convert.ToInt32(loggedUser));
            var FindCuenta = _accountsDAO.GetCuenta(numeroCuenta);

            if (FindCuenta == null)
            {
                return new Response((int)SystemEnums.ResponseCode.Error, "No se encuentra cuenta solicitada");
            }
            if (validateClientAccount == true)
            {
                return new Response((int)SystemEnums.ResponseCode.Error, "La cuenta no pertenece a usuario");
            }
            if (amount <= 0)
            {
                return new Response((int)SystemEnums.ResponseCode.Error, "Ingrese un monto valido (mayor a 0) para su extracción");
            }

            _accountsDAO.AccountExtract(amount, numeroCuenta);

            return new Response((int)SystemEnums.ResponseCode.Ok, "Extracción realizada correctamente");

        }

        public async Task<Response> GetCuentaSaldo(string numeroCuenta, string userName, string token)
        {

            var loggedUser = this._tokenJwtGenerator.GetUserIdFromToken(token);
            if (string.IsNullOrEmpty(loggedUser))
            {
                return new Response((int)SystemEnums.ResponseCode.Error, "No se encontro el usuario de sesión");
            }

            var FindCuenta = _accountsDAO.GetCuenta(numeroCuenta);
            var cliente = await this._accountsDAO.FindClientByUser(userName);

   
            if (FindCuenta == null)
            {
                return new Response((int)SystemEnums.ResponseCode.Error, "No existe el numero de cuenta");
            }

            if (cliente == null)
            {
                return new Response((int)SystemEnums.ResponseCode.Error, "No se encontro usuario");
            }

            CuentaDTO cuentaDto = new CuentaDTO();

            cuentaDto.numeroCuenta = FindCuenta.NumeroCuenta;
            cuentaDto.total = FindCuenta.Saldo;
            cuentaDto.FechaActualizado = (DateTime)FindCuenta.FechaActualizacion;
            cuentaDto.UserCuenta = cliente.Usuario;

            return new Response((int)SystemEnums.ResponseCode.Ok, "Saldo de cuenta",cuentaDto);
        }


    }
}
