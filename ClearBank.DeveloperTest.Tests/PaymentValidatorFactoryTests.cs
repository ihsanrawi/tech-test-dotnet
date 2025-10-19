using System;
using ClearBank.DeveloperTest.Services.PaymentValidator;
using Xunit;

namespace ClearBank.DeveloperTest.Tests;

public class PaymentValidatorFactoryTests
{
    private readonly PaymentValidatorFactory _sut;

    public PaymentValidatorFactoryTests()
    {
        var paymentValidators = new IPaymentValidator[]
        {
            new BacsPaymentValidator(),
            new FasterPaymentsPaymentValidator(),
            new ChapsPaymentValidator()
        };

        _sut = new PaymentValidatorFactory(paymentValidators);
    }
    
    [Theory]
    [InlineData(Types.PaymentScheme.Bacs, typeof(BacsPaymentValidator))]
    [InlineData(Types.PaymentScheme.FasterPayments, typeof(FasterPaymentsPaymentValidator))]
    [InlineData(Types.PaymentScheme.Chaps, typeof(ChapsPaymentValidator))]
    public void GetPaymentValidator_ShouldReturnCorrectValidator(Types.PaymentScheme paymentScheme, Type expectedValidatorType)
    {
        // Act
        var validator = _sut.GetValidator(paymentScheme);

        // Assert
        Assert.IsType(expectedValidatorType, validator);
    }
    
    [Fact]
    public void GetPaymentValidator_ShouldThrowInvalidOperationException_WhenNoValidatorFound()
    {
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _sut.GetValidator((Types.PaymentScheme)(-1)));
    }
}