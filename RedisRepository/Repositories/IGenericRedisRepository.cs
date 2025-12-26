namespace RedisRepository.Repositories;

/// <summary>
/// Repository générique pour la gestion de données de tout type dans Redis.
/// </summary>
/// <typeparam name="T">Le type de données à stocker et récupérer.</typeparam>
/// <remarks>
/// Ce repository fournit une interface générique pour les opérations CRUD avec Redis,
/// permettant de stocker et récupérer n'importe quel type de données sérialisables.
/// </remarks>
public interface IGenericRedisRepository<T> where T : class
{
    /// <summary>
    /// Sauvegarde un objet pour une clé spécifique.
    /// </summary>
    /// <param name="key">La clé Redis pour identifier les données.</param>
    /// <param name="data">L'objet à sauvegarder.</param>
    /// <param name="expiration">Durée de vie des données dans Redis. Par défaut: 24 heures.</param>
    /// <param name="ct">Jeton d'annulation pour annuler l'opération.</param>
    /// <returns>Une tâche représentant l'opération asynchrone.</returns>
    Task SetAsync(string key, T data, TimeSpan? expiration = null, CancellationToken ct = default);

    /// <summary>
    /// Récupère un objet associé à une clé spécifique.
    /// </summary>
    /// <param name="key">La clé Redis pour identifier les données.</param>
    /// <param name="ct">Jeton d'annulation pour annuler l'opération.</param>
    /// <returns>
    /// L'objet associé à la clé, ou null si aucun objet n'est trouvé ou si les données ont expiré.
    /// </returns>
    Task<T?> GetAsync(string key, CancellationToken ct = default);

    /// <summary>
    /// Vérifie si une clé existe dans Redis.
    /// </summary>
    /// <param name="key">La clé Redis à vérifier.</param>
    /// <param name="ct">Jeton d'annulation pour annuler l'opération.</param>
    /// <returns>True si la clé existe, False sinon.</returns>
    Task<bool> ExistsAsync(string key, CancellationToken ct = default);

    /// <summary>
    /// Supprime les données associées à une clé spécifique.
    /// </summary>
    /// <param name="key">La clé Redis des données à supprimer.</param>
    /// <param name="ct">Jeton d'annulation pour annuler l'opération.</param>
    /// <returns>
    /// True si les données ont été supprimées avec succès,
    /// False si aucune donnée n'existait pour cette clé.
    /// </returns>
    Task<bool> DeleteAsync(string key, CancellationToken ct = default);

    /// <summary>
    /// Définit une expiration sur une clé existante.
    /// </summary>
    /// <param name="key">La clé Redis sur laquelle définir l'expiration.</param>
    /// <param name="expiration">La durée d'expiration.</param>
    /// <param name="ct">Jeton d'annulation pour annuler l'opération.</param>
    /// <returns>True si l'expiration a été définie, False si la clé n'existe pas.</returns>
    Task<bool> ExpireAsync(string key, TimeSpan expiration, CancellationToken ct = default);

    /// <summary>
    /// Récupère le temps restant avant expiration d'une clé.
    /// </summary>
    /// <param name="key">La clé Redis à vérifier.</param>
    /// <param name="ct">Jeton d'annulation pour annuler l'opération.</param>
    /// <returns>
    /// Le temps restant avant expiration, ou null si la clé n'existe pas 
    /// ou n'a pas d'expiration définie.
    /// </returns>
    Task<TimeSpan?> TimeToLiveAsync(string key, CancellationToken ct = default);
}