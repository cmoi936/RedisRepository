namespace RedisRepository.IntegrationTests.Models;

/// <summary>
/// Modèle de test simple pour les tests d'intégration.
/// </summary>
public class TestModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<TestItem> Items { get; set; } = new();
}

/// <summary>
/// Modèle d'item de test.
/// </summary>
public class TestItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
}
