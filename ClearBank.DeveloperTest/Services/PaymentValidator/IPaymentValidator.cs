using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.PaymentValidator;

public interface IPaymentValidator
{
    PaymentScheme PaymentScheme { get; }
    bool IsValid(MakePaymentRequest request, Account account);
}