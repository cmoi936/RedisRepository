# Script pour arrêter l'instance Redis Docker pour les tests d'intégration
# Usage: .\Stop-RedisDocker.ps1

Write-Host "🛑 Arrêt de l'instance Redis Docker pour les tests..." -ForegroundColor Cyan

# Vérifier si le conteneur existe
$existingContainer = docker ps -a --filter "name=redis-integration-tests" --format "{{.Names}}"

if ($existingContainer) {
    # Arrêter avec docker-compose
    Write-Host "🔄 Arrêt de Redis avec docker-compose..." -ForegroundColor Cyan
    docker-compose down
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Redis arrêté avec succès!" -ForegroundColor Green
        Write-Host ""
        Write-Host "💡 Pour redémarrer Redis: .\Start-RedisDocker.ps1" -ForegroundColor Yellow
    }
    else {
        Write-Host "❌ Échec de l'arrêt de Redis." -ForegroundColor Red
        exit 1
    }
}
else {
    Write-Host "ℹ️  Aucune instance Redis en cours d'exécution." -ForegroundColor Yellow
}
