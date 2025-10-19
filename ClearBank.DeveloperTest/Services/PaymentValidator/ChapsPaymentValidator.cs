using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.PaymentValidator;

public class ChapsPaymentValidator : IPaymentValidator
{
    public PaymentScheme PaymentScheme => PaymentScheme.Chaps;

    public bool IsValid(MakePaymentRequest request, Account account)
    {
        return account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps)
               && account.Status == AccountStatus.Live;
    }
}