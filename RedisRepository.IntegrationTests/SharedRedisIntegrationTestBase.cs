using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace RedisRepository.IntegrationTests;

/// <summary>
/// Classe de base pour les tests d'intégration Redis qui utilisent une fixture partagée.
/// Cette approche est plus performante car elle réutilise le même conteneur Redis pour tous les tests d'une collection.
/// </summary>
/// <remarks>
/// Pour utiliser cette classe, ajoutez l'attribut [Collection(nameof(RedisCollection))] à votre classe de test.
/// </remarks>
[Collection(nameof(RedisCollection))]
public abstract class SharedRedisIntegrationTestBase : IAsyncLifetime
{
    protected IServiceProvider ServiceProvider { get; private set; } = null!;
    protected IConnectionMultiplexer ConnectionMultiplexer { get; private set; } = null!;
    protected IDatabase Database { get; private set; } = null!;

    private readonly RedisContainerFixture _fixture;
    private readonly List<string> _keysToCleanup = new();
    
    private const int TestDatabaseIndex = 15; // Utilise une base de données dédiée aux tests

    /// <summary>
    /// Initialise une nouvelle instance de la classe SharedRedisIntegrationTestBase.
    /// </summary>
    /// <param name="fixture">La fixture Redis partagée fournie par xUnit.</param>
    protected SharedRedisIntegrationTestBase(RedisContainerFixture fixture)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
    }

    public virtual async Task InitializeAsync()
    {
        // Utilisation du ServiceProvider de la fixture
        ServiceProvider = _fixture.CreateServiceProvider();

        // Récupération des services Redis
        ConnectionMultiplexer = ServiceProvider.GetRequiredService<IConnectionMultiplexer>();

        // Utilisation d'une base de données dédiée aux tests
        Database = ConnectionMultiplexer.GetDatabase(TestDatabaseIndex);

        // Vérification de la connexion
        await Database.PingAsync();

        // Nettoyage de la base de données avant chaque test pour isoler les tests
        await Database.ExecuteAsync("FLUSHDB");
    }

    public virtual async Task DisposeAsync()
    {
        // Nettoyage des clés créées pendant le test
        if (Database != null && _keysToCleanup.Count > 0)
        {
            try
            {
                var keysArray = _keysToCleanup.Select(k => (RedisKey)k).ToArray();
                await Database.KeyDeleteAsync(keysArray);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du nettoyage des clés Redis: {ex.Message}");
            }
        }

        // Libération des ressources (mais pas du conteneur Redis qui est partagé)
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
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
