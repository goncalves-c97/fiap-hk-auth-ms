using Core.Dtos;
using Core.Entities;
using Core.Enums;
using Core.Gateways;
using Core.Interfaces;
using Core.UseCases;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Controllers
{
    public static class ClienteController
    {

        public static async Task<string> LoginCliente(IDbConnection dbConnection, string secret, ClienteDto clienteDto, AuthTypeEnum authType)
        {
            ClienteGateway gateway = new(dbConnection);
            string token = await ClienteUseCases.LoginCliente(gateway, secret, clienteDto, authType);
            return token;
        }

        public static async Task<IEnumerable<Cliente>> GetAll(IDbConnection dbConnection)
        {
            ClienteGateway gateway = new(dbConnection);
            IEnumerable<Cliente> clientes = await ClienteUseCases.GetAllClientes(gateway);
            return clientes;
        }

        public static async Task<Cliente?> InsertNewCliente(IDbConnection dbConnection, ClienteDto clienteDto)
        {
            ClienteGateway gateway = new(dbConnection);
            Cliente? cliente = await ClienteUseCases.InsertNewCliente(gateway, clienteDto);
            return cliente;
        }

        public static async Task DeleteAll(IDbConnection dbConnection)
        {
            ClienteGateway gateway = new(dbConnection);
            await ClienteUseCases.DeleteAll(gateway);
        }

        public static async Task<Cliente?> GetById(IDbConnection dbConnection, int id)
        {
            ClienteGateway gateway = new(dbConnection);
            return await ClienteUseCases.GetById(gateway, id);
        }
    }
}
