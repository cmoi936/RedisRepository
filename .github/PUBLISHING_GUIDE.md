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

> ⚠️ **Important** : Sauvegardez cette clé en lieu sûr, vous ne pourrez plus la voir après avoir quitté la page.

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
git push origin master

# 2. Créez un tag avec la version (le 'v' est obligatoire)
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
5. Entrez la version **sans le préfixe 'v'** (ex: `1.0.0`)
6. Cliquez sur **Run workflow**

## 📦 Processus du pipeline

Le pipeline effectue les étapes suivantes :

1. ✅ **Checkout** : Récupère le code source
2. ✅ **Setup .NET** : Configure l'environnement .NET 8.0
3. ✅ **Determine version** : Détermine la version du package
4. ✅ **Validate version** : Valide le format de la version (X.Y.Z ou X.Y.Z-suffix)
5. ✅ **Restore** : Restaure les dépendances NuGet
6. ✅ **Build** : Compile le projet en mode Release
7. ✅ **Test** : Exécute les tests d'intégration (avec Redis en service)
8. ✅ **Upload test results** : Sauvegarde les résultats des tests
9. ✅ **Pack** : Crée le package NuGet (.nupkg) et les symboles (.snupkg)
10. ✅ **Validate package** : Vérifie que le package a été créé correctement
11. ✅ **Publish** : Publie le package sur NuGet.org
12. ✅ **Upload artifacts** : Sauvegarde les packages (conservation 90 jours)
13. ✅ **GitHub Release** : Crée une release GitHub (pour les tags uniquement)
14. ✅ **Summary** : Affiche un résumé avec les liens utiles

## 📝 Convention de versioning

Utilisez le [Semantic Versioning](https://semver.org/) :

- **MAJOR.MINOR.PATCH** (ex: 1.0.0)
- **MAJOR** : Changements incompatibles avec les versions précédentes
- **MINOR** : Nouvelles fonctionnalités rétrocompatibles
- **PATCH** : Corrections de bugs rétrocompatibles

Vous pouvez également utiliser des suffixes pour les versions pré-release :

```bash
git tag v1.0.0          # Version stable
git tag v1.0.0-alpha    # Version alpha
git tag v1.0.0-beta     # Version beta
git tag v1.0.0-rc.1     # Release candidate
```

Exemples de progression :
```bash
git tag v1.0.0-beta.1   # Première beta
git tag v1.0.0-beta.2   # Deuxième beta
git tag v1.0.0-rc.1     # Release candidate
git tag v1.0.0          # Version stable finale
git tag v1.1.0          # Nouvelle fonctionnalité
git tag v1.1.1          # Correction de bug
git tag v2.0.0          # Breaking change
```

## 🔍 Vérifier la publication

Après la publication, vérifiez :

1. **GitHub Actions** : 
   - Allez dans l'onglet **Actions**
   - Vérifiez que le workflow **Publish NuGet Package** est passé (✅ vert)
   - Consultez le résumé du workflow pour les liens rapides

2. **NuGet.org** : 
   - `https://www.nuget.org/packages/RedisRepository`
   - La nouvelle version devrait apparaître dans les 5-10 minutes

3. **GitHub Releases** : 
   - `https://github.com/cmoi936/RedisRepository/releases`
   - Une release devrait être créée automatiquement (pour les tags uniquement)

4. **Artifacts** :
   - Les packages (.nupkg et .snupkg) sont disponibles dans les artifacts du workflow
   - Conservation pendant 90 jours

## 📥 Installation du package

Une fois publié, les utilisateurs peuvent installer le package :

```bash
# Installation via .NET CLI
dotnet add package RedisRepository --version 1.0.0

# Installation via Package Manager Console
Install-Package RedisRepository -Version 1.0.0

# Référence dans .csproj
<PackageReference Include="RedisRepository" Version="1.0.0" />
```

## ⚠️ Dépannage

### Erreur : "Package already exists"
**Cause** : Vous ne pouvez pas republier la même version sur NuGet.org

**Solution** :
```bash
# Supprimez le tag local
git tag -d v1.0.0

# Supprimez le tag distant
git push origin :refs/tags/v1.0.0

# Créez un nouveau tag avec une version incrémentée
git tag v1.0.1
git push origin v1.0.1
```

### Erreur : "Invalid API key" ou "403 Forbidden"
**Cause** : Le secret `NUGET_API_KEY` est invalide ou a expiré

**Solution** :
1. Connectez-vous à [nuget.org](https://www.nuget.org/)
2. Vérifiez que votre clé API est toujours active
3. Si nécessaire, générez une nouvelle clé
4. Mettez à jour le secret dans GitHub Settings > Secrets and variables > Actions

### Erreur : "Tests failed"
**Cause** : Les tests d'intégration ont échoué

**Solution** :
1. Exécutez les tests localement : `dotnet test`
2. Vérifiez les logs du workflow GitHub Actions
3. Consultez les artifacts de test pour plus de détails
4. Corrigez les tests et recommencez

### Erreur : "Build failed"
**Cause** : Le code ne compile pas

**Solution** :
1. Compilez localement : `dotnet build --configuration Release`
2. Vérifiez les erreurs de compilation
3. Corrigez les erreurs et recommencez

### Erreur : "Invalid version format"
**Cause** : Le format de version ne respecte pas le Semantic Versioning

**Solution** :
```bash
# ❌ Incorrect
git tag v1.0          # Manque le PATCH
git tag 1.0.0         # Manque le préfixe 'v'
git tag v1.0.0.0      # Trop de segments

# ✅ Correct
git tag v1.0.0
git tag v1.0.0-beta
git tag v1.0.0-rc.1
```

### Avertissement : "Package size is suspiciously small"
**Cause** : Le package généré est plus petit que prévu (< 1 KB)

**Solution** :
1. Vérifiez que le projet contient bien du code
2. Vérifiez que les fichiers sont inclus dans le package
3. Examinez les artifacts pour vérifier le contenu du package

## 🔄 Rollback d'une version

Si vous avez publié une version défectueuse :

### Option 1 : Unlisting (Recommandé)
1. Connectez-vous à [nuget.org](https://www.nuget.org/)
2. Allez dans **Manage Packages** > **RedisRepository**
3. Sélectionnez la version à retirer
4. Cliquez sur **Unlist**

> 💡 Le package restera téléchargeable pour ceux qui l'ont déjà référencé, mais n'apparaîtra plus dans les recherches.

### Option 2 : Publier une nouvelle version corrective
```bash
# Si v1.0.0 est défectueux
git tag v1.0.1
git push origin v1.0.1
```

## 🛡️ Sécurité

### Protection des secrets
- ✅ Ne committez **JAMAIS** votre clé API NuGet dans le code
- ✅ Utilisez toujours les GitHub Secrets
- ✅ Limitez les permissions de la clé API au strict nécessaire
- ✅ Régénérez régulièrement vos clés API

### Permissions du workflow
Le workflow nécessite les permissions suivantes :
- `contents: write` - Pour créer des releases GitHub
- `packages: write` - Pour publier des packages

## 📚 Ressources

- [Documentation NuGet](https://docs.microsoft.com/nuget/)
- [GitHub Actions Documentation](https://docs.github.com/actions)
- [Semantic Versioning](https://semver.org/)
- [NuGet Package Best Practices](https://docs.microsoft.com/nuget/create-packages/package-authoring-best-practices)
- [Symbol Packages (.snupkg)](https://docs.microsoft.com/nuget/create-packages/symbol-packages-snupkg)

## 📞 Support

Si vous rencontrez des problèmes :
1. Consultez les [Issues GitHub](https://github.com/cmoi936/RedisRepository/issues)
2. Consultez les logs du workflow dans GitHub Actions
3. Créez une nouvelle issue si nécessaire
