using SolarGuard.Api.Models;
using SolarGuard.Api.Services;
using Xunit;

namespace SolarGuard.Api.Tests;

public class AlarmRulesTests
{
    private static readonly DateTime Now = new(2026, 6, 15, 12, 0, 0);

    [Fact]
    public void IsMissing_NoMeasurementsAndInstalledInPast_ReturnsTrue()
    {
        var installedOn = DateOnly.FromDateTime(Now.AddDays(-10));

        Assert.True(AlarmRules.IsMissing(null, installedOn, Now));
    }

    [Fact]
    public void IsMissing_NoMeasurementsAndInstalledToday_ReturnsFalse()
    {
        var installedOn = DateOnly.FromDateTime(Now);

        Assert.False(AlarmRules.IsMissing(null, installedOn, Now));
    }

    [Fact]
    public void IsMissing_GapOverTwoHours_ReturnsTrue()
    {
        var last = Now.AddHours(-3);

        Assert.True(AlarmRules.IsMissing(last, DateOnly.FromDateTime(Now.AddDays(-10)), Now));
    }

    [Fact]
    public void IsMissing_GapUnderTwoHours_ReturnsFalse()
    {
        var last = Now.AddHours(-1);

        Assert.False(AlarmRules.IsMissing(last, DateOnly.FromDateTime(Now.AddDays(-10)), Now));
    }

    [Fact]
    public void LowProduction_FewerThanThreeHours_ReturnsNull()
    {
        var hours = new List<(double Actual, double Expected)> { (3, 10), (3, 10) };

        Assert.Null(AlarmRules.LowProduction(hours));
    }

    [Fact]
    public void LowProduction_ExpectedBelowMinimum_ReturnsNull()
    {
        var hours = new List<(double Actual, double Expected)> { (0, 0.3), (3, 10), (3, 10) };

        Assert.Null(AlarmRules.LowProduction(hours));
    }

    [Fact]
    public void LowProduction_DeviationOverFiftyPercentForAllHours_ReturnsCritical()
    {
        var hours = new List<(double Actual, double Expected)> { (3, 10), (3, 10), (3, 10) };

        Assert.Equal(AlarmSeverity.Critical, AlarmRules.LowProduction(hours));
    }

    [Fact]
    public void LowProduction_DeviationBetweenTwentyAndFiftyPercent_ReturnsWarning()
    {
        var hours = new List<(double Actual, double Expected)> { (7, 10), (7, 10), (7, 10) };

        Assert.Equal(AlarmSeverity.Warning, AlarmRules.LowProduction(hours));
    }

    [Fact]
    public void LowProduction_DeviationUnderTwentyPercent_ReturnsNull()
    {
        var hours = new List<(double Actual, double Expected)> { (9, 10), (9, 10), (9, 10) };

        Assert.Null(AlarmRules.LowProduction(hours));
    }

    [Fact]
    public void LowProduction_OneGoodHourAmongBadOnes_DoesNotTriggerCritical()
    {
        // najmanjše odstopanje (najboljša ura) mora presegati prag, da se sproži alarm
        var hours = new List<(double Actual, double Expected)> { (3, 10), (3, 10), (9.5, 10) };

        Assert.Null(AlarmRules.LowProduction(hours));
    }
}
