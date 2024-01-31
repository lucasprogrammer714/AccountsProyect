using Accounts.DAO.Model;
using AccountsProyect.BE;
using AccountsProyectBLL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CuentasProyect.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]

    [ApiController]
    public class AccountsController : ControllerBase
    {

        private AccountsControlContext _context;
        private CuentaManager _accountManager;
        private IOptions<AudienceModel> _settings;
        private IConfiguration _configuration;


        public AccountsController(IConfiguration configuration, AccountsControlContext context, IOptions<AudienceModel> settings)
        {
            this._context = context;
            this._settings = settings;
            this._configuration = configuration;
            this._accountManager = new CuentaManager(this._context, this._configuration, this._settings);
        }

        [Authorize]
        [HttpGet("private")]
        public IActionResult Private()
        {
            return Ok(new
            {
                Message = "Hello from a private endpoint! You need to be authenticated to see this."
            });
        }



        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] Login login)
        {
            try
            {
                var response = await this._accountManager.SignIn(login);
                await this._context.SaveChangesAsync();
                return Ok(response);

            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost("CreateClient")]
        public async Task<IActionResult> CreateClient([FromBody] Model.Cliente newClient)
        {
            try
            {
              

                Cliente client = new Cliente();

                client.Nombre = newClient.Nombre;
                client.Apellido = newClient.Apellido;
                client.Usuario = newClient.Usuario;
                client.Dni = newClient.Dni;
                client.Contraseña = newClient.Contraseña;
                
                var response = await this._accountManager.CreateCliente(client);
                await this._context.SaveChangesAsync();

                if (response.Code.Equals(SystemEnums.ResponseCode.Error))
                {
                    return NotFound(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        [Authorize]
        [HttpPost("CreateAccount")]
        public async Task<IActionResult> CreateAccount([FromBody] Model.Cuenta cuenta)
        {
            try
            {
                string? token = GetToken();

                Cuenta newAccount = new Cuenta();

                newAccount.NumeroCuenta = cuenta.NumeroCuenta;
                newAccount.Saldo = cuenta.Saldo;
                newAccount.FechaCreacion = DateTime.Now;
                newAccount.FechaActualizacion = DateTime.Now;
                newAccount.IdCliente = cuenta.IdCliente;

                var response = await this._accountManager.CreateAccount(newAccount,token);
                await this._context.SaveChangesAsync();

                if (response.Code.Equals(SystemEnums.ResponseCode.Error))
                {
                    return NotFound(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message); 
            }
        }

        [Authorize]
        [HttpPost("AccountDeposit")]
        public async Task<IActionResult> AccountDeposit([FromBody] Model.AccountTransaction accountTransaction)
        {
            try
            {
                string? token = GetToken();
                var response = await this._accountManager.AccountDeposit(accountTransaction.amount, accountTransaction.numeroCuenta, token);
                await this._context.SaveChangesAsync();


                if (response.Code.Equals(SystemEnums.ResponseCode.Error))
                {
                    return NotFound(response);
                }

                return Ok(response);
            }
            catch(Exception ex)
            {
                return NotFound(ex.Message);
            }
        }



        [Authorize]
        [HttpPost("AccountExtract")]
        public async Task<IActionResult> AccountExtract([FromBody] Model.AccountTransaction accountTransaction)
        {
            try
            {
                string? token = GetToken();
                var response = await this._accountManager.AccountExtract(accountTransaction.amount, accountTransaction.numeroCuenta, token);
                await this._context.SaveChangesAsync();


                if (response.Code.Equals(SystemEnums.ResponseCode.Error))
                {
                    return NotFound(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        [Authorize]
        [HttpGet("GetCuentaSaldo")]
        public async Task<IActionResult> GetCuentaSaldo([FromQuery] string numeroCuenta, string nombreUsuario)
        {
            try
            {

                string token = GetToken();

                var response = await this._accountManager.GetCuentaSaldo(numeroCuenta, nombreUsuario, token);


                if (response.Code.Equals(SystemEnums.ResponseCode.Error))
                {
                    return NotFound(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("ConnectToDB")]
        public IActionResult ConnectToDB()
        {
            try
            {
                _context.Database.OpenConnection();
                return Ok("Conexion a base de datos exitosa");

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al conectar a la base de datos: {ex.Message}");
            }
            finally 
            {
                _context.Database.CloseConnection();
            }
        }

        private string? GetToken()
        {
            return Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        }
    }
}
