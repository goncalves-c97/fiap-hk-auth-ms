using Core.Dtos;
using Core.Entities;

namespace Core.Interfaces.Gateways
{
    public interface IClienteGateway
    {
        public Task<IEnumerable<Cliente>> GetAll();
        public Task<Cliente> Insert(ClienteDto cliente);
        public Task<Cliente?> GetById(int IdUsuario);
        public Task<Cliente?> GetByEmailAndSenha(string email, string senha);
        public Task<Cliente?> GetByUsuarioAndSenha(string usuario, string senha);
        public Task<bool> ExistsByEmail(string email);
        public Task<bool> ExistsByUsuario(string usuario);
        public Task DeleteAll();
    }
}
