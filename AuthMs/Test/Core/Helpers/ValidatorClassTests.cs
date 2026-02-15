using Core.Helpers;

namespace Test.Core.Helpers;

public class ValidatorClassTests
{
    private sealed class DummyValidator : ValidatorClass
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public byte[]? Token { get; set; }

        protected override void Validate()
        {
            IdValidation(Id, nameof(Id), validateZero: true);
            NotEmptyStringValidation(nameof(Name), Name);
            ByteArraySizeValidation(nameof(Token), Token, expectedSize: 4);
        }
    }

    [Fact]
    public void Errors_Summary_ContainsRegisteredErrors()
    {
        var v = new DummyValidator { Id = -1, Name = null, Token = null };
        v.ValidateValueObjectsForTests();

        Assert.False(v.IsValid);
        Assert.Contains(GenericErrors.NegativeIdError.ToString(), v.Errors.Summary);
        Assert.Contains(GenericErrors.EmptyStringError.ToString(), v.Errors.Summary);
        Assert.Contains(GenericErrors.ByteArraySizeError.ToString(), v.Errors.Summary);
    }

    [Fact]
    public void ContainsError_WhenErrorWasRegistered_ReturnsTrue()
    {
        var v = new DummyValidator { Id = 0, Name = "ok", Token = new byte[] { 1, 2, 3, 4 } };
        v.ValidateValueObjectsForTests();

        Assert.False(v.IsValid);
        Assert.True(v.ContainsError(GenericErrors.IdZeroError, nameof(DummyValidator.Id)));
    }

    [Fact]
    public void ByteArraySizeValidation_WhenWrongSize_RegistersError()
    {
        var v = new DummyValidator { Id = 1, Name = "ok", Token = new byte[] { 1, 2, 3 } };
        v.ValidateValueObjectsForTests();

        Assert.False(v.IsValid);
        Assert.Contains(GenericErrors.ByteArraySizeError.ToString(), v.Errors.Summary);
    }

    [Fact]
    public void Error_PropertiesNamesSummary_ReturnsCommaSeparatedNames()
    {
        var e = new Error(GenericErrors.InvalidObjectError, "msg", "A", "B");
        Assert.Equal("A, B", e.PropertiesNamesSummary);
    }
}

internal static class ValidatorExtensions
{
    public static void ValidateValueObjectsForTests(this ValidatorClass validator)
    {
        // force Validate() execution by invoking a protected workflow via reflection
        var method = validator.GetType().GetMethod("Validate", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        method!.Invoke(validator, null);
    }
}
