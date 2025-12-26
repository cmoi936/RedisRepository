# Changelog

## [Unreleased]

### Added
- Implémentation initiale du `GenericRedisRepository<T>` pour la gestion générique de données dans Redis
- Interface `IGenericRedisRepository<T>` définissant les opérations CRUD
- Extension methods `ServiceCollectionExtensions` pour l'injection de dépendances
- Support de la sérialisation/désérialisation JSON automatique
- Gestion de l'expiration (TTL) des clés Redis
- Tests d'intégration complets avec xUnit et FluentAssertions
- Documentation complète en français (README.md)
- Pipeline GitHub Actions pour la publication automatique sur NuGet.org
- Licence MIT

### Features
- **Opérations CRUD** : SetAsync, GetAsync, ExistsAsync, DeleteAsync
- **Gestion TTL** : ExpireAsync, TimeToLiveAsync
- **Configuration flexible** : Support de chaînes de connexion et d'options avancées
- **Logging intégré** : Utilisation de Microsoft.Extensions.Logging
- **Récupération d'erreurs** : Nettoyage automatique des données corrompues
- **Préfixes de clés** : Support de préfixes personnalisables pour organiser les données

### Dependencies
- StackExchange.Redis 2.10.1
- Microsoft.Extensions.Logging.Abstractions
- Microsoft.Extensions.DependencyInjection.Abstractions

### Target Framework
- .NET 8.0

---

## Format du Changelog

Ce fichier suit le format de [Keep a Changelog](https://keepachangelog.com/fr/1.0.0/),
et ce projet adhère au [Semantic Versioning](https://semver.org/lang/fr/).

### Types de changements
- **Added** : Nouvelles fonctionnalités
- **Changed** : Modifications de fonctionnalités existantes
- **Deprecated** : Fonctionnalités bientôt supprimées
- **Removed** : Fonctionnalités supprimées
- **Fixed** : Corrections de bugs
- **Security** : Corrections de vulnérabilités de sécurité
