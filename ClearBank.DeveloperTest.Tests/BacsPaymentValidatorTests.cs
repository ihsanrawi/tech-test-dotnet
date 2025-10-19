using AutoFixture;
using ClearBank.DeveloperTest.Services.PaymentValidator;
using ClearBank.DeveloperTest.Types;
using Xunit;

namespace ClearBank.DeveloperTest.Tests;

public class BacsPaymentValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly BacsPaymentValidator _sut = new();

    [Theory]
    [InlineData(AllowedPaymentSchemes.FasterPayments, false)]
    [InlineData(AllowedPaymentSchemes.Bacs, true)]
    [InlineData(AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps, true)]
    public void IsValid_AccountAllowsBacsPayment_ReturnsExpectedResult(AllowedPaymentSchemes allowedPaymentSchemes, bool expectedResult)
    {
        // Arrange
        var request = _fixture.Create<MakePaymentRequest>();
        
        var account = _fixture.Build<Account>()
            .With(a => a.AllowedPaymentSchemes, allowedPaymentSchemes)
            .Create();
        
        // Act
        var result = _sut.IsValid(request, account);
        
        // Assert
        Assert.Equal(expectedResult, result);
    }
}