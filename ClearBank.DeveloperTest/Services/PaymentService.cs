using System;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services.PaymentValidator;
using ClearBank.DeveloperTest.Types;
using Microsoft.Extensions.Options;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService(IOptions<PaymentSettings> options, IDataStoreFactory dataStoreFactory, IPaymentValidatorFactory validatorFactory)
        : IPaymentService
    {
        private readonly string _dataStoreType = options.Value.DataStoreType;

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            if (_dataStoreType == null)
            {
                return new MakePaymentResult(){ ErrorMessage = "DataStoreType not found in configuration" };
            }

            if (!Enum.TryParse(_dataStoreType, out DataStoreType dataStoreEnum))
            {
                return new MakePaymentResult(){ ErrorMessage = "Invalid DataStoreType in configuration" };
            }
            
            var account = RetrieveAccount(dataStoreEnum, request.DebtorAccountNumber);
            if (account == null)
            {
                return new MakePaymentResult(){ErrorMessage = "Account not found"};
            }

            var result = new MakePaymentResult();
            
            var paymentValidator = validatorFactory.GetValidator(request.PaymentScheme);
            result.Success = paymentValidator.IsValid(request, account);

            if (!result.Success)
            {
                return result;
            }

            account.Balance -= request.Amount;
            UpdateAccount(dataStoreEnum, account);

            return result;
        }
        
        private Account RetrieveAccount(DataStoreType dataStoreEnum, string debtorAccountNumber)
        {
            var dataStore = dataStoreFactory.GetDataStore(dataStoreEnum);
            return dataStore.GetAccount(debtorAccountNumber);
        }
        
        private void UpdateAccount(DataStoreType dataStoreEnum, Account account)
        {
            try
            {
                var dataStore = dataStoreFactory.GetDataStore(dataStoreEnum);
                dataStore.UpdateAccount(account);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
