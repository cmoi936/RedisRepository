using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using RedisRepository.Extensions;

namespace RedisRepository.IntegrationTests.Extensions;

/// <summary>
/// Tests d'intégration pour ServiceCollectionExtensions.
/// </summary>
[Collection("Redis Integration Tests")]
public class ServiceCollectionExtensionsIntegrationTests : RedisIntegrationTestBase
{
    [Fact]
    public void AddRedisServices_WithConnectionString_ShouldRegisterServicesCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var connectionString = "localhost:6379";

        // Act
        services.AddRedisServices(connectionString);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var connectionMultiplexer = serviceProvider.GetService<IConnectionMultiplexer>();
        connectionMultiplexer.Should().NotBeNull();

        var database = serviceProvider.GetService<IDatabase>();
        database.Should().NotBeNull();

        var genericRepository = serviceProvider.GetService<RedisRepository.Repositories.IGenericRedisRepository<Extensions.ServiceCollectionExtensionsIntegrationTests.TestModel>>();
        genericRepository.Should().NotBeNull();
    }

    [Fact]
    public void AddRedisServices_WithConfigurationAction_ShouldRegisterServicesCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddRedisServices(options =>
        {
            options.EndPoints.Add("localhost", 6379);
            options.AbortOnConnectFail = false;
            options.ConnectTimeout = 5000;
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var connectionMultiplexer = serviceProvider.GetService<IConnectionMultiplexer>();
        connectionMultiplexer.Should().NotBeNull();

        var database = serviceProvider.GetService<IDatabase>();
        database.Should().NotBeNull();

        var genericRepository = serviceProvider.GetService<RedisRepository.Repositories.IGenericRedisRepository<Extensions.ServiceCollectionExtensionsIntegrationTests.TestModel>>();
        genericRepository.Should().NotBeNull();
    }

    [Fact]
    public void AddRedisServices_NullConnectionString_ShouldThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        FluentActions.Invoking(() => services.AddRedisServices((string)null!))
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AddRedisServices_EmptyConnectionString_ShouldThrowArgumentException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        FluentActions.Invoking(() => services.AddRedisServices(string.Empty))
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AddRedisServices_NullConfigurationAction_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        FluentActions.Invoking(() => services.AddRedisServices((Action<ConfigurationOptions>)null!))
            .Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task IntegratedServices_ShouldWorkWithRealRedis()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddRedisServices("localhost:6379");

        var serviceProvider = services.BuildServiceProvider();
        var repository = serviceProvider.GetRequiredService<RedisRepository.Repositories.IGenericRedisRepository<Extensions.ServiceCollectionExtensionsIntegrationTests.TestModel>>();

        var testData = new Extensions.ServiceCollectionExtensionsIntegrationTests.TestModel { Id = 1, Name = "Test", Value = "Integration Test" };
        var key = "integration-test-key";

        try
        {
            // Act
            await repository.SetAsync(key, testData);
            var retrievedData = await repository.GetAsync(key);

            // Assert
            retrievedData.Should().NotBeNull();
            retrievedData!.Id.Should().Be(testData.Id);
            retrievedData.Name.Should().Be(testData.Name);
            retrievedData.Value.Should().Be(testData.Value);
        }
        finally
        {
            // Cleanup
            await repository.DeleteAsync(key);
            serviceProvider.Dispose();
        }
    }

    /// <summary>
    /// Modèle de test simple pour vérifier la sérialisation/désérialisation.
    /// </summary>
    public class TestModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}