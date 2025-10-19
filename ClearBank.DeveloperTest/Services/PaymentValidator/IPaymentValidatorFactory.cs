using System;
using System.Collections.Generic;
using System.Linq;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.PaymentValidator;

public interface IPaymentValidatorFactory
{
    IPaymentValidator GetValidator(PaymentScheme scheme);
}

public class PaymentValidatorFactory : IPaymentValidatorFactory
{
    private readonly Dictionary<PaymentScheme, IPaymentValidator> _validators;

    public PaymentValidatorFactory(IEnumerable<IPaymentValidator> validators)
    {
        _validators = validators.ToDictionary(paymentValidator => paymentValidator.PaymentScheme, v => v);
    }

    public IPaymentValidator GetValidator(PaymentScheme scheme)
    {
        if (_validators.TryGetValue(scheme, out var validator))
            return validator;

        throw new InvalidOperationException($"No validator found for scheme {scheme}");
    }
}
