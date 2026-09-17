using FluentAssertions;
using Hypesoft.Application.Common;
using Hypesoft.Domain.Entities;
using Xunit;

namespace Hypesoft.Tests;

public class StockCalculatorTests
{
    [Fact]
    public void CalculateTotalStockValue_DeveSomarPrecoVezesQuantidadeDeTodosOsProdutos()
    {
        var products = new List<Product>
        {
            new() { Price = 10m, StockQuantity = 5 },  // 50
            new() { Price = 100m, StockQuantity = 2 }, // 200
            new() { Price = 7.5m, StockQuantity = 0 }  // 0
        };

        StockCalculator.CalculateTotalStockValue(products).Should().Be(250m);
    }

    [Fact]
    public void CalculateTotalStockValue_DeveRetornarZeroParaListaVazia()
    {
        StockCalculator.CalculateTotalStockValue(new List<Product>()).Should().Be(0m);
    }

    [Fact]
    public void GetLowStockProducts_DeveRetornarApenasProdutosAbaixoDoLimite()
    {
        var products = new List<Product>
        {
            new() { Name = "A", StockQuantity = 3 },
            new() { Name = "B", StockQuantity = 15 },
            new() { Name = "C", StockQuantity = 9 }
        };

        var result = StockCalculator.GetLowStockProducts(products, threshold: 10).ToList();

        result.Should().HaveCount(2);
        result.Select(p => p.Name).Should().BeEquivalentTo(new[] { "A", "C" });
    }
}
