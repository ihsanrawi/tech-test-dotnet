using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Data;

public interface IDataStoreFactory
{
    IDataStore GetDataStore(DataStoreType dataStoreType);
}