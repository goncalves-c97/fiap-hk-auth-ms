using Core.Interfaces.Gateways;

namespace Core.Interfaces
{
    public interface IUnitOfWork
    {
        public IClienteGateway ClienteRepository { get; }

    }
}
