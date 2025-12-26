# Tests d'Intégration Redis avec Testcontainers

Ce projet utilise **Testcontainers for .NET** pour gérer automatiquement les conteneurs Redis nécessaires aux tests d'intégration.

## 🚀 Avantages de Testcontainers

- ✅ **Démarrage automatique** : Le conteneur Redis se crée et démarre automatiquement
- ✅ **Isolation complète** : Chaque session de test a son propre conteneur
- ✅ **Nettoyage automatique** : Les conteneurs sont supprimés après les tests
- ✅ **Pas de configuration manuelle** : Aucun script à exécuter manuellement
- ✅ **CI/CD ready** : Fonctionne parfaitement dans les pipelines d'intégration continue

## 📋 Prérequis

### Docker Desktop

Testcontainers nécessite que Docker soit installé et en cours d'exécution :

1. **Installez Docker Desktop** : https://www.docker.com/products/docker-desktop
2. **Démarrez Docker Desktop** avant d'exécuter les tests
3. **Vérifiez l'installation** :
   ```bash
   docker --version
   docker ps
   ```

### Package NuGet

Le package est déjà installé dans le projet :
```
Testcontainers.Redis 4.9.0
```

## 🏗️ Architecture des Tests

### Option 1 : Classe de Base Standard (`RedisIntegrationTestBase`)

Chaque classe de test crée son propre conteneur Redis.

**Avantages** :
- Isolation maximale entre les classes de tests
- Pas de risque de collision de données

**Inconvénients** :
- Plus lent (création d'un conteneur par classe de test)

**Utilisation** :
```csharp
public class MyRedisTests : RedisIntegrationTestBase
{
    [Fact]
    public async Task MyTest()
    {
        // Le conteneur Redis est déjà démarré et prêt
        await Database.StringSetAsync("key", "value");
        // ...
    }
}
```

### Option 2 : Fixture Partagée (`SharedRedisIntegrationTestBase`) ⭐ Recommandé

Tous les tests d'une collection partagent le même conteneur Redis.

**Avantages** :
- Beaucoup plus rapide (un seul conteneur pour toute la collection)
- Économie de ressources

**Inconvénients** :
- Nécessite un nettoyage entre les tests (fait automatiquement)

**Utilisation** :
```csharp
// L'attribut [Collection] est déjà inclus dans la classe de base
public class MyRedisTests : SharedRedisIntegrationTestBase
{
    public MyRedisTests(RedisContainerFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task MyTest()
    {
        // Le conteneur Redis est partagé et prêt
        await Database.StringSetAsync("key", "value");
        // La base de données est nettoyée automatiquement après le test
    }
}
```

## 🔄 Migration des Tests Existants

### Depuis l'ancienne approche (scripts PowerShell)

**Avant** :
```csharp
public class MyTests : IAsyncLifetime
{
    // Configuration manuelle de Redis...
}
```

**Après (Option Rapide)** :
```csharp
public class MyTests : SharedRedisIntegrationTestBase
{
    public MyTests(RedisContainerFixture fixture) : base(fixture)
    {
    }
    
    // Vos tests existants fonctionnent sans changement!
}
```

## 📊 Exemple Complet

```csharp
using AutoFixture;
using FluentAssertions;
using RedisRepository.Repositories;

namespace RedisRepository.IntegrationTests.Repositories;

public class GenericRedisRepositoryIntegrationTests : SharedRedisIntegrationTestBase
{
    private readonly IGenericRedisRepository<TestModel> _repository;
    private readonly IFixture _fixture;

    public GenericRedisRepositoryIntegrationTests(RedisContainerFixture fixture) 
        : base(fixture)
    {
        _fixture = new Fixture();
        _repository = ServiceProvider
            .GetRequiredService<IGenericRedisRepository<TestModel>>();
    }

    [Fact]
    public async Task SetAsync_ValidModel_ShouldStoreSuccessfully()
    {
        // Arrange
        var key = GenerateTestKey(); // Nettoyage automatique
        var model = _fixture.Create<TestModel>();

        // Act
        await _repository.SetAsync(key, model);

        // Assert
        var exists = await _repository.ExistsAsync(key);
        exists.Should().BeTrue();
    }
}
```

## 🎯 Exécution des Tests

### Visual Studio
1. Assurez-vous que Docker Desktop est démarré
2. Ouvrez le Test Explorer
3. Exécutez les tests normalement
4. Les conteneurs Redis se créent automatiquement

### CLI
```bash
# Démarrer Docker Desktop d'abord

# Exécuter tous les tests
dotnet test

# Exécuter avec logs détaillés
dotnet test --logger "console;verbosity=detailed"

# Exécuter une classe de test spécifique
dotnet test --filter FullyQualifiedName~GenericRedisRepositoryIntegrationTests
```

### GitHub Actions / Azure DevOps

Testcontainers fonctionne automatiquement dans les pipelines CI/CD :

```yaml
# GitHub Actions
jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
      - name: Run tests
        run: dotnet test
        # Docker est déjà disponible dans les runners GitHub Actions
```

## 🔍 Dépannage

### Docker n'est pas en cours d'exécution

**Erreur** :
```
Docker is not running or not accessible
```

**Solution** :
1. Démarrez Docker Desktop
2. Attendez que l'icône Docker soit verte
3. Réessayez les tests

### Timeout lors du démarrage du conteneur

**Erreur** :
```
Container did not reach desired state within timeout
```

**Solutions** :
- La première fois, Docker doit télécharger l'image Redis (quelques minutes)
- Vérifiez votre connexion Internet
- Vérifiez les ressources disponibles pour Docker Desktop (CPU/RAM)

### Ports déjà utilisés

**Erreur** :
```
Bind for 0.0.0.0:6379 failed
```

**Solution** :
- Testcontainers utilise des ports aléatoires, ce problème ne devrait pas arriver
- Vérifiez qu'aucun autre conteneur Redis n'est en cours
- Exécutez : `docker ps` et arrêtez les conteneurs inutiles

### Voir les conteneurs actifs

```bash
# Lister les conteneurs de test en cours
docker ps --filter "name=redis-test"

# Voir les logs d'un conteneur
docker logs <container-id>

# Nettoyer manuellement (si nécessaire)
docker container prune
```

## 📚 Configuration Avancée

### Personnaliser l'image Redis

```csharp
_redisContainer = new RedisBuilder()
    .WithImage("redis:7.2-alpine")  // Version spécifique
    .WithName($"redis-test-{Guid.NewGuid():N}")
    .WithCleanUp(true)
    .Build();
```

### Ajouter des paramètres Redis

```csharp
_redisContainer = new RedisBuilder()
    .WithImage("redis:7-alpine")
    .WithCommand("--requirepass", "mypassword")  // Mot de passe
    .WithCommand("--maxmemory", "256mb")         // Limite mémoire
    .Build();
```

### Conserver les conteneurs pour débogage

Modifiez `.WithCleanUp(false)` pour garder les conteneurs après les tests.

⚠️ N'oubliez pas de les nettoyer manuellement : `docker container prune`

## 🔗 Ressources

- [Testcontainers for .NET](https://dotnet.testcontainers.org/)
- [Documentation Redis](https://redis.io/documentation)
- [xUnit Documentation](https://xunit.net/)
- [GitHub Testcontainers](https://github.com/testcontainers/testcontainers-dotnet)

## 💡 Bonnes Pratiques

1. **Utilisez `SharedRedisIntegrationTestBase`** pour de meilleures performances
2. **Utilisez `GenerateTestKey()`** pour des clés uniques avec nettoyage automatique
3. **Nettoyez explicitement les données** si vous testez des scénarios de collision
4. **Isolez les tests** : Chaque test doit être indépendant
5. **Utilisez des assertions explicites** avec FluentAssertions

## 🗂️ Structure du Projet

```
RedisRepository.IntegrationTests/
├── RedisIntegrationTestBase.cs           # Classe de base (un conteneur par classe)
├── SharedRedisIntegrationTestBase.cs     # Classe de base (conteneur partagé) ⭐
├── RedisContainerFixture.cs              # Fixture xUnit pour partage
├── Repositories/
│   └── GenericRedisRepositoryIntegrationTests.cs
└── TESTCONTAINERS.md                     # Cette documentation
```

## ❓ FAQ

**Q: Dois-je installer Redis localement ?**
R: Non ! Testcontainers gère tout automatiquement.

**Q: Puis-je utiliser ces tests en CI/CD ?**
R: Oui ! Aucune configuration supplémentaire nécessaire.

**Q: Les tests sont lents ?**
R: Utilisez `SharedRedisIntegrationTestBase` pour partager le conteneur.

**Q: Comment déboguer un test avec le conteneur ?**
R: Mettez un point d'arrêt et inspectez `_redisContainer.GetConnectionString()`.

**Q: Puis-je avoir plusieurs conteneurs Redis ?**
R: Oui ! Créez plusieurs fixtures avec des noms différents.
