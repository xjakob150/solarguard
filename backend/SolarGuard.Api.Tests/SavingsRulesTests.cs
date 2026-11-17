using SolarGuard.Api.Models;
using SolarGuard.Api.Services;
using Xunit;

namespace SolarGuard.Api.Tests;

public class SavingsRulesTests
{
    private static ElectricityPriceChange Price(DateTime validFrom, decimal price) =>
        new() { ValidFrom = validFrom, PricePerKwh = price };

    [Fact]
    public void PriceAt_HourBeforeAllChanges_ReturnsFirstPrice()
    {
        var prices = new List<ElectricityPriceChange>
        {
            Price(new DateTime(2026, 1, 1), 0.10m),
            Price(new DateTime(2026, 6, 1), 0.20m),
        };

        var result = SavingsRules.PriceAt(prices, new DateTime(2025, 12, 1));

        Assert.Equal(0.10m, result);
    }

    [Fact]
    public void PriceAt_HourBetweenTwoChanges_ReturnsEarlierPrice()
    {
        var prices = new List<ElectricityPriceChange>
        {
            Price(new DateTime(2026, 1, 1), 0.10m),
            Price(new DateTime(2026, 6, 1), 0.20m),
        };

        var result = SavingsRules.PriceAt(prices, new DateTime(2026, 3, 1));

        Assert.Equal(0.10m, result);
    }

    [Fact]
    public void PriceAt_HourAfterLatestChange_ReturnsLatestPrice()
    {
        var prices = new List<ElectricityPriceChange>
        {
            Price(new DateTime(2026, 1, 1), 0.10m),
            Price(new DateTime(2026, 6, 1), 0.20m),
        };

        var result = SavingsRules.PriceAt(prices, new DateTime(2026, 7, 1));

        Assert.Equal(0.20m, result);
    }

    [Fact]
    public void Calculate_SumsProductionIntoDayMonthYearAndTotal()
    {
        var prices = new List<ElectricityPriceChange> { Price(new DateTime(2000, 1, 1), 0.5m) };
        var hours = new List<HourlyProduction>
        {
            new(new DateTime(2026, 6, 15, 10, 0, 0), 2m), // danes
            new(new DateTime(2026, 6, 15, 14, 0, 0), 1m), // danes
            new(new DateTime(2026, 6, 14, 10, 0, 0), 2m), // ta mesec, ne danes
            new(new DateTime(2026, 5, 1, 10, 0, 0), 1m), // to leto, ne ta mesec
            new(new DateTime(2025, 12, 31, 10, 0, 0), 1m), // samo skupaj, ne to leto
        };
        var now = new DateTime(2026, 6, 15, 12, 0, 0);

        var savings = SavingsRules.Calculate(hours, prices, 0.5m, now);

        Assert.Equal(1.5m, savings.Day);
        Assert.Equal(2.5m, savings.Month);
        Assert.Equal(3.0m, savings.Year);
        Assert.Equal(3.5m, savings.Total);
    }
}
