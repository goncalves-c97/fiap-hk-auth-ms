using Core.Entities;
using Core.Helpers;

namespace Test.Core.Entities;

public class ClienteTests
{
    [Fact]
    public void Ctor_WhenValid_SetsPropertiesAndIsValid()
    {
        var c = new Cliente("Nome", "email@a.com", "123");
        Assert.True(c.IsValid);
        Assert.Equal("Nome", c.Usuario);
    }

    [Fact]
    public void Validate_WhenMissingFields_RegisterErrors()
    {
        var c = new Cliente { Usuario = null, Email = null, Senha = null };
        c.ValidateValueObjects();

        Assert.False(c.IsValid);
        Assert.True(c.ContainsError(GenericErrors.EmptyStringError, nameof(Cliente.Usuario)));
        Assert.True(c.ContainsError(GenericErrors.EmptyStringError, nameof(Cliente.Email)));
        Assert.True(c.ContainsError(GenericErrors.EmptyStringError, nameof(Cliente.Senha)));
    }
}
