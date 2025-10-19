using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.PaymentValidator;

public class FasterPaymentsPaymentValidator : IPaymentValidator
{
    public PaymentScheme PaymentScheme => PaymentScheme.FasterPayments;

    public bool IsValid(MakePaymentRequest request, Account account)
    {
        return account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments)
               && account.Balance >= request.Amount;
    }
}