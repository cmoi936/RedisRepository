# Guide de publication du package NuGet

Ce guide explique comment publier le package RedisRepository sur NuGet.org à l'aide du pipeline GitHub Actions.

## 📋 Prérequis

1. **Compte NuGet.org** : Créez un compte sur [nuget.org](https://www.nuget.org/)
2. **Clé API NuGet** : Générez une clé API depuis votre compte NuGet.org
3. **Secret GitHub** : Configurez le secret dans votre repository GitHub

## 🔑 Configuration de la clé API NuGet

### Étape 1 : Créer une clé API sur NuGet.org

1. Connectez-vous à [nuget.org](https://www.nuget.org/)
2. Allez dans **Account Settings** > **API Keys**
3. Cliquez sur **Create**
4. Configurez :
   - **Key Name** : `RedisRepository GitHub Actions`
   - **Select Scopes** : `Push new packages and package versions`
   - **Select Packages** : `*` (ou sélectionnez RedisRepository une fois créé)
   - **Glob Pattern** : `*`
5. Cliquez sur **Create** et copiez la clé générée

### Étape 2 : Ajouter le secret dans GitHub

1. Allez sur votre repository GitHub : `https://github.com/cmoi936/RedisRepository`
2. Cliquez sur **Settings** > **Secrets and variables** > **Actions**
3. Cliquez sur **New repository secret**
4. Configurez :
   - **Name** : `NUGET_API_KEY`
   - **Secret** : Collez la clé API NuGet copiée
5. Cliquez sur **Add secret**

## 🚀 Publier une nouvelle version

### Option 1 : Publication via Tag Git (Recommandé)

```bash
# 1. Assurez-vous que tous les changements sont committés
git add .
git commit -m "Prepare release v1.0.0"

# 2. Créez un tag avec la version
git tag v1.0.0

# 3. Poussez le tag vers GitHub
git push origin v1.0.0
```

Le pipeline se déclenchera automatiquement et publiera le package avec la version `1.0.0`.

### Option 2 : Publication manuelle via GitHub Actions

1. Allez sur votre repository GitHub
2. Cliquez sur **Actions**
3. Sélectionnez **Publish NuGet Package**
4. Cliquez sur **Run workflow**
5. Entrez la version (ex: `1.0.0`)
6. Cliquez sur **Run workflow**

## 📦 Processus du pipeline

Le pipeline effectue les étapes suivantes :

1. ✅ **Checkout** : Récupère le code source
2. ✅ **Setup .NET** : Configure l'environnement .NET 8.0
3. ✅ **Determine version** : Détermine la version du package
4. ✅ **Restore** : Restaure les dépendances NuGet
5. ✅ **Build** : Compile le projet en mode Release
6. ✅ **Test** : Exécute les tests d'intégration
7. ✅ **Pack** : Crée le package NuGet (.nupkg)
8. ✅ **Publish** : Publie le package sur NuGet.org
9. ✅ **Upload artifacts** : Sauvegarde les artefacts
10. ✅ **GitHub Release** : Crée une release GitHub (pour les tags)

## 📝 Convention de versioning

Utilisez le [Semantic Versioning](https://semver.org/) :

- **MAJOR.MINOR.PATCH** (ex: 1.0.0)
- **MAJOR** : Changements incompatibles avec les versions précédentes
- **MINOR** : Nouvelles fonctionnalités rétrocompatibles
- **PATCH** : Corrections de bugs rétrocompatibles

Exemples :
```bash
git tag v1.0.0    # Première version stable
git tag v1.1.0    # Nouvelle fonctionnalité
git tag v1.1.1    # Correction de bug
git tag v2.0.0    # Breaking change
```

## 🔍 Vérifier la publication

Après la publication, vérifiez :

1. **NuGet.org** : `https://www.nuget.org/packages/RedisRepository`
2. **GitHub Actions** : Vérifiez que le workflow est passé (vert)
3. **GitHub Releases** : Une release devrait être créée automatiquement

## ⚠️ Dépannage

### Erreur : "Package already exists"
- Vous ne pouvez pas republier la même version
- Incrémentez le numéro de version

### Erreur : "Invalid API key"
- Vérifiez que le secret `NUGET_API_KEY` est correctement configuré
- Vérifiez que la clé API n'a pas expiré sur NuGet.org

### Erreur : "Tests failed"
- Les tests doivent passer avant la publication
- Corrigez les tests et recommencez

### Erreur : "Build failed"
- Vérifiez que le code compile localement
- Vérifiez les logs du pipeline pour plus de détails

## 📚 Ressources

- [Documentation NuGet](https://docs.microsoft.com/nuget/)
- [GitHub Actions](https://docs.github.com/actions)
- [Semantic Versioning](https://semver.org/)
