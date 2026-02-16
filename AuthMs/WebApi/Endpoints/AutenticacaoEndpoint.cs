using Core.Controllers;
using Core.Dtos;
using Core.Enums;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebApi.Endpoints
{
    [AllowAnonymous]
    [ApiController]
    [Route("Autenticacao")]
    public class AutenticacaoEndpoint : ControllerBase
    {
        private readonly IDbConnection _dbConnection;
        private readonly string _jwtSecret;

        public AutenticacaoEndpoint(IDbConnection dbConnection, IConfiguration configuration)
        {
            _dbConnection = dbConnection;

            string? jwtSecret = configuration["API_AUTHENTICATION_KEY"];

            if (string.IsNullOrEmpty(jwtSecret))
                throw new ArgumentException("A chave de autenticação da API não está configurada no appsettings.json.");

            _jwtSecret = jwtSecret;
        }

        [HttpPost, Route("LoginCliente")]
        public async Task<IActionResult> LoginCliente([FromBody] ClienteDto clienteDto, [FromHeader] AuthTypeEnum authType)
        {
            try
            {
                string token = await ClienteController.LoginCliente(_dbConnection, _jwtSecret, clienteDto, authType);

                if (string.IsNullOrEmpty(token))
                    return Unauthorized();
                else
                    return Ok(new { Token = token });
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
