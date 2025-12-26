# Dal.Redis Integration Tests

## 📋 Vue d'ensemble

Ce projet contient les tests d'intégration pour le module **Dal.Redis**, permettant de valider le bon fonctionnement des repositories Redis avec une instance Redis réelle.

## 🎯 Objectifs

- **Tests d'intégration complets** : Validation des fonctionnalités avec une instance Redis locale
- **Tests de performance** : Vérification des temps de réponse et de la scalabilité
- **Tests de robustesse** : Validation de la gestion des erreurs et des cas limites
- **Tests de configuration** : Vérification de l'injection de dépendances et de la configuration

## 🧪 Structure des tests

### Classes de test

#### `RedisIntegrationTestBase`
- **Classe de base** : Fournit l'infrastructure commune pour tous les tests d'intégration
- **Configuration Redis** : Connexion à une base de données dédiée aux tests (index 15)
- **Nettoyage automatique** : Suppression des données de test après chaque execution
- **Données de test** : Génération de recettes et d'identifiants de thread pour les tests

#### `GenericRedisRepositoryIntegrationTests`
- **Tests CRUD complets** : SetAsync, GetAsync, ExistsAsync, DeleteAsync
- **Gestion de l'expiration** : ExpireAsync, TimeToLiveAsync
- **Validation des erreurs** : Tests avec paramètres invalides
- **Sérialisation complexe** : Tests avec des objets RecetteDto complets

#### `RecetteRedisRepositoryIntegrationTests`
- **Gestion des recettes par thread** : SaveRecettesAsync, GetRecettesAsync
- **Ajout de recettes** : AddRecetteAsync
- **Suppression** : DeleteRecettesAsync
- **Workflows complets** : Tests de scénarios métier complets
- **TTL et expiration** : Tests de persistance temporaire

#### `ServiceCollectionExtensionsIntegrationTests`
- **Configuration d'injection de dépendances** : Tests des méthodes d'extension
- **Validation des paramètres** : Tests avec configurations invalides
- **Tests d'intégration complète** : Validation de la stack entière

## 🔧 Prérequis

### Redis Local
Pour exécuter ces tests, vous devez avoir Redis installé et en cours d'exécution sur votre machine locale :

```bash
# Installation avec Chocolatey (Windows)
choco install redis-64

# Démarrage du service Redis
redis-server

# Vérification que Redis fonctionne
redis-cli ping
# Devrait retourner "PONG"
```

### Configuration Redis pour les tests
- **Host** : localhost
- **Port** : 6379
- **Base de données** : Index 15 (dédiée aux tests)
- **Authentification** : Aucune (configuration par défaut)

## 🚀 Exécution des tests

### Via Visual Studio
1. Ouvrir l'Explorateur de tests (Test Explorer)
2. Cliquer sur "Exécuter tous les tests"
3. Les tests d'intégration seront exécutés avec Redis

### Via ligne de commande
```bash
# Depuis la racine du projet
dotnet test src/Dal.Redis.IntegrationTests/

# Avec plus de détails
dotnet test src/Dal.Redis.IntegrationTests/ --logger "console;verbosity=detailed"

# Exécution d'une seule classe de test
dotnet test src/Dal.Redis.IntegrationTests/ --filter "FullyQualifiedName~GenericRedisRepositoryIntegrationTests"
```

## 📊 Couverture des tests

### Fonctionnalités testées
- ✅ **Opérations CRUD** : Create, Read, Update, Delete
- ✅ **Gestion de l'expiration** : TTL, expiration automatique
- ✅ **Sérialisation/Désérialisation** : JSON avec objets complexes
- ✅ **Gestion des erreurs** : Paramètres invalides, connexion Redis
- ✅ **Configuration DI** : Injection de dépendances
- ✅ **Workflows métier** : Scénarios complets d'utilisation

### Cas de test couverts
- **Cas nominaux** : Fonctionnement normal avec données valides
- **Cas d'erreur** : Paramètres null, clés inexistantes, expiration
- **Cas limites** : Données complexes, gros volumes, expiration courte
- **Configuration** : Différentes options de configuration Redis

## 🐛 Dépannage

### Redis non disponible
Si les tests échouent avec une erreur de connexion Redis :
1. Vérifier que Redis est installé et démarré
2. Tester la connexion : `redis-cli ping`
3. Vérifier le port 6379
4. Vérifier les permissions firewall

### Tests lents
Si les tests sont lents :
1. Vérifier la latence réseau vers Redis
2. Optimiser la configuration Redis
3. Réduire les timeouts dans les tests

### Nettoyage des données
Les tests nettoient automatiquement la base de données 15 après chaque exécution. Si nécessaire, nettoyage manuel :
```bash
redis-cli -n 15 FLUSHDB
```

## 📈 Métriques et performance

Les tests d'intégration permettent de mesurer :
- **Temps de réponse** : Latence des opérations Redis
- **Throughput** : Nombre d'opérations par seconde
- **Utilisation mémoire** : Footprint des données en Redis
- **Stabilité** : Comportement sous charge et avec erreurs

## 🔄 CI/CD

Ces tests d'intégration peuvent être intégrés dans une pipeline CI/CD avec :
1. **Redis dans un conteneur** : Docker Redis pour les environnements de build
2. **Tests conditionnels** : Exécution uniquement si Redis est disponible
3. **Rapports de test** : Export des résultats pour monitoring

```yaml
# Exemple GitHub Actions
services:
  redis:
    image: redis:7-alpine
    ports:
      - 6379:6379
```