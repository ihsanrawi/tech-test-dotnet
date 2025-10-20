namespace ClearBank.DeveloperTest.Types
{
    public class Account
    {
        public string AccountNumber { get; private set; }
        public decimal Balance { get; private set; }
        public AccountStatus Status { get; private set; }
        public AllowedPaymentSchemes AllowedPaymentSchemes { get; private set; }

        public void DeductFunds(decimal amount) => Balance -= amount;
    }
}
