using Core.Dtos;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Test.Helpers.Fakes;
using WebApi.Endpoints;

namespace Test.WebApi.Endpoints;

public class ClienteEndpointTests
{
    [Fact]
    public async Task GetById_WhenNotFound_ReturnsNotFound()
    {
        var db = new FakeDbConnection
        {
            SearchFirstOrDefaultHandler = (_, __, ___) => null
        };
        var endpoint = new ClienteEndpoint(db);

        var result = await endpoint.GetById(123);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Matches(@"Cliente n.o encontrado", notFound.Value.ToString());
    }

    [Fact]
    public async Task GetById_WhenFound_ReturnsOkWithCliente()
    {
        var expected = new Cliente { IdUsuario =123, Usuario = "Id", Email = "id@test.com" };

        var db = new FakeDbConnection
        {
            SearchFirstOrDefaultHandler = (_, __, ___) => expected
        };
        var endpoint = new ClienteEndpoint(db);

        var result = await endpoint.GetById(123);

        var ok = Assert.IsType<OkObjectResult>(result);
        var cliente = Assert.IsType<Cliente>(ok.Value);
        Assert.Equal(123, cliente.IdUsuario);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithClientes()
    {
        var expected = new[]
        {
            new Cliente { IdUsuario =1, Usuario = "A", Email = "a@a.com" },
            new Cliente { IdUsuario =2, Usuario = "B", Email = "b@b.com" },
        };

        var db = new FakeDbConnection
        {
            ListAllHandler = (_, __) => expected.Cast<object>()
        };
        var endpoint = new ClienteEndpoint(db);

        var result = await endpoint.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        var clientes = Assert.IsAssignableFrom<IEnumerable<Cliente>>(ok.Value);
        Assert.Equal(2, clientes.Count());
    }

    [Fact]
    public async Task InsertNew_WhenNullData_ThrowsArgumentException()
    {
        var db = new FakeDbConnection();
        var endpoint = new ClienteEndpoint(db);

        var dto = new ClienteDto();
        IActionResult httpResponse = await endpoint.InsertNew(dto);
        Assert.Equal(((BadRequestObjectResult)httpResponse).StatusCode, new BadRequestObjectResult("").StatusCode);
    }
}
