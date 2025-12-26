# Redis Docker pour Tests d'Intégration

Ce dossier contient la configuration Docker nécessaire pour exécuter une instance Redis dédiée aux tests d'intégration.

## Prérequis

- Docker Desktop installé et en cours d'exécution
- Docker Compose (inclus avec Docker Desktop)

## Installation de Docker

Si vous n'avez pas encore Docker Desktop :
1. Téléchargez Docker Desktop : https://www.docker.com/products/docker-desktop
2. Installez et démarrez Docker Desktop
3. Attendez que Docker soit complètement démarré (icône dans la barre système)

## Utilisation Rapide

### Démarrer Redis

```powershell
.\Start-RedisDocker.ps1
```

Ce script va :
- Vérifier que Docker est installé et en cours d'exécution
- Démarrer un conteneur Redis sur le port 6379
- Attendre que Redis soit prêt à accepter des connexions
- Afficher les informations de connexion

### Arrêter Redis

```powershell
.\Stop-RedisDocker.ps1
```

### Utilisation Manuelle (Alternative)

Si vous préférez utiliser directement docker-compose :

```bash
# Démarrer Redis
docker-compose up -d

# Arrêter Redis
docker-compose down

# Voir les logs
docker logs redis-integration-tests

# Accéder au CLI Redis
docker exec -it redis-integration-tests redis-cli
```

## Configuration Redis

L'instance Redis est configurée avec :
- **Image** : redis:7-alpine (légère et performante)
- **Port** : 6379 (port par défaut)
- **Bases de données** : 16 (0-15)
- **Persistance** : Activée (AOF - Append Only File)
- **Healthcheck** : Vérifie automatiquement que Redis répond

## Informations de Connexion

```
Host: localhost
Port: 6379
Database Index (Tests): 15
```

## Tests d'Intégration

Les tests d'intégration utilisent la base de données **15** pour éviter les conflits avec d'autres données.

### Avant d'exécuter les tests

1. Assurez-vous que Redis est démarré :
   ```powershell
   .\Start-RedisDocker.ps1
   ```

2. Exécutez vos tests :
   ```bash
   dotnet test
   ```

### Dépannage

#### Docker n'est pas en cours d'exécution
```
Error: Cannot connect to the Docker daemon
```
**Solution** : Démarrez Docker Desktop et attendez qu'il soit prêt.

#### Port 6379 déjà utilisé
```
Error: Bind for 0.0.0.0:6379 failed: port is already allocated
```
**Solution** : 
1. Arrêtez l'autre instance Redis en cours d'exécution
2. Ou modifiez le port dans `docker-compose.yml`

#### Timeout de connexion dans les tests
```
RedisConnectionException: The message timed out
```
**Solution** : 
1. Vérifiez que Redis est démarré : `.\Start-RedisDocker.ps1`
2. Vérifiez que Redis répond : `docker exec redis-integration-tests redis-cli ping`
3. Attendez quelques secondes après le démarrage

## Nettoyage

Pour supprimer complètement l'instance Redis et ses données :

```bash
docker-compose down -v
```

Le flag `-v` supprime également le volume de données persistantes.

## Structure des Fichiers

```
.
├── docker-compose.yml          # Configuration Docker Compose
├── Start-RedisDocker.ps1       # Script de démarrage (Windows)
├── Stop-RedisDocker.ps1        # Script d'arrêt (Windows)
└── REDIS-DOCKER.md            # Cette documentation
```

## Commandes Utiles

```bash
# Voir l'état du conteneur
docker ps -a | grep redis-integration-tests

# Voir les logs en temps réel
docker logs -f redis-integration-tests

# Obtenir des informations sur Redis
docker exec redis-integration-tests redis-cli INFO

# Vider toutes les données (attention !)
docker exec redis-integration-tests redis-cli FLUSHALL

# Redémarrer le conteneur
docker restart redis-integration-tests
```

## Support Multi-Plateforme

### Linux/macOS

Pour Linux ou macOS, créez un script bash équivalent :

```bash
#!/bin/bash
# start-redis-docker.sh

echo "🚀 Démarrage de Redis..."
docker-compose up -d

echo "⏳ Attente que Redis soit prêt..."
until docker exec redis-integration-tests redis-cli ping 2>/dev/null; do
    sleep 1
done

echo "✓ Redis est prêt!"
```

Rendez-le exécutable : `chmod +x start-redis-docker.sh`

## CI/CD

Pour intégrer Redis dans votre pipeline CI/CD, ajoutez simplement :

```yaml
# Exemple GitHub Actions
services:
  redis:
    image: redis:7-alpine
    ports:
      - 6379:6379
    options: >-
      --health-cmd "redis-cli ping"
      --health-interval 10s
      --health-timeout 5s
      --health-retries 5
```

## Ressources

- [Documentation Redis](https://redis.io/documentation)
- [Documentation Docker](https://docs.docker.com/)
- [StackExchange.Redis](https://stackexchange.github.io/StackExchange.Redis/)
