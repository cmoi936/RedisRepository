# 🚀 Guide rapide - Pipeline GitHub Actions pour RedisRepository

## ✅ Ce qui a été créé

### 1. Pipeline GitHub Actions
**Fichier** : `.github/workflows/publish-nuget.yml`

Ce pipeline automatise la publication de votre package NuGet sur NuGet.org avec les fonctionnalités suivantes :
- ✅ Compilation du projet en mode Release
- ✅ Exécution des tests d'intégration
- ✅ Création du package NuGet (.nupkg)
- ✅ Publication automatique sur NuGet.org
- ✅ Création d'une GitHub Release avec le package
- ✅ Sauvegarde des artefacts

### 2. Métadonnées du package
**Fichier** : `RedisRepository/RedisRepository.csproj`

Les métadonnées suivantes ont été ajoutées :
- Package ID, auteur, description
- Tags pour la recherche (redis, repository, generic, cache, etc.)
- URL du repository GitHub
- Licence MIT
- Support des symboles de débogage (.snupkg)

### 3. Documentation
- ✅ `README.md` - Documentation principale du projet
- ✅ `LICENSE` - Licence MIT
- ✅ `.github/PUBLISHING_GUIDE.md` - Guide détaillé de publication
- ✅ `build-package.ps1` - Script de test local

## 🎯 Prochaines étapes (OBLIGATOIRE)

### Étape 1 : Créer une clé API NuGet

1. Allez sur [nuget.org](https://www.nuget.org/)
2. Connectez-vous ou créez un compte
3. Allez dans **Account Settings** → **API Keys**
4. Cliquez sur **Create**
5. Configurez :
   - **Key Name** : `RedisRepository GitHub Actions`
   - **Select Scopes** : Cochez `Push new packages and package versions`
   - **Glob Pattern** : `*`
6. Cliquez sur **Create** et **copiez immédiatement la clé** (elle ne sera plus affichée)

### Étape 2 : Ajouter le secret GitHub

1. Allez sur votre repository : `https://github.com/cmoi936/RedisRepository`
2. Cliquez sur **Settings** → **Secrets and variables** → **Actions**
3. Cliquez sur **New repository secret**
4. Configurez :
   - **Name** : `NUGET_API_KEY`
   - **Secret** : Collez la clé API NuGet
5. Cliquez sur **Add secret**

## 📤 Publier votre package

### Méthode 1 : Via Tag Git (Recommandée pour les releases)

```bash
# 1. Committez vos changements
git add .
git commit -m "Release v1.0.0"

# 2. Créez un tag avec la version
git tag v1.0.0

# 3. Poussez le tag
git push origin v1.0.0
```

Le pipeline se déclenchera automatiquement et publiera la version `1.0.0`.

### Méthode 2 : Via l'interface GitHub (Pour les tests ou releases manuelles)

1. Allez sur `https://github.com/cmoi936/RedisRepository/actions`
2. Sélectionnez le workflow **Publish NuGet Package**
3. Cliquez sur **Run workflow**
4. Entrez la version (ex: `1.0.0-beta`)
5. Cliquez sur **Run workflow**

## 🧪 Tester localement (avant de publier)

Exécutez le script PowerShell fourni :

```powershell
.\build-package.ps1
```

Ce script va :
1. Nettoyer les anciens artefacts
2. Restaurer les dépendances
3. Compiler le projet
4. Exécuter les tests
5. Créer le package NuGet dans le dossier `./artifacts`

**Note** : Les tests nécessitent Redis sur localhost:6379

## 📋 Convention de versioning

Utilisez [Semantic Versioning](https://semver.org/) :

- `v1.0.0` - Première version stable
- `v1.1.0` - Nouvelle fonctionnalité
- `v1.1.1` - Correction de bug
- `v2.0.0` - Breaking change
- `v1.0.0-beta` - Version beta
- `v1.0.0-rc.1` - Release candidate

## 🔍 Vérifier la publication

Après publication, vérifiez :

1. **GitHub Actions** : `https://github.com/cmoi936/RedisRepository/actions`
   - Le workflow doit être vert ✅
   
2. **NuGet.org** : `https://www.nuget.org/packages/RedisRepository`
   - Le package doit apparaître (peut prendre quelques minutes)
   
3. **GitHub Releases** : `https://github.com/cmoi936/RedisRepository/releases`
   - Une release doit être créée automatiquement

## 📦 Installer votre package

Une fois publié, les utilisateurs pourront installer votre package :

```bash
dotnet add package RedisRepository
```

ou via le Package Manager Console dans Visual Studio :

```powershell
Install-Package RedisRepository
```

## ⚠️ Points importants

1. **Redis requis pour les tests** : Les tests d'intégration nécessitent Redis sur localhost:6379
2. **Ne republiez pas la même version** : Incrémentez toujours le numéro de version
3. **Tests obligatoires** : Le pipeline échouera si les tests ne passent pas
4. **Délai de publication** : Il peut y avoir un délai de quelques minutes avant que le package apparaisse sur NuGet.org

## 📚 Documentation complète

Pour plus de détails, consultez : `.github/PUBLISHING_GUIDE.md`

## 🤝 Support

Pour toute question ou problème :
1. Consultez les logs du workflow GitHub Actions
2. Vérifiez que Redis est bien démarré pour les tests
3. Vérifiez que le secret `NUGET_API_KEY` est correctement configuré
