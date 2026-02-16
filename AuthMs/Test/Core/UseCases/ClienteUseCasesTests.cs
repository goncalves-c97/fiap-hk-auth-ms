using Core.Dtos;
using Core.Entities;
using Core.Enums;
using Core.Interfaces.Gateways;
using Core.UseCases;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
    public async Task InsertNewCliente_WhenDuplicateUsuario_Throws()
    {
        var gw = new Mock<IClienteGateway>();
        gw.Setup(x => x.ExistsByEmail("e@e.com")).ReturnsAsync(false);
        gw.Setup(x => x.ExistsByUsuario("N")).ReturnsAsync(true);

        var dto = new ClienteDto { Usuario = "N", Email = "e@e.com", Senha = "1234" };
        await Assert.ThrowsAsync<ArgumentException>(() => ClienteUseCases.InsertNewCliente(gw.Object, dto));
    }

    [Fact]
    public async Task InsertNewCliente_WhenValid_Inserts()
    {
        var gw = new Mock<IClienteGateway>();
        gw.Setup(x => x.ExistsByEmail("e@e.com")).ReturnsAsync(false);
        gw.Setup(x => x.ExistsByUsuario("N")).ReturnsAsync(false);
        gw.Setup(x => x.Insert(It.IsAny<ClienteDto>()))
            .ReturnsAsync(new Cliente("N", "e@e.com", "1234") { IdUsuario =1 });

        var dto = new ClienteDto { Usuario = "N", Email = "e@e.com", Senha = "1234" };
        var c = await ClienteUseCases.InsertNewCliente(gw.Object, dto);

        Assert.NotNull(c);
        gw.Verify(x => x.Insert(It.Is<ClienteDto>(d => d.Email == "e@e.com" && d.Usuario == "N")), Times.Once);
    }

    [Fact]
    public async Task LoginCliente_WithAuthTypeEmail_WhenMissingEmailOrSenha_Throws()
    {
        var gw = new Mock<IClienteGateway>();
        await Assert.ThrowsAsync<ArgumentException>(() =>
            ClienteUseCases.LoginCliente(gw.Object, Secret, new ClienteDto { Email = null, Senha = "1234" }, AuthTypeEnum.Email));

        await Assert.ThrowsAsync<ArgumentException>(() =>
            ClienteUseCases.LoginCliente(gw.Object, Secret, new ClienteDto { Email = "e@e.com", Senha = "" }, AuthTypeEnum.Email));
    }

    [Fact]
    public async Task LoginCliente_WithAuthTypeUsuario_WhenMissingUsuarioOrSenha_Throws()
    {
        var gw = new Mock<IClienteGateway>();
        await Assert.ThrowsAsync<ArgumentException>(() =>
            ClienteUseCases.LoginCliente(gw.Object, Secret, new ClienteDto { Usuario = null, Senha = "1234" }, AuthTypeEnum.Usuario));

        await Assert.ThrowsAsync<ArgumentException>(() =>
            ClienteUseCases.LoginCliente(gw.Object, Secret, new ClienteDto { Usuario = "N", Senha = "" }, AuthTypeEnum.Usuario));
    }

    [Fact]
    public async Task LoginCliente_WhenInvalidAuthType_Throws()
    {
        var gw = new Mock<IClienteGateway>();
        var dto = new ClienteDto { Usuario = "N", Email = "e@e.com", Senha = "1234" };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            ClienteUseCases.LoginCliente(gw.Object, Secret, dto, (AuthTypeEnum)999));
    }

    [Fact]
    public async Task LoginCliente_WithAuthTypeEmail_WhenCredentialsInvalid_Throws()
    {
        var gw = new Mock<IClienteGateway>();
        gw.Setup(x => x.GetByEmailAndSenha("e@e.com", "1234")).ReturnsAsync((Cliente?)null);

        var dto = new ClienteDto { Email = "e@e.com", Senha = "1234" };
        await Assert.ThrowsAsync<ArgumentException>(() =>
            ClienteUseCases.LoginCliente(gw.Object, Secret, dto, AuthTypeEnum.Email));
    }

    [Fact]
    public async Task LoginCliente_WithAuthTypeUsuario_WhenCredentialsInvalid_Throws()
    {
        var gw = new Mock<IClienteGateway>();
        gw.Setup(x => x.GetByUsuarioAndSenha("N", "1234")).ReturnsAsync((Cliente?)null);

        var dto = new ClienteDto { Usuario = "N", Senha = "1234" };
        await Assert.ThrowsAsync<ArgumentException>(() =>
            ClienteUseCases.LoginCliente(gw.Object, Secret, dto, AuthTypeEnum.Usuario));
    }

    [Fact]
    public async Task LoginCliente_WithAuthTypeEmail_WhenValid_ReturnsJwtWithExpectedClaims()
    {
        var gw = new Mock<IClienteGateway>();
        gw.Setup(x => x.GetByEmailAndSenha("e@e.com", "1234"))
            .ReturnsAsync(new Cliente("N", "e@e.com", "1234") { IdUsuario = 10 });

        var dto = new ClienteDto { Email = "e@e.com", Senha = "1234" };
        var token = await ClienteUseCases.LoginCliente(gw.Object, Secret, dto, AuthTypeEnum.Email);

        Assert.False(string.IsNullOrWhiteSpace(token));

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);


        Assert.Contains(jwt.Claims, c =>
            (c.Type == ClaimTypes.NameIdentifier || c.Type == "nameid") &&
            c.Value == "10");

        Assert.Contains(jwt.Claims, c =>
            (c.Type == ClaimTypes.Name || c.Type == "unique_name") &&
            c.Value == "N");

        Assert.Contains(jwt.Claims, c =>
            (c.Type == ClaimTypes.Email || c.Type == JwtRegisteredClaimNames.Email) &&
            c.Value == "e@e.com");
    }

    [Fact]
    public async Task LoginCliente_WithAuthTypeUsuario_WhenValid_ReturnsJwtWithExpectedClaims()
    {
        var gw = new Mock<IClienteGateway>();
        gw.Setup(x => x.GetByUsuarioAndSenha("N", "1234"))
            .ReturnsAsync(new Cliente("N", "e@e.com", "1234") { IdUsuario =10 });

        var dto = new ClienteDto { Usuario = "N", Senha = "1234" };
        var token = await ClienteUseCases.LoginCliente(gw.Object, Secret, dto, AuthTypeEnum.Usuario);

        Assert.False(string.IsNullOrWhiteSpace(token));

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Contains(jwt.Claims, c =>
            (c.Type == ClaimTypes.NameIdentifier || c.Type == "nameid") &&
            c.Value == "10");

        Assert.Contains(jwt.Claims, c =>
            (c.Type == ClaimTypes.Name || c.Type == "unique_name") &&
            c.Value == "N");

        Assert.Contains(jwt.Claims, c =>
            (c.Type == ClaimTypes.Email || c.Type == JwtRegisteredClaimNames.Email) &&
            c.Value == "e@e.com");
    }
}
