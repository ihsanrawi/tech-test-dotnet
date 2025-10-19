using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.PaymentValidator;

public class BacsPaymentValidator : IPaymentValidator
{
    public PaymentScheme PaymentScheme => PaymentScheme.Bacs;

    public bool IsValid(MakePaymentRequest request, Account account)
    {
        return account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs);
    }
}