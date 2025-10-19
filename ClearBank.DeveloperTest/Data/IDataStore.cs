using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Data;

public interface IDataStore
{
    DataStoreType DataStoreType { get; }
    public Account GetAccount(string accountNumber);
    public void UpdateAccount(Account account);
}