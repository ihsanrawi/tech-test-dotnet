using System;
using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;
using Xunit;

namespace ClearBank.DeveloperTest.Tests;

public class DataStoreFactoryTests
{
    private readonly DataStoreFactory _sut;

    public DataStoreFactoryTests()
    {
        var dataStores = new IDataStore[]
        {
            new PrimaryAccountDataStore(),
            new BackupAccountDataStore()
        };

        _sut = new DataStoreFactory(dataStores);
    }
    
    [Theory]
    [InlineData(DataStoreType.Backup, typeof(BackupAccountDataStore))]
    [InlineData(DataStoreType.Primary, typeof(PrimaryAccountDataStore))]
    public void GetDataStore_ShouldReturnPrimaryAccountDataStore_WhenDataStoreTypeIsPrimary(DataStoreType dataStoreType, Type expectedType)
    {
        // Act
        var result = _sut.GetDataStore(dataStoreType);

        // Assert
        Assert.IsType(expectedType, result);
    }
    
    [Fact]
    public void GetDataStore_ShouldThrowArgumentOutOfRangeException_WhenDataStoreTypeIsNotInEnum()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _sut.GetDataStore(DataStoreType.Backup - 1));
    }
}