using Core.Dtos;
using Core.Entities;
using Core.Interfaces.Gateways;
using Core.UseCases;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CoreNamespace = global::Core;

namespace Test.Core.UseCases;

public class ClienteUseCasesTests
{
    private const string Secret = "0123456789ABCDEF0123456789ABCDEF"; //32 chars

    [Fact]
    public async Task InsertNewCliente_WhenDuplicateEmail_Throws()
    {
        var gw = new Mock<IClienteGateway>();
        gw.Setup(x => x.ExistsByEmail("e@e.com")).ReturnsAsync(true);

        var dto = new ClienteDto { Usuario = "N", Email = "e@e.com", Senha = "1234" };
        await Assert.ThrowsAsync<ArgumentException>(() => ClienteUseCases.InsertNewCliente(gw.Object, dto));
    }

    [Fact]
    public async Task InsertNewCliente_WhenValid_Inserts()
    {
        var gw = new Mock<IClienteGateway>();
        gw.Setup(x => x.GetByEmailAndSenha("e@e.com", "1234")).ReturnsAsync((Cliente?)null);
        gw.Setup(x => x.Insert(It.IsAny<ClienteDto>())).ReturnsAsync(new Cliente("N", "e@e.com", "1234") { IdUsuario = 1 });

        var dto = new ClienteDto { Usuario = "N", Email = "e@e.com", Senha = "1234" };
        var c = await ClienteUseCases.InsertNewCliente(gw.Object, dto);

        Assert.NotNull(c);
        gw.Verify(x => x.Insert(It.Is<ClienteDto>(d => d.Email == "e@e.com")), Times.Once);
    }
}
