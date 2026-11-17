using SolarGuard.Api.Models;
using SolarGuard.Api.Services;
using Xunit;

namespace SolarGuard.Api.Tests;

public class ForecastRulesTests
{
    [Fact]
    public void Grade_RatioBelowWeakThreshold_ReturnsWeak()
    {
        Assert.Equal(ForecastGrade.Weak, ForecastRules.Grade(0.3));
    }

    [Fact]
    public void Grade_RatioAtBaseline_ReturnsNormal()
    {
        Assert.Equal(ForecastGrade.Normal, ForecastRules.Grade(1.0));
    }

    [Fact]
    public void Grade_RatioAboveGoodThreshold_ReturnsGood()
    {
        Assert.Equal(ForecastGrade.Good, ForecastRules.Grade(1.5));
    }

    [Fact]
    public void Grade_RatioExactlyAtWeakBoundary_ReturnsNormal()
    {
        Assert.Equal(ForecastGrade.Normal, ForecastRules.Grade(ForecastRules.WeakDay));
    }

    [Fact]
    public void Grade_RatioExactlyAtGoodBoundary_ReturnsNormal()
    {
        Assert.Equal(ForecastGrade.Normal, ForecastRules.Grade(ForecastRules.GoodDay));
    }
}
