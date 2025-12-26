# Script pour démarrer l'instance Redis Docker pour les tests d'intégration
# Usage: .\Start-RedisDocker.ps1

Write-Host "🚀 Démarrage de l'instance Redis Docker pour les tests..." -ForegroundColor Cyan

# Vérifier si Docker est installé
try {
    $dockerVersion = docker --version
    Write-Host "✓ Docker détecté: $dockerVersion" -ForegroundColor Green
}
catch {
    Write-Host "❌ Docker n'est pas installé ou n'est pas accessible." -ForegroundColor Red
    Write-Host "Veuillez installer Docker Desktop: https://www.docker.com/products/docker-desktop" -ForegroundColor Yellow
    exit 1
}

# Vérifier si Docker est en cours d'exécution
$dockerInfo = docker info 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Docker n'est pas en cours d'exécution." -ForegroundColor Red
    Write-Host "Veuillez démarrer Docker Desktop." -ForegroundColor Yellow
    exit 1
}

# Arrêter et supprimer le conteneur existant s'il existe
$existingContainer = docker ps -a --filter "name=redis-integration-tests" --format "{{.Names}}"
if ($existingContainer) {
    Write-Host "⚠️  Arrêt et suppression du conteneur Redis existant..." -ForegroundColor Yellow
    docker stop redis-integration-tests 2>&1 | Out-Null
    docker rm redis-integration-tests 2>&1 | Out-Null
}

# Démarrer Redis avec docker-compose
Write-Host "🔄 Démarrage de Redis avec docker-compose..." -ForegroundColor Cyan
docker-compose up -d

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Redis démarré avec succès!" -ForegroundColor Green
    
    # Attendre que Redis soit prêt
    Write-Host "⏳ Attente que Redis soit prêt..." -ForegroundColor Cyan
    $maxAttempts = 30
    $attempt = 0
    $ready = $false
    
    while ($attempt -lt $maxAttempts -and -not $ready) {
        Start-Sleep -Seconds 1
        $attempt++
        
        try {
            $pingResult = docker exec redis-integration-tests redis-cli ping 2>&1
            if ($pingResult -eq "PONG") {
                $ready = $true
            }
        }
        catch {
            # Continuer à attendre
        }
        
        Write-Host "." -NoNewline
    }
    
    Write-Host ""
    
    if ($ready) {
        Write-Host "✓ Redis est prêt et répond aux requêtes!" -ForegroundColor Green
        Write-Host ""
        Write-Host "📊 Informations de connexion:" -ForegroundColor Cyan
        Write-Host "   Host: localhost" -ForegroundColor White
        Write-Host "   Port: 6379" -ForegroundColor White
        Write-Host "   Databases: 0-15" -ForegroundColor White
        Write-Host ""
        Write-Host "💡 Pour arrêter Redis: .\Stop-RedisDocker.ps1" -ForegroundColor Yellow
        Write-Host "💡 Pour voir les logs: docker logs redis-integration-tests" -ForegroundColor Yellow
    }
    else {
        Write-Host "⚠️  Redis a démarré mais ne répond pas encore. Veuillez patienter quelques secondes." -ForegroundColor Yellow
    }
}
else {
    Write-Host "❌ Échec du démarrage de Redis." -ForegroundColor Red
    exit 1
}
