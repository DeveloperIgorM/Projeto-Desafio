using FluentAssertions;
using Hypesoft.Domain.Entities;
using Xunit;

namespace Hypesoft.Tests;

public class ProductTests
{
    [Theory]
    [InlineData(0, true)]
    [InlineData(5, true)]
    [InlineData(9, true)]
    [InlineData(10, false)]
    [InlineData(50, false)]
    public void IsLowStock_DeveConsiderarEstoqueBaixoQuandoMenorQueLimitePadrao(int quantity, bool expected)
    {
        var product = new Product { StockQuantity = quantity };

        product.IsLowStock().Should().Be(expected);
    }

    [Fact]
    public void IsLowStock_DeveAceitarLimitePersonalizado()
    {
        var product = new Product { StockQuantity = 15 };

        product.IsLowStock(threshold: 20).Should().BeTrue();
        product.IsLowStock(threshold: 10).Should().BeFalse();
    }
}
