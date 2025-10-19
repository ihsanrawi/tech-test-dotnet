using System.Collections.Generic;
using AutoFixture;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Services.PaymentValidator;
using ClearBank.DeveloperTest.Types;
using Moq;
using Xunit;

namespace ClearBank.DeveloperTest.Tests;

public class PaymentServiceTests
{
    private readonly Fixture _fixture;
    private readonly PaymentService _sut;

    public PaymentServiceTests()
    {
        _fixture = new Fixture();

        var optionsMock = new Mock<Microsoft.Extensions.Options.IOptions<PaymentSettings>>();
        optionsMock.Setup(o => o.Value).Returns(new PaymentSettings
            { DataStoreType = "Primary" });

        var dataStoreMock = new Mock<IDataStore>();
        dataStoreMock
            .Setup(ds => ds.GetAccount(It.IsAny<string>()))
            .Returns((string accountNumber) =>
                _fixture.Build<Account>()
                    .With(a => a.AccountNumber, accountNumber)
                    .With(a => a.AllowedPaymentSchemes, AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Chaps)
                    .With(a => a.Balance, 10000m)
                    .With(a => a.Status, AccountStatus.Live)
                    .Create());

        var dataStoreFactoryMock = new Mock<IDataStoreFactory>();
        dataStoreFactoryMock
            .Setup(f => f.GetDataStore(It.IsAny<DataStoreType>()))
            .Returns(dataStoreMock.Object);
        
        var paymentValidatorFactoryMock = new Mock<IPaymentValidatorFactory>();
        paymentValidatorFactoryMock
            .Setup(f => f.GetValidator(It.IsAny<PaymentScheme>()))
            .Returns((PaymentScheme scheme) =>
            {
                return scheme switch
                {
                    PaymentScheme.Bacs => new BacsPaymentValidator(),
                    PaymentScheme.FasterPayments => new FasterPaymentsPaymentValidator(),
                    PaymentScheme.Chaps => new ChapsPaymentValidator(),
                    _ => throw new KeyNotFoundException()
                };
            });
        
        _sut = new PaymentService(optionsMock.Object, dataStoreFactoryMock.Object, paymentValidatorFactoryMock.Object);
    }
    
    private PaymentService CreatePaymentService(IDataStoreFactory dataStoreFactory)
    {
        var optionsMock = new Mock<Microsoft.Extensions.Options.IOptions<PaymentSettings>>();
        optionsMock.Setup(o => o.Value).Returns(new PaymentSettings
            { DataStoreType = "Primary" });

        var paymentValidatorFactoryMock = new Mock<IPaymentValidatorFactory>();
        paymentValidatorFactoryMock
            .Setup(f => f.GetValidator(It.IsAny<PaymentScheme>()))
            .Returns((PaymentScheme scheme) =>
            {
                return scheme switch
                {
                    PaymentScheme.Bacs => new BacsPaymentValidator(),
                    PaymentScheme.FasterPayments => new FasterPaymentsPaymentValidator(),
                    PaymentScheme.Chaps => new ChapsPaymentValidator(),
                    _ => throw new KeyNotFoundException()
                };
            });

        return new PaymentService(optionsMock.Object, dataStoreFactory, paymentValidatorFactoryMock.Object);
    }
    
    [Fact]
    public void BacsPayment_ShouldFail_WhenAccountIsNull()
    {
        // Arrange
        var dataStoreMock = new Mock<IDataStore>();
        dataStoreMock.Setup(ds => ds.GetAccount(It.IsAny<string>()))
            .Returns((Account)null);  // Return null this time

        var dataStoreFactoryMock = new Mock<IDataStoreFactory>();
        dataStoreFactoryMock.Setup(f => f.GetDataStore(It.IsAny<DataStoreType>()))
            .Returns(dataStoreMock.Object);

        var sut = CreatePaymentService(dataStoreFactoryMock.Object);

        var request = _fixture.Build<MakePaymentRequest>()
            .With(prop => prop.PaymentScheme, PaymentScheme.Bacs)
            .With(prop => prop.Amount, 100)
            .Create();

        // Act
        var result = sut.MakePayment(request);

        // Assert
        Assert.False(result.Success);


    }

    [Fact]
    public void BacsPayment_ShouldFail_WhenAccountDoesNotAllowBacs()
    {
        // Arrange
        var dataStoreMock = new Mock<IDataStore>();
        dataStoreMock.Setup(ds => ds.GetAccount(It.IsAny<string>()))
            .Returns(_fixture.Build<Account>()
                .With(a => a.AllowedPaymentSchemes, AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Chaps)  // No Bacs!
                .With(a => a.Balance, 10000m)
                .With(a => a.Status, AccountStatus.Live)
                .Create());

        var dataStoreFactoryMock = new Mock<IDataStoreFactory>();
        dataStoreFactoryMock.Setup(f => f.GetDataStore(It.IsAny<DataStoreType>()))
            .Returns(dataStoreMock.Object);

        var sut = CreatePaymentService(dataStoreFactoryMock.Object);

        var request = _fixture.Build<MakePaymentRequest>()
            .With(prop => prop.PaymentScheme, PaymentScheme.Bacs)
            .With(prop => prop.Amount, 100)
            .Create();

        // Act
        var result = sut.MakePayment(request);

        // Assert
        Assert.False(result.Success);
    }

    [Fact]
    public void FasterPayments_ShouldFail_WhenInsufficientFunds()
    {
        // Arrange
        var request = _fixture.Build<MakePaymentRequest>()
            .With(prop => prop.PaymentScheme, PaymentScheme.FasterPayments)
            .With(prop => prop.Amount, 20000) // More than the default balance of 10000
            .Create();

        // Act
        var result = _sut.MakePayment(request);

        // Assert
        Assert.False(result.Success);
    }

    [Fact]
    public void ChapsPayment_ShouldFail_WhenAccountIsNotLive()
    {
        // Arrange
        var dataStoreMock = new Mock<IDataStore>();
        dataStoreMock.Setup(ds => ds.GetAccount(It.IsAny<string>()))
            .Returns(_fixture.Build<Account>()
                .With(a => a.AllowedPaymentSchemes, AllowedPaymentSchemes.Chaps)
                .With(a => a.Status, AccountStatus.Disabled)  // NOT Live!
                .With(a => a.Balance, 10000m)
                .Create());

        var dataStoreFactoryMock = new Mock<IDataStoreFactory>();
        dataStoreFactoryMock.Setup(f => f.GetDataStore(It.IsAny<DataStoreType>()))
            .Returns(dataStoreMock.Object);

        var sut = CreatePaymentService(dataStoreFactoryMock.Object);

        var request = _fixture.Build<MakePaymentRequest>()
            .With(prop => prop.PaymentScheme, PaymentScheme.Chaps)
            .With(prop => prop.Amount, 100)
            .Create();

        // Act
        var result = sut.MakePayment(request);

        // Assert
        Assert.False(result.Success);
    }

    [Fact]
    public void ValidPayment_ShouldSucceed_AndDeductAmount()
    {
        // Arrange
        var initialBalance = 10000m;
        var paymentAmount = 500m;

        Account capturedAccount = null;

        var dataStoreMock = new Mock<IDataStore>();
        dataStoreMock.Setup(ds => ds.GetAccount(It.IsAny<string>()))
            .Returns(() =>
            {
                capturedAccount = _fixture.Build<Account>()
                    .With(a => a.AllowedPaymentSchemes, AllowedPaymentSchemes.Bacs)
                    .With(a => a.Balance, initialBalance)
                    .With(a => a.Status, AccountStatus.Live)
                    .Create();
                return capturedAccount;
            });

        dataStoreMock.Setup(ds => ds.UpdateAccount(It.IsAny<Account>()))
            .Callback<Account>(acc => capturedAccount = acc);

        var dataStoreFactoryMock = new Mock<IDataStoreFactory>();
        dataStoreFactoryMock.Setup(f => f.GetDataStore(It.IsAny<DataStoreType>()))
            .Returns(dataStoreMock.Object);

        var sut = CreatePaymentService(dataStoreFactoryMock.Object);

        var request = _fixture.Build<MakePaymentRequest>()
            .With(prop => prop.PaymentScheme, PaymentScheme.Bacs)
            .With(prop => prop.Amount, paymentAmount)
            .Create();

        // Act
        var result = sut.MakePayment(request);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(capturedAccount);
        Assert.Equal(initialBalance - paymentAmount, capturedAccount.Balance);

        // Verify UpdateAccount was called
        dataStoreMock.Verify(ds => ds.UpdateAccount(It.Is<Account>(a => a.Balance == initialBalance - paymentAmount)), Times.Once);
    }
}