using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RedisRepository.Repositories;

/// <summary>
/// Implémentation générique Redis du repository pour la gestion de données de tout type.
/// </summary>
/// <typeparam name="T">Le type de données à stocker et récupérer.</typeparam>
/// <remarks>
/// Cette classe utilise Redis comme stockage pour maintenir n'importe quel type de données
/// sérialisables. Elle fournit des opérations CRUD optimisées pour les performances et la scalabilité.
/// </remarks>
public class GenericRedisRepository<T> : Repositories.IGenericRedisRepository<T> where T : class
{
    private readonly IDatabase _database;
    private readonly ILogger<Repositories.GenericRedisRepository<T>> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly string _keyPrefix;
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromHours(24);

    /// <summary>
    /// Initialise une nouvelle instance du repository Redis générique.
    /// </summary>
    /// <param name="database">L'instance de base de données Redis.</param>
    /// <param name="logger">Le logger pour enregistrer les opérations.</param>
    /// <param name="keyPrefix">Préfixe optionnel pour les clés Redis. Par défaut basé sur le nom du type.</param>
    public GenericRedisRepository(IDatabase database, ILogger<Repositories.GenericRedisRepository<T>> logger, string? keyPrefix = null)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _keyPrefix = keyPrefix ?? $"{typeof(T).Name.ToLowerInvariant()}:";

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    /// <inheritdoc />
    [Description("Sauvegarde des données de type T dans Redis avec une clé unique et une expiration optionnelle.")]
    public async Task SetAsync(
        [Description("Identifiant de la donnée")] string key,
        [Description("Objet à sauvegarder en Redis")] T data,
        [Description("Durée de vie des données. Par défaut 24 heures")] TimeSpan? expiration = null,
        [Description("Jeton d'annulation pour interrompre l'opération")] CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("La clé ne peut pas être null ou vide.", nameof(key));

        if (data == null)
            throw new ArgumentNullException(nameof(data));

        try
        {
            var redisKey = GetRedisKey(key);
            var jsonData = JsonSerializer.Serialize(data, _jsonOptions);
            var expirationTime = expiration ?? DefaultExpiration;

            await _database.StringSetAsync(redisKey, jsonData, expirationTime);

            _logger.LogDebug("Sauvegarde de données de type {Type} pour la clé {Key} avec expiration {Expiration}",
                typeof(T).Name, key, expirationTime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la sauvegarde des données de type {Type} pour la clé {Key}",
                typeof(T).Name, key);
            throw;
        }
    }

    /// <inheritdoc />
    [Description("Récupère les données de type T stockées dans Redis pour une clé donnée.")]
    public async Task<T?> GetAsync(
        [Description("Identifiant de la donnée à récupérer")] string key,
        [Description("Jeton d'annulation pour interrompre l'opération")] CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("La clé ne peut pas être null ou vide.", nameof(key));

        try
        {
            var redisKey = GetRedisKey(key);
            var jsonData = await _database.StringGetAsync(redisKey);

            if (!jsonData.HasValue)
            {
                _logger.LogDebug("Aucune donnée de type {Type} trouvée pour la clé {Key}", typeof(T).Name, key);
                return null;
            }

            var data = JsonSerializer.Deserialize<T>((string)jsonData!, _jsonOptions);

            _logger.LogDebug("Récupération des données de type {Type} pour la clé {Key}", typeof(T).Name, key);

            return data;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Erreur de désérialisation des données de type {Type} pour la clé {Key}", typeof(T).Name, key);
            // En cas d'erreur de désérialisation, on supprime la clé corrompue
            await _database.KeyDeleteAsync(GetRedisKey(key));
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des données de type {Type} pour la clé {Key}",
                typeof(T).Name, key);
            throw;
        }
    }

    /// <inheritdoc />
    [Description("Vérifie l'existence d'une clé dans Redis.")]
    public async Task<bool> ExistsAsync(
        [Description("Identifiant de la donnée à vérifier")] string key,
        [Description("Jeton d'annulation pour interrompre l'opération")] CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("La clé ne peut pas être null ou vide.", nameof(key));

        try
        {
            var redisKey = GetRedisKey(key);
            var exists = await _database.KeyExistsAsync(redisKey);

            _logger.LogDebug("Vérification de l'existence de la clé {Key} pour le type {Type}: {Exists}",
                key, typeof(T).Name, exists);

            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la vérification de l'existence de la clé {Key} pour le type {Type}",
                key, typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    [Description("Supprime définitivement une donnée de Redis.")]
    public async Task<bool> DeleteAsync(
        [Description("Identifiant de la donnée à supprimer")] string key,
        [Description("Jeton d'annulation pour interrompre l'opération")] CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("La clé ne peut pas être null ou vide.", nameof(key));

        try
        {
            var redisKey = GetRedisKey(key);
            var deleted = await _database.KeyDeleteAsync(redisKey);

            if (deleted)
            {
                _logger.LogDebug("Suppression des données de type {Type} pour la clé {Key}", typeof(T).Name, key);
            }
            else
            {
                _logger.LogDebug("Aucune donnée de type {Type} à supprimer pour la clé {Key}", typeof(T).Name, key);
            }

            return deleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la suppression des données de type {Type} pour la clé {Key}",
                typeof(T).Name, key);
            throw;
        }
    }

    /// <inheritdoc />
    [Description("Définit ou modifie la durée d'expiration d'une clé existante dans Redis.")]
    public async Task<bool> ExpireAsync(
        [Description("Identifiant de la donnée dont l'expiration doit être modifiée")] string key,
        [Description("Nouvelle durée avant expiration")] TimeSpan expiration,
        [Description("Jeton d'annulation pour interrompre l'opération")] CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("La clé ne peut pas être null ou vide.", nameof(key));

        try
        {
            var redisKey = GetRedisKey(key);
            var success = await _database.KeyExpireAsync(redisKey, expiration);

            if (success)
            {
                _logger.LogDebug("Définition de l'expiration {Expiration} pour la clé {Key} de type {Type}",
                    expiration, key, typeof(T).Name);
            }
            else
            {
                _logger.LogDebug("Impossible de définir l'expiration pour la clé {Key} de type {Type} (clé inexistante)",
                    key, typeof(T).Name);
            }

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la définition de l'expiration pour la clé {Key} de type {Type}",
                key, typeof(T).Name);
            throw;
        }
    }

    /// <inheritdoc />
    [Description("Récupère le temps restant avant expiration d'une clé dans Redis.")]
    public async Task<TimeSpan?> TimeToLiveAsync(
        [Description("Identifiant de la donnée dont le TTL est à vérifier")] string key,
        [Description("Jeton d'annulation pour interrompre l'opération")] CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("La clé ne peut pas être null ou vide.", nameof(key));

        try
        {
            var redisKey = GetRedisKey(key);
            var ttl = await _database.KeyTimeToLiveAsync(redisKey);

            _logger.LogDebug("Temps de vie restant pour la clé {Key} de type {Type}: {Ttl}",
                key, typeof(T).Name, ttl);

            return ttl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération du temps de vie pour la clé {Key} de type {Type}",
                key, typeof(T).Name);
            throw;
        }
    }

    /// <summary>
    /// Génère la clé Redis complète avec le préfixe pour une clé donnée.
    /// </summary>
    /// <param name="key">La clé de base.</param>
    /// <returns>La clé Redis formatée avec préfixe.</returns>
    private string GetRedisKey(string key) => $"{_keyPrefix}{key}";
}