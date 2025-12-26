using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using RedisRepository.Extensions;
using Testcontainers.Redis;

namespace RedisRepository.IntegrationTests;

/// <summary>
/// Fixture xUnit pour partager un conteneur Redis entre plusieurs tests.
/// Cela améliore les performances en évitant de créer/détruire un conteneur pour chaque test.
/// </summary>
public class RedisContainerFixture : IAsyncLifetime
{
    private RedisContainer? _redisContainer;
    
    /// <summary>
    /// Obtient le conteneur Redis partagé.
    /// </summary>
    public RedisContainer RedisContainer => _redisContainer 
        ?? throw new InvalidOperationException("Le conteneur Redis n'est pas initialisé.");

    /// <summary>
    /// Obtient la chaîne de connexion du conteneur Redis.
    /// </summary>
    public string ConnectionString => RedisContainer.GetConnectionString();

    /// <summary>
    /// Crée un ServiceProvider configuré avec Redis.
    /// </summary>
    public IServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
        services.AddRedisServices(ConnectionString);
        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Initialise le conteneur Redis au début de la collection de tests.
    /// </summary>
    public async Task InitializeAsync()
    {
        _redisContainer = new RedisBuilder()
            .WithImage("redis:7-alpine")
            .WithName($"redis-test-shared-{Guid.NewGuid():N}")
            .WithCleanUp(true)
            .Build();

        await _redisContainer.StartAsync();

        // Vérification que Redis est prêt
        var connectionString = _redisContainer.GetConnectionString();
        using var connection = await ConnectionMultiplexer.ConnectAsync(connectionString);
        var db = connection.GetDatabase();
        await db.PingAsync();
    }

    /// <summary>
    /// Arrête et supprime le conteneur Redis à la fin de la collection de tests.
    /// </summary>
    public async Task DisposeAsync()
    {
        if (_redisContainer != null)
        {
            await _redisContainer.StopAsync();
            await _redisContainer.DisposeAsync();
        }
    }
}

/// <summary>
/// Définit une collection de tests qui partagera le même conteneur Redis.
/// </summary>
[CollectionDefinition(nameof(RedisCollection))]
public class RedisCollection : ICollectionFixture<RedisContainerFixture>
{
    // Cette classe n'a pas de code, elle sert juste de marqueur pour xUnit
    // pour identifier quels tests doivent partager la même fixture
}
