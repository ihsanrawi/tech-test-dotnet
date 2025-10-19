using AutoFixture;
using ClearBank.DeveloperTest.Services.PaymentValidator;
using ClearBank.DeveloperTest.Types;
using Xunit;

namespace ClearBank.DeveloperTest.Tests;

public class FasterPaymentsPaymentValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly FasterPaymentsPaymentValidator _sut = new();

    [Theory]
    [InlineData(AllowedPaymentSchemes.FasterPayments, 10000, true)]
    [InlineData(AllowedPaymentSchemes.FasterPayments, 500, false)]
    [InlineData(AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.Chaps, 1500, false)]
    public void IsValid_AccountAllowsFasterPaymentsPayment_ReturnsExpectedResult(AllowedPaymentSchemes allowedPaymentSchemes, decimal accountBalance, bool expectedResult)
    {
        // Arrange
        var request = _fixture.Build<MakePaymentRequest>()
            .With(a => a.Amount, 1000m)
            .Create();
        
        var account = _fixture.Build<Account>()
            .With(a => a.AllowedPaymentSchemes, allowedPaymentSchemes)
            .With(prop => prop.Balance, accountBalance)
            .Create();
        
        // Act
        var result = _sut.IsValid(request, account);
        
        // Assert
        Assert.Equal(expectedResult, result);
    }
}