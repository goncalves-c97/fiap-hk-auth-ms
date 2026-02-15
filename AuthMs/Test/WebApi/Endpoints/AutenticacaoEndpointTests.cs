using System.IdentityModel.Tokens.Jwt;
using Core.Dtos;
using Core.Enums;
using Core.Helpers;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Test.Helpers.Fakes;
using WebApi.Endpoints;

namespace Test.WebApi.Endpoints;

public class AutenticacaoEndpointTests
{
    private const string Secret = "0123456789ABCDEF0123456789ABCDEF";

    [Fact]
    public void Ctor_WhenMissingSecret_Throws()
    {
        var db = new Moq.Mock<IDbConnection>().Object;
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();

        Assert.Throws<ArgumentException>(() => new AutenticacaoEndpoint(db, config));
    }

    [Fact]
    public async Task LoginClienteWithAuthTypeUsuario_WhenValid_ReturnsOkWithToken()
    {
        var db = new FakeDbConnection();
        db.SearchFirstOrDefaultHandler = (table, where, param) =>
        {
            if (table == "Cliente" && where == "usuario = @Usuario AND senha = @Senha")
                return new global::Core.Entities.Cliente { IdUsuario = 10, Senha = "1234", Email = "e@e.com", Usuario = "N" };

            return null;
        };

        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "API_AUTHENTICATION_KEY", Secret }
        }).Build();

        var endpoint = new AutenticacaoEndpoint(db, config);

        var result = await endpoint.LoginCliente(new ClienteDto { Usuario = "N", Senha = "1234" }, AuthTypeEnum.Usuario);

        var ok = Assert.IsType<OkObjectResult>(result);
        var tokenValue = ok.Value?.GetType().GetProperty("Token")?.GetValue(ok.Value) as string;
        Assert.False(string.IsNullOrWhiteSpace(tokenValue));
        _ = new JwtSecurityTokenHandler().ReadJwtToken(tokenValue);
    }

    [Fact]
    public async Task LoginClienteWithAuthTypeEmail_WhenValid_ReturnsOkWithToken()
    {
        var db = new FakeDbConnection();
        db.SearchFirstOrDefaultHandler = (table, where, param) =>
        {
            if (table == "Cliente" && where == "email = @Email AND senha = @Senha")
                return new global::Core.Entities.Cliente { IdUsuario = 10, Senha = "1234", Email = "e@e.com", Usuario = "N" };

            return null;
        };

        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "API_AUTHENTICATION_KEY", Secret }
        }).Build();

        var endpoint = new AutenticacaoEndpoint(db, config);

        var result = await endpoint.LoginCliente(new ClienteDto { Email = "e@e.com", Senha = "1234" }, AuthTypeEnum.Email);

        var ok = Assert.IsType<OkObjectResult>(result);
        var tokenValue = ok.Value?.GetType().GetProperty("Token")?.GetValue(ok.Value) as string;
        Assert.False(string.IsNullOrWhiteSpace(tokenValue));
        _ = new JwtSecurityTokenHandler().ReadJwtToken(tokenValue);
    }
}
