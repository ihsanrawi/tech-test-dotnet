using System;
using System.Collections.Generic;
using System.Linq;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Data;

public class DataStoreFactory : IDataStoreFactory
{
    private readonly Dictionary<DataStoreType, IDataStore> _dataStores;

    public DataStoreFactory(IEnumerable<IDataStore> dataStores)
    {
        _dataStores = dataStores.ToDictionary(dataStore => dataStore.DataStoreType, v => v);
    }

    public IDataStore GetDataStore(DataStoreType dataStoreType)
    {
        if (_dataStores.TryGetValue(dataStoreType, out var dataStore))
            return dataStore;

        throw new ArgumentOutOfRangeException(nameof(dataStoreType), $"Not expected data store type value: {dataStoreType}");
    }
}