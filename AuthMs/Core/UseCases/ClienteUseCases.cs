using Core.Dtos;
using Core.Entities;
using Core.Enums;
using Core.Gateways;
using Core.Helpers;
using Core.Interfaces.Gateways;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Core.UseCases
{
    public static class ClienteUseCases
    {
        private static async Task<string> GenerateClienteToken(string secret, Cliente cliente)
        {
            JwtSecurityTokenHandler tokenHandler = new();

            byte[] key = Encoding.ASCII.GetBytes(secret);

            var claims = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, cliente.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, cliente.Usuario),
                new Claim(ClaimTypes.Email, cliente.Email)
            ]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(12),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return await Task.FromResult(tokenHandler.WriteToken(token));
        }

        private static async Task<string> LoginByEmail(IClienteGateway clienteGateway, string secret, ClienteDto clienteDto)
        {
            Cliente? cliente = await clienteGateway.GetByEmailAndSenha(clienteDto.Email, clienteDto.Senha);

            if (cliente == null)
                throw new ArgumentException("Email ou senha inválidos!");

            return await GenerateClienteToken(secret, cliente);
        }

        private static async Task<string> LoginByUsuario(IClienteGateway clienteGateway, string secret, ClienteDto clienteDto)
        {
            Cliente? cliente = await clienteGateway.GetByUsuarioAndSenha(clienteDto.Usuario, clienteDto.Senha);

            if (cliente == null)
                throw new ArgumentException("Usuario ou senha inválidos!");

            return await GenerateClienteToken(secret, cliente);
        }

        public static async Task<string> LoginCliente(IClienteGateway clienteGateway, string secret, ClienteDto clienteDto, AuthTypeEnum authTypeEnum)
        {
            if (authTypeEnum == AuthTypeEnum.Email && (string.IsNullOrEmpty(clienteDto.Email) || string.IsNullOrEmpty(clienteDto.Senha)))
                throw new ArgumentException("Email e senha devem ser informados para login!");
            else if (authTypeEnum == AuthTypeEnum.Usuario && (string.IsNullOrEmpty(clienteDto.Usuario) || string.IsNullOrEmpty(clienteDto.Senha)))
                throw new ArgumentException("Usuário e senha devem ser informados para login!");

            return authTypeEnum switch
            {
                AuthTypeEnum.Email => await LoginByEmail(clienteGateway, secret, clienteDto),
                AuthTypeEnum.Usuario => await LoginByUsuario(clienteGateway, secret, clienteDto),
                _ => throw new ArgumentException("Tipo de autenticação inválida!"),
            };
        }
        
        public static async Task<IEnumerable<Cliente>> GetAllClientes(IClienteGateway clienteGateway)
        {
            return await clienteGateway.GetAll();
        }

        public static async Task<Cliente?> InsertNewCliente(IClienteGateway clienteGateway, ClienteDto clienteDto)
        {

            Cliente cliente = new(clienteDto.Usuario, clienteDto.Email, clienteDto.Senha);

            cliente.ValidateValueObjects();

            if (!cliente.IsValid)
                throw new ArgumentException("Cliente inválido: " + cliente.Errors.Summary);

            if (await clienteGateway.ExistsByEmail(clienteDto.Email))
                throw new ArgumentException("Email já cadastrado no sistema!");

            if (await clienteGateway.ExistsByUsuario(clienteDto.Usuario))
                throw new ArgumentException("Usuário já cadastrado no sistema!");

            return await clienteGateway.Insert(clienteDto);
        }

        public static async Task DeleteAll(IClienteGateway clienteGateway)
        {
            await clienteGateway.DeleteAll();
        }

        public static async Task<Cliente?> GetById(ClienteGateway gateway, int id)
        {
            if (id <= 0)
                throw new ArgumentException("id não informado ou inválido!");

            return await gateway.GetById(id);
        }
	}
}