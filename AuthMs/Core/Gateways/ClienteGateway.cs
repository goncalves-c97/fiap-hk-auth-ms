using Core.Dtos;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Gateways;
using Microsoft.IdentityModel.Tokens;

namespace Core.Gateways
{
    public class ClienteGateway(IDbConnection dbConnection) : IClienteGateway
    {
        private readonly IDbConnection _dbConnection = dbConnection;
        private const string _tableName = nameof(Cliente);

        public async Task<IEnumerable<Cliente>> GetAll()
        {
            return await _dbConnection.ListAllAsync<Cliente>(nameof(Cliente));
        }

        public async Task<Cliente> Insert(ClienteDto cliente)
        {
            int registeredId = await _dbConnection.InsertAndReturnIdAsync(_tableName, new Dictionary<string, object>
            {
                { "usuario", cliente.Usuario },
                { "email", cliente.Email },
                { "senha", cliente.Senha },
				{ "guid", Guid.NewGuid() }
            }, "id_usuario");

            return await GetById(registeredId);
        }

        public async Task<Cliente?> GetById(int IdUsuario)
        {
            return await _dbConnection.SearchFirstOrDefaultByParametersAsync<Cliente>(
                _tableName,
                "id_usuario = @Id",
                new { Id = IdUsuario }
            );
        }

        public async Task DeleteAll()
        {
            await _dbConnection.DeleteAsync(_tableName, "1=1");
        }

        public async Task<Cliente?> GetByEmailAndSenha(string email, string senha)
        {
            return await _dbConnection.SearchFirstOrDefaultByParametersAsync<Cliente>(
                _tableName,
                "email = @Email AND senha = @Senha",
                new { Email = email, Senha = senha }
            );
        }

        public async Task<Cliente?> GetByUsuarioAndSenha(string usuario, string senha)
        {
            return await _dbConnection.SearchFirstOrDefaultByParametersAsync<Cliente>(
                _tableName,
                "usuario = @Usuario AND senha = @Senha",
                new { Usuario = usuario, Senha = senha }
            );
        }

        public async Task<bool> ExistsByEmail(string email)
        {
            object? cliente = await _dbConnection.SearchFirstOrDefaultByParametersAsync<object?>(
                _tableName,
                "email = @Email",
                new { Email = email }
            );

            return cliente != null;
        }

        public async Task<bool> ExistsByUsuario(string usuario)
        {
            object? cliente = await _dbConnection.SearchFirstOrDefaultByParametersAsync<object?>(
                _tableName,
                "usuario = @Usuario",
                new { Usuario = usuario }
            );

            return cliente  != null;
        }
    }
}