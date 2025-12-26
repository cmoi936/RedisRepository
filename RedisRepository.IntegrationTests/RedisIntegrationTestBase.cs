using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using RedisRepository.Extensions;
using Testcontainers.Redis;

namespace RedisRepository.IntegrationTests;

/// <summary>
/// Classe de base pour les tests d'intégration Redis fournissant la configuration commune et l'infrastructure de test.
/// Utilise Testcontainers pour démarrer automatiquement une instance Redis pour chaque session de tests.
/// </summary>
public abstract class RedisIntegrationTestBase : IAsyncLifetime
{
    protected IServiceProvider ServiceProvider { get; private set; } = null!;
    protected IConnectionMultiplexer ConnectionMultiplexer { get; private set; } = null!;
    protected IDatabase Database { get; private set; } = null!;
    
    private RedisContainer _redisContainer = null!;
    private readonly List<string> _keysToCleanup = new();

    private const int TestDatabaseIndex = 15; // Utilise une base de données dédiée aux tests

    public virtual async Task InitializeAsync()
    {
        // Création et démarrage du conteneur Redis avec Testcontainers
        _redisContainer = new RedisBuilder()
            .WithImage("redis:7-alpine")
            .WithName($"redis-test-{Guid.NewGuid():N}")
            .WithCleanUp(true)
            .Build();

        await _redisContainer.StartAsync();

        // Configuration des services
        var services = new ServiceCollection();

        // Configuration du logging pour les tests
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));

        // Configuration Redis avec la chaîne de connexion du conteneur Testcontainers
        var connectionString = _redisContainer.GetConnectionString();
        services.AddRedisServices(connectionString);

        ServiceProvider = services.BuildServiceProvider();

        // Récupération des services Redis
        ConnectionMultiplexer = ServiceProvider.GetRequiredService<IConnectionMultiplexer>();

        // Utilisation d'une base de données dédiée aux tests
        Database = ConnectionMultiplexer.GetDatabase(TestDatabaseIndex);

        // Vérification de la connexion Redis
        try
        {
            await Database.PingAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Impossible de se connecter au conteneur Redis Testcontainers. " +
                $"Connection string: {connectionString}", ex);
        }
    }

    public virtual async Task DisposeAsync()
    {
        // Nettoyage sélectif des clés créées pendant les tests
        if (Database != null && _keysToCleanup.Count > 0)
        {
            try
            {
                var keysArray = _keysToCleanup.Select(k => (RedisKey)k).ToArray();
                await Database.KeyDeleteAsync(keysArray);
            }
            catch (Exception ex)
            {
                // Log l'erreur mais ne pas faire échouer le test
                Console.WriteLine($"Erreur lors du nettoyage des clés Redis: {ex.Message}");
            }
        }

        // Libération des ressources
        ConnectionMultiplexer?.Dispose();
        
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }

        // Arrêt et suppression du conteneur Redis
        if (_redisContainer != null)
        {
            await _redisContainer.StopAsync();
            await _redisContainer.DisposeAsync();
        }
    }

    /// <summary>
    /// Enregistre une clé pour nettoyage automatique à la fin du test.
    /// </summary>
    protected void RegisterKeyForCleanup(string key)
    {
        _keysToCleanup.Add(key);
    }

    /// <summary>
    /// Génère un identifiant de thread unique pour les tests.
    /// </summary>
    protected static string GenerateTestThreadId()
    {
        return $"test-thread-{Guid.NewGuid():N}";
    }

    /// <summary>
    /// Génère une clé unique pour les tests avec nettoyage automatique.
    /// </summary>
    protected string GenerateTestKey(string prefix = "test")
    {
        var key = $"{prefix}:{Guid.NewGuid():N}";
        RegisterKeyForCleanup(key);
        return key;
    }
}