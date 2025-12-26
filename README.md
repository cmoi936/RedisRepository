# RedisRepository

[![NuGet](https://img.shields.io/nuget/v/RedisRepository.svg)](https://www.nuget.org/packages/RedisRepository/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Implémentation générique d'un repository Redis pour .NET, permettant la gestion de données de tout type avec support de sérialisation JSON, expiration et opérations CRUD optimisées.

## 🚀 Fonctionnalités

- **Repository générique** : Fonctionne avec n'importe quel type de classe
- **Sérialisation JSON** : Sérialisation/désérialisation automatique
- **Gestion d'expiration** : Support TTL (Time To Live) configurable
- **Opérations CRUD complètes** : Set, Get, Delete, Exists
- **Logging intégré** : Support de Microsoft.Extensions.Logging
- **Gestion d'erreurs** : Récupération automatique des données corrompues
- **Performance optimisée** : Utilisation de StackExchange.Redis

## 📦 Installation

```bash
dotnet add package RedisRepository
```

## 🔧 Configuration

### Enregistrement dans le conteneur DI

```csharp
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using RedisRepository.Repositories;

services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    return ConnectionMultiplexer.Connect("localhost:6379");
});

services.AddSingleton(sp =>
{
    var redis = sp.GetRequiredService<IConnectionMultiplexer>();
    return redis.GetDatabase();
});

services.AddScoped<IGenericRedisRepository<MyModel>, GenericRedisRepository<MyModel>>();
```

## 💡 Utilisation

### Exemple basique

```csharp
public class MyModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Value { get; set; }
}

// Injection du repository
public class MyService
{
    private readonly IGenericRedisRepository<MyModel> _repository;

    public MyService(IGenericRedisRepository<MyModel> repository)
    {
        _repository = repository;
    }

    public async Task Example()
    {
        var model = new MyModel 
        { 
            Id = "123", 
            Name = "Test", 
            Value = 42 
        };

        // Sauvegarder avec expiration de 1 heure
        await _repository.SetAsync("key1", model, TimeSpan.FromHours(1));

        // Récupérer les données
        var retrieved = await _repository.GetAsync("key1");

        // Vérifier l'existence
        bool exists = await _repository.ExistsAsync("key1");

        // Obtenir le TTL
        TimeSpan? ttl = await _repository.TimeToLiveAsync("key1");

        // Modifier l'expiration
        await _repository.ExpireAsync("key1", TimeSpan.FromMinutes(30));

        // Supprimer
        await _repository.DeleteAsync("key1");
    }
}
```

### Préfixe de clé personnalisé

```csharp
var repository = new GenericRedisRepository<MyModel>(
    database, 
    logger, 
    keyPrefix: "myapp:models:"
);
```

## 🛠️ API

### Méthodes disponibles

| Méthode | Description |
|---------|-------------|
| `SetAsync(key, data, expiration?, ct)` | Sauvegarde des données avec expiration optionnelle |
| `GetAsync(key, ct)` | Récupère les données pour une clé |
| `ExistsAsync(key, ct)` | Vérifie l'existence d'une clé |
| `DeleteAsync(key, ct)` | Supprime une clé |
| `ExpireAsync(key, expiration, ct)` | Modifie l'expiration d'une clé |
| `TimeToLiveAsync(key, ct)` | Récupère le TTL d'une clé |

## 📋 Prérequis

- .NET 8.0 ou supérieur
- Redis 5.0 ou supérieur
- StackExchange.Redis 2.10.1 ou supérieur

## 🤝 Contribution

Les contributions sont les bienvenues ! N'hésitez pas à ouvrir une issue ou une pull request.

## 📄 Licence

Ce projet est sous licence MIT - voir le fichier [LICENSE](LICENSE) pour plus de détails.

## 🔗 Liens utiles

- [Documentation Redis](https://redis.io/documentation)
- [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis)
- [GitHub Repository](https://github.com/cmoi936/RedisRepository)
