# Dal.Redis

## 📋 Vue d'ensemble

Le projet **Dal.Redis** fournit une couche d'accès aux données utilisant Redis comme système de cache pour la gestion temporaire des recettes par thread de conversation.

## 🎯 Responsabilités

- **Stockage temporaire** : Sauvegarde des recettes générées pour un thread de conversation spécifique
- **Récupération rapide** : Accès optimisé aux recettes par identifiant de thread
- **Gestion de l'expiration** : Nettoyage automatique des données expirées
- **Sérialisation JSON** : Conversion bidirectionnelle des objets RecetteDto

## 🏗️ Architecture

### Abstractions
- **`IRecetteRedisRepository`** : Interface définissant les opérations CRUD pour les recettes dans Redis

### Implémentations
- **`RecetteRedisRepository`** : Implémentation Redis avec gestion des erreurs et logging

### Extensions
- **`ServiceCollectionExtensions`** : Configuration de l'injection de dépendances pour Redis

## 🚀 Utilisation

### Configuration dans Program.cs ou Startup.cs

```csharp
// Configuration simple avec chaîne de connexion
services.AddRedisRecetteServices("localhost:6379");

// Configuration avancée
services.AddRedisRecetteServices(options =>
{
    options.EndPoints.Add("localhost", 6379);
    options.Password = "mypassword";
    options.ConnectTimeout = 10000;
    options.AbortOnConnectFail = false;
});
```

### Injection et utilisation

```csharp
public class MonService
{
    private readonly IRecetteRedisRepository _recetteRepository;

    public MonService(IRecetteRedisRepository recetteRepository)
    {
        _recetteRepository = recetteRepository;
    }

    public async Task ExempleUtilisation()
    {
        var threadId = "conversation-123";
        
        // Sauvegarder des recettes
        var recettes = new List<RecetteDto> { /* vos recettes */ };
        await _recetteRepository.SaveRecettesAsync(threadId, recettes);
        
        // Récupérer des recettes
        var recettesRecuperees = await _recetteRepository.GetRecettesAsync(threadId);
        
        // Ajouter une recette
        await _recetteRepository.AddRecetteAsync(threadId, nouvelleRecette);
        
        // Supprimer toutes les recettes d'un thread
        await _recetteRepository.DeleteRecettesAsync(threadId);
    }
}
```

## ⚙️ Configuration Redis

### Paramètres par défaut
- **Expiration** : 24 heures
- **Timeout de connexion** : 5 secondes  
- **Timeout de synchronisation** : 5 secondes
- **Reconnexion automatique** : Activée

### Format des clés Redis
```
recettes:thread:{threadId}
```

## 🔧 Fonctionnalités

### Gestion des erreurs
- Validation des paramètres d'entrée
- Gestion des erreurs de sérialisation/désérialisation
- Logging détaillé des opérations et erreurs
- Nettoyage automatique des données corrompues

### Performance
- Sérialisation JSON optimisée
- Clés Redis structurées pour un accès rapide
- Expiration automatique des données pour économiser la mémoire

### Logging
- Logs de debug pour les opérations normales
- Logs d'erreur avec contexte détaillé
- Statistiques sur le nombre de recettes traitées

## 📦 Dépendances

- **StackExchange.Redis** (2.8.16) : Client Redis performant
- **System.Text.Json** (9.0.9) : Sérialisation JSON native .NET
- **Microsoft.Extensions.Logging.Abstractions** (9.0.0) : Interface de logging