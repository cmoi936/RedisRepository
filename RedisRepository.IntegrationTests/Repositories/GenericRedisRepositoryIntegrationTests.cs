using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RedisRepository.IntegrationTests.Models;

namespace RedisRepository.IntegrationTests.Repositories;

/// <summary>
/// Tests d'intégration pour GenericRedisRepository avec une instance Redis réelle.
/// </summary>
[Collection("Redis Integration Tests")]
public class GenericRedisRepositoryIntegrationTests : IntegrationTests.RedisIntegrationTestBase
{
    private RedisRepository.Repositories.IGenericRedisRepository<TestModel> _repository = null!;
    private ILogger<RedisRepository.Repositories.GenericRedisRepository<TestModel>> _logger = null!;
    private readonly IFixture _fixture;

    public GenericRedisRepositoryIntegrationTests()
    {
        _fixture = new Fixture();
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _logger = ServiceProvider.GetRequiredService<ILogger<RedisRepository.Repositories.GenericRedisRepository<TestModel>>>();
        _repository = new RedisRepository.Repositories.GenericRedisRepository<TestModel>(Database, _logger, "test:model:");
    }

    public override async Task DisposeAsync()
    {
        // Nettoyer les données de test avant de disposer
        await CleanupTestData();
        await base.DisposeAsync();
    }

    private async Task CleanupTestData()
    {
        var testKeys = new[]
        {
            "model-1", "existing-key", "key-to-delete", "key-with-expiration",
            "key-with-ttl", "complex-model"
        };

        foreach (var key in testKeys)
        {
            await _repository.DeleteAsync(key);
        }
    }

    [Fact]
    public async Task SetAsync_ValidData_ShouldStoreSuccessfully()
    {
        // Arrange
        var key = "model-1";
        var model = _fixture.Create<TestModel>();
        var expiration = TimeSpan.FromMinutes(30);

        // Act
        await _repository.SetAsync(key, model, expiration);

        // Assert
        var exists = await _repository.ExistsAsync(key);
        exists.Should().BeTrue();

        var retrievedModel = await _repository.GetAsync(key);
        retrievedModel.Should().NotBeNull();
        retrievedModel!.Name.Should().Be(model.Name);
        retrievedModel.Description.Should().Be(model.Description);
        retrievedModel.Items.Should().HaveCount(model.Items.Count);
    }

    [Fact]
    public async Task GetAsync_NonExistentKey_ShouldReturnNull()
    {
        // Arrange
        var nonExistentKey = "non-existent-key";

        // Act
        var result = await _repository.GetAsync(nonExistentKey);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_ExistingKey_ShouldReturnTrue()
    {
        // Arrange
        var key = "existing-key";
        var model = _fixture.Create<TestModel>();
        await _repository.SetAsync(key, model);

        // Act
        var exists = await _repository.ExistsAsync(key);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_NonExistentKey_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentKey = "non-existent-key";

        // Act
        var exists = await _repository.ExistsAsync(nonExistentKey);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_ExistingKey_ShouldReturnTrueAndRemoveData()
    {
        // Arrange
        var key = "key-to-delete";
        var model = _fixture.Create<TestModel>();
        await _repository.SetAsync(key, model);

        // Act
        var deleted = await _repository.DeleteAsync(key);

        // Assert
        deleted.Should().BeTrue();
        var exists = await _repository.ExistsAsync(key);
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_NonExistentKey_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentKey = "non-existent-key";

        // Act
        var deleted = await _repository.DeleteAsync(nonExistentKey);

        // Assert
        deleted.Should().BeFalse();
    }

    [Fact]
    public async Task ExpireAsync_ExistingKey_ShouldSetExpirationSuccessfully()
    {
        // Arrange
        var key = "key-with-expiration";
        var model = _fixture.Create<TestModel>();
        var expiration = TimeSpan.FromSeconds(30);

        await _repository.SetAsync(key, model);

        // Act
        var result = await _repository.ExpireAsync(key, expiration);

        // Assert
        result.Should().BeTrue();

        var ttl = await _repository.TimeToLiveAsync(key);
        ttl.Should().NotBeNull();
        ttl!.Value.Should().BeLessThanOrEqualTo(expiration);
        ttl.Value.Should().BeGreaterThan(TimeSpan.Zero);
    }

    [Fact]
    public async Task ExpireAsync_NonExistentKey_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentKey = "non-existent-key";
        var expiration = TimeSpan.FromSeconds(30);

        // Act
        var result = await _repository.ExpireAsync(nonExistentKey, expiration);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task TimeToLiveAsync_KeyWithExpiration_ShouldReturnRemainingTime()
    {
        // Arrange
        var key = "key-with-ttl";
        var model = _fixture.Create<TestModel>();
        var expiration = TimeSpan.FromMinutes(5);

        await _repository.SetAsync(key, model, expiration);

        // Act
        var ttl = await _repository.TimeToLiveAsync(key);

        // Assert
        ttl.Should().NotBeNull();
        ttl!.Value.Should().BeLessThanOrEqualTo(expiration);
        ttl.Value.Should().BeGreaterThan(TimeSpan.Zero);
    }

    [Fact]
    public async Task TimeToLiveAsync_NonExistentKey_ShouldReturnNull()
    {
        // Arrange
        var nonExistentKey = "non-existent-key";

        // Act
        var ttl = await _repository.TimeToLiveAsync(nonExistentKey);

        // Assert
        ttl.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_NullKey_ShouldThrowArgumentException()
    {
        // Arrange
        var model = _fixture.Create<TestModel>();

        // Act & Assert
        await FluentActions.Invoking(async () => await _repository.SetAsync(null!, model))
            .Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SetAsync_EmptyKey_ShouldThrowArgumentException()
    {
        // Arrange
        var model = _fixture.Create<TestModel>();

        // Act & Assert
        await FluentActions.Invoking(async () => await _repository.SetAsync(string.Empty, model))
            .Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SetAsync_NullData_ShouldThrowArgumentNullException()
    {
        // Arrange
        var key = "test-key";

        // Act & Assert
        await FluentActions.Invoking(async () => await _repository.SetAsync(key, null!))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task ComplexModelData_ShouldSerializeAndDeserializeCorrectly()
    {
        // Arrange
        var key = "complex-model";
        var complexModel = _fixture.Build<TestModel>()
            .With(m => m.Items, _fixture.CreateMany<TestItem>(5).ToList())
            .Create();

        // Act
        await _repository.SetAsync(key, complexModel);
        var retrievedModel = await _repository.GetAsync(key);

        // Assert
        retrievedModel.Should().NotBeNull();
        retrievedModel!.Name.Should().Be(complexModel.Name);
        retrievedModel.Description.Should().Be(complexModel.Description);
        retrievedModel.Id.Should().Be(complexModel.Id);

        retrievedModel.Items.Should().HaveCount(complexModel.Items.Count);
        for (int i = 0; i < complexModel.Items.Count; i++)
        {
            retrievedModel.Items[i].Name.Should().Be(complexModel.Items[i].Name);
            retrievedModel.Items[i].Value.Should().Be(complexModel.Items[i].Value);
            retrievedModel.Items[i].Id.Should().Be(complexModel.Items[i].Id);
        }
    }

    [Fact]
    public async Task SetAsync_MultipleModels_ShouldHandleDifferentDataStructures()
    {
        // Arrange - Teste avec différents types de modèles générés automatiquement
        var models = _fixture.CreateMany<TestModel>(3).ToArray();
        var keys = new[] { "model-test-1", "model-test-2", "model-test-3" };

        try
        {
            // Act
            for (int i = 0; i < models.Length; i++)
            {
                await _repository.SetAsync(keys[i], models[i]);
            }

            // Assert
            for (int i = 0; i < models.Length; i++)
            {
                var retrievedModel = await _repository.GetAsync(keys[i]);
                retrievedModel.Should().NotBeNull();
                retrievedModel!.Name.Should().Be(models[i].Name);
                retrievedModel.Id.Should().Be(models[i].Id);
                retrievedModel.Items.Should().HaveCount(models[i].Items.Count);
            }
        }
        finally
        {
            // Cleanup
            foreach (var key in keys)
            {
                await _repository.DeleteAsync(key);
            }
        }
    }
}