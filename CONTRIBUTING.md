# Guide de contribution

Merci de votre intérêt pour contribuer à RedisRepository ! 🎉

## Comment contribuer

### 1. Reporter un bug

Si vous trouvez un bug, veuillez créer une issue avec :
- Une description claire du problème
- Les étapes pour reproduire le bug
- Le comportement attendu vs le comportement actuel
- Votre environnement (OS, version de .NET, version de Redis)

### 2. Proposer une fonctionnalité

Pour proposer une nouvelle fonctionnalité :
1. Créez une issue pour discuter de l'idée
2. Attendez les retours avant de commencer le développement
3. Assurez-vous que la fonctionnalité est alignée avec les objectifs du projet

### 3. Soumettre une Pull Request

#### Prérequis
- .NET 8.0 SDK ou supérieur
- Redis installé et en cours d'exécution (pour les tests)
- Git

#### Processus

1. **Fork le repository**
   ```bash
   # Clonez votre fork
   git clone https://github.com/VOTRE_USERNAME/RedisRepository.git
   cd RedisRepository
   ```

2. **Créez une branche**
   ```bash
   git checkout -b feature/ma-nouvelle-fonctionnalite
   # ou
   git checkout -b fix/correction-du-bug
   ```

3. **Développez votre fonctionnalité**
   - Écrivez du code propre et documenté
   - Suivez les conventions de code existantes
   - Ajoutez des tests unitaires/intégration
   - Mettez à jour la documentation si nécessaire

4. **Testez votre code**
   ```bash
   # Assurez-vous que Redis est démarré
   redis-server
   
   # Compilez le projet
   dotnet build
   
   # Exécutez tous les tests
   dotnet test
   
   # Ou utilisez le script PowerShell
   .\build-package.ps1
   ```

5. **Committez vos changements**
   ```bash
   git add .
   git commit -m "feat: ajout de la fonctionnalité X"
   # ou
   git commit -m "fix: correction du bug Y"
   ```
   
   Utilisez les préfixes de commit conventionnels :
   - `feat:` nouvelle fonctionnalité
   - `fix:` correction de bug
   - `docs:` documentation uniquement
   - `test:` ajout ou modification de tests
   - `refactor:` refactoring de code
   - `perf:` amélioration des performances
   - `chore:` tâches de maintenance

6. **Poussez vers votre fork**
   ```bash
   git push origin feature/ma-nouvelle-fonctionnalite
   ```

7. **Créez une Pull Request**
   - Allez sur GitHub et créez une PR depuis votre branche
   - Remplissez le template de PR
   - Liez les issues pertinentes

#### Checklist pour la Pull Request

- [ ] Le code compile sans erreurs ni warnings
- [ ] Tous les tests passent
- [ ] De nouveaux tests ont été ajoutés pour couvrir les changements
- [ ] La documentation a été mise à jour
- [ ] Le CHANGELOG.md a été mis à jour
- [ ] Le code suit les conventions du projet
- [ ] Les commentaires et la documentation sont en français
- [ ] Les messages de commit sont clairs et suivent les conventions

## Standards de code

### Style de code
- Utilisez les conventions C# standard
- Nommage en PascalCase pour les classes, méthodes, propriétés
- Nommage en camelCase pour les variables locales et paramètres
- Utilisez des noms significatifs et descriptifs
- Évitez les abréviations sauf si elles sont courantes (ex: Id, Dto)

### Documentation
- Tous les types publics doivent avoir une documentation XML (///)
- Les méthodes publiques doivent avoir une documentation complète
- Les commentaires doivent être en français
- Incluez des exemples de code dans la documentation quand c'est pertinent

### Tests
- Écrivez des tests pour toute nouvelle fonctionnalité
- Maintenez ou améliorez la couverture de tests
- Utilisez des noms de test descriptifs : `MethodName_Scenario_ExpectedBehavior`
- Utilisez le pattern AAA (Arrange, Act, Assert)
- Utilisez FluentAssertions pour les assertions

### Exemple de test

```csharp
[Fact]
public async Task SetAsync_ValidData_ShouldStoreSuccessfully()
{
    // Arrange
    var key = "test-key";
    var data = new TestModel { Name = "Test" };
    var expiration = TimeSpan.FromMinutes(30);

    // Act
    await _repository.SetAsync(key, data, expiration);

    // Assert
    var exists = await _repository.ExistsAsync(key);
    exists.Should().BeTrue();
}
```

## Gestion des versions

Le projet utilise [Semantic Versioning](https://semver.org/lang/fr/) :
- **MAJOR** : Changements incompatibles avec les versions précédentes
- **MINOR** : Nouvelles fonctionnalités rétrocompatibles
- **PATCH** : Corrections de bugs rétrocompatibles

## Questions ?

N'hésitez pas à :
- Ouvrir une issue pour poser une question
- Consulter la documentation existante
- Regarder les PRs précédentes pour voir des exemples

## Code de conduite

- Soyez respectueux et professionnel
- Acceptez les critiques constructives
- Concentrez-vous sur ce qui est le mieux pour la communauté
- Faites preuve d'empathie envers les autres membres

Merci de contribuer à RedisRepository ! 🚀
