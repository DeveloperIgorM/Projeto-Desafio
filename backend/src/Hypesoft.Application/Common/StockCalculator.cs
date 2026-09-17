using Hypesoft.Domain.Entities;

namespace Hypesoft.Application.Common;

// Regras de estoque isoladas aqui pra dar pra testar sem precisar subir Mongo/API.
// E o que o Dia 3 do desafio pede: cálculo de valor total em estoque e detecção
// de estoque baixo, testados de verdade (nao so "no olho" via curl).
public static class StockCalculator
{
    public static decimal CalculateTotalStockValue(IEnumerable<Product> products) =>
        products.Sum(p => p.Price * p.StockQuantity);

    public static IEnumerable<Product> GetLowStockProducts(IEnumerable<Product> products, int threshold = 10) =>
        products.Where(p => p.IsLowStock(threshold));
}
