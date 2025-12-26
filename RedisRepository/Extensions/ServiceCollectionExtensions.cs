using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace RedisRepository.Extensions;

/// <summary>
/// Extensions pour configurer les services Redis dans l'injection de dépendances.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Ajoute les services Redis
    /// </summary>
    /// <param name="services">La collection de services.</param>
    /// <param name="connectionString">La chaîne de connexion Redis.</param>
    /// <returns>La collection de services pour permettre le chaînage.</returns>
    /// <remarks>
    /// Cette méthode configure:
    /// - La connexion Redis avec la chaîne de connexion fournie
    /// - L'enregistrement du repository générique comme service scopé
    /// - La configuration par défaut de StackExchange.Redis
    /// </remarks>
    /// <example>
    /// <code>
    /// services.AddRedisServices("localhost:6379");
    /// </code>
    /// </example>
    public static IServiceCollection AddRedisServices(this IServiceCollection services, string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("La chaîne de connexion Redis ne peut pas être null ou vide.", nameof(connectionString));

        // Configuration de la connexion Redis
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var configuration = ConfigurationOptions.Parse(connectionString);
            configuration.AbortOnConnectFail = false; // Permet la reconnexion automatique
            configuration.ConnectTimeout = 5000; // 5 secondes
            configuration.SyncTimeout = 5000; // 5 secondes

            return ConnectionMultiplexer.Connect(configuration);
        });

        // Enregistrement de la base de données Redis
        services.AddScoped<IDatabase>(provider =>
        {
            var connectionMultiplexer = provider.GetRequiredService<IConnectionMultiplexer>();
            return connectionMultiplexer.GetDatabase();
        });

        // Enregistrement du repository générique
        services.AddScoped(typeof(Repositories.IGenericRedisRepository<>), typeof(Repositories.GenericRedisRepository<>));

        return services;
    }

    /// <summary>
    /// Ajoute les services Redis avec une configuration personnalisée.
    /// </summary>
    /// <param name="services">La collection de services.</param>
    /// <param name="configureOptions">Action pour configurer les options Redis.</param>
    /// <returns>La collection de services pour permettre le chaînage.</returns>
    /// <remarks>
    /// Cette surcharge permet de configurer finement les options Redis selon les besoins spécifiques.
    /// </remarks>
    /// <example>
    /// <code>
    /// services.AddRedisServices(options =>
    /// {
    ///     options.EndPoints.Add("localhost", 6379);
    ///     options.Password = "mypassword";
    ///     options.ConnectTimeout = 10000;
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddRedisServices(this IServiceCollection services, Action<ConfigurationOptions> configureOptions)
    {
        if (configureOptions == null)
            throw new ArgumentNullException(nameof(configureOptions));

        // Configuration de la connexion Redis
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var configuration = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                ConnectTimeout = 5000,
                SyncTimeout = 5000
            };

            configureOptions(configuration);

            return ConnectionMultiplexer.Connect(configuration);
        });

        // Enregistrement de la base de données Redis
        services.AddScoped<IDatabase>(provider =>
        {
            var connectionMultiplexer = provider.GetRequiredService<IConnectionMultiplexer>();
            return connectionMultiplexer.GetDatabase();
        });

        // Enregistrement du repository générique
        services.AddScoped(typeof(Repositories.IGenericRedisRepository<>), typeof(Repositories.GenericRedisRepository<>));

        return services;
    }
}