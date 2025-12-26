# Script PowerShell pour tester la création du package localement

Write-Host "=== Test de création du package NuGet ===" -ForegroundColor Cyan

# Configuration
$projectPath = ".\RedisRepository\RedisRepository.csproj"
$outputPath = ".\artifacts"
$version = "1.0.0-local"

# Nettoyage
Write-Host "`nNettoyage des artefacts précédents..." -ForegroundColor Yellow
if (Test-Path $outputPath) {
    Remove-Item $outputPath -Recurse -Force
}
New-Item -ItemType Directory -Path $outputPath | Out-Null

# Restore
Write-Host "`nRestauration des dépendances..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "Erreur lors de la restauration" -ForegroundColor Red
    exit 1
}

# Build
Write-Host "`nCompilation du projet..." -ForegroundColor Yellow
dotnet build --configuration Release --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "Erreur lors de la compilation" -ForegroundColor Red
    exit 1
}

# Test
Write-Host "`nExécution des tests..." -ForegroundColor Yellow
dotnet test --configuration Release --no-build --verbosity normal
if ($LASTEXITCODE -ne 0) {
    Write-Host "Erreur lors des tests" -ForegroundColor Red
    exit 1
}

# Pack
Write-Host "`nCréation du package NuGet..." -ForegroundColor Yellow
dotnet pack $projectPath `
    --configuration Release `
    --no-build `
    --output $outputPath `
    /p:PackageVersion=$version `
    /p:Version=$version

if ($LASTEXITCODE -ne 0) {
    Write-Host "Erreur lors de la création du package" -ForegroundColor Red
    exit 1
}

# Résumé
Write-Host "`n=== Package créé avec succès ===" -ForegroundColor Green
Write-Host "Emplacement : $outputPath" -ForegroundColor Cyan
Get-ChildItem $outputPath -Filter *.nupkg | ForEach-Object {
    Write-Host "  - $($_.Name)" -ForegroundColor White
}

Write-Host "`nPour inspecter le package :" -ForegroundColor Yellow
Write-Host "  nuget.exe list -Source $outputPath -AllVersions" -ForegroundColor Gray

Write-Host "`nPour publier sur NuGet.org (en production) :" -ForegroundColor Yellow
Write-Host "  dotnet nuget push .\artifacts\*.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json" -ForegroundColor Gray
