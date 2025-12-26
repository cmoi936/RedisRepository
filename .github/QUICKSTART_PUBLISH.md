# 🚀 Quick Start - Publication NuGet

Guide rapide pour publier le package RedisRepository sur NuGet.org.

## Configuration initiale (une seule fois)

### 1. Créer une clé API NuGet
1. Allez sur [nuget.org](https://www.nuget.org/) → **API Keys** → **Create**
2. Configurez :
   - **Name** : `RedisRepository GitHub Actions`
   - **Scopes** : `Push new packages and package versions`
   - **Glob Pattern** : `*`
3. **Copiez la clé** (vous ne pourrez plus la voir après)

### 2. Ajouter le secret dans GitHub
1. GitHub → **Settings** → **Secrets and variables** → **Actions**
2. **New repository secret**
3. **Name** : `NUGET_API_KEY`
4. **Secret** : Collez votre clé API
5. **Add secret**

## Publication (à chaque release)

### Méthode 1 : Via Git Tag (Recommandé)

```bash
# 1. Committez vos changements
git add .
git commit -m "feat: nouvelle fonctionnalité"
git push origin master

# 2. Créez et poussez le tag
git tag v1.0.0
git push origin v1.0.0
```

✅ **Le pipeline se lance automatiquement !**

### Méthode 2 : Via GitHub Actions UI

1. GitHub → **Actions** → **Publish NuGet Package**
2. **Run workflow** → Entrez la version (ex: `1.0.0`)
3. **Run workflow**

## Versioning rapide

```bash
# Version stable
git tag v1.0.0        # Première release

# Nouvelle fonctionnalité
git tag v1.1.0        # Minor version

# Correction de bug
git tag v1.0.1        # Patch version

# Breaking change
git tag v2.0.0        # Major version

# Pre-release
git tag v1.0.0-beta   # Version beta
git tag v1.0.0-rc.1   # Release candidate
```

## Vérification

Après publication, vérifiez :

- ✅ **GitHub Actions** : Workflow vert (passé)
- ✅ **NuGet.org** : https://www.nuget.org/packages/RedisRepository
- ✅ **GitHub Releases** : https://github.com/cmoi936/RedisRepository/releases

## Commandes d'installation

```bash
# .NET CLI
dotnet add package RedisRepository --version 1.0.0

# Package Manager Console
Install-Package RedisRepository -Version 1.0.0
```

## Troubleshooting rapide

| Erreur | Solution |
|--------|----------|
| "Package already exists" | Incrémentez la version (1.0.0 → 1.0.1) |
| "Invalid API key" | Vérifiez/régénérez la clé sur nuget.org |
| "Tests failed" | Exécutez `dotnet test` localement |
| "Build failed" | Exécutez `dotnet build` localement |

## Rollback

### Retirer une version (Unlist)
1. [nuget.org](https://www.nuget.org/) → **Manage Packages** → **RedisRepository**
2. Sélectionnez la version → **Unlist**

### Publier une correction
```bash
git tag v1.0.1
git push origin v1.0.1
```

## Liens utiles

- 📖 [Guide complet](./PUBLISHING_GUIDE.md) - Documentation détaillée
- 🔧 [Workflow](.github/workflows/publish-nuget.yml) - Configuration du pipeline
- 📦 [NuGet.org](https://www.nuget.org/packages/RedisRepository) - Package publié
- 🐛 [Issues](https://github.com/cmoi936/RedisRepository/issues) - Support

---

💡 **Astuce** : Utilisez toujours le préfixe `v` pour les tags (v1.0.0) !
