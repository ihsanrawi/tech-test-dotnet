using AutoFixture;
using ClearBank.DeveloperTest.Services.PaymentValidator;
using ClearBank.DeveloperTest.Types;
using Xunit;

namespace ClearBank.DeveloperTest.Tests;

public class ChapsPaymentValidatorTests
{
    private readonly Fixture _fixture = new();
    private readonly ChapsPaymentValidator _sut = new();

    [Theory]
    [InlineData(AllowedPaymentSchemes.Chaps, AccountStatus.Live, true)]
    [InlineData(AllowedPaymentSchemes.Chaps, AccountStatus.Disabled, false)]
    [InlineData(AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.FasterPayments, AccountStatus.Live, false)]
    public void IsValid_AccountAllowsFasterPaymentsPayment_ReturnsExpectedResult(AllowedPaymentSchemes allowedPaymentSchemes, AccountStatus accountStatus, bool expectedResult)
    {
        // Arrange
        var request = _fixture.Create<MakePaymentRequest>();
        
        var account = _fixture.Build<Account>()
            .With(a => a.AllowedPaymentSchemes, allowedPaymentSchemes)
            .With(prop => prop.Status, accountStatus)
            .Create();
        
        // Act
        var result = _sut.IsValid(request, account);
        
        // Assert
        Assert.Equal(expectedResult, result);
    }
}