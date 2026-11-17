using SolarGuard.Api.Models;
using SolarGuard.Api.Services;
using Xunit;

namespace SolarGuard.Api.Tests;

public class WarrantyRulesTests
{
    [Fact]
    public void Message_IncludesDeviceSerialAndDaysLeft()
    {
        var today = new DateOnly(2026, 6, 1);
        var warrantyUntil = new DateOnly(2026, 6, 20);

        var message = WarrantyRules.Message(DeviceType.Inverter, "SN-123", "Hiša 1", warrantyUntil, today);

        Assert.Contains("SN-123", message);
        Assert.Contains("20.06.2026", message);
        Assert.Contains("19 dni", message);
    }

    [Fact]
    public void DedupeKey_SameInputs_ProducesSameKey()
    {
        var deviceId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var userId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var warrantyUntil = new DateOnly(2026, 6, 20);

        var key1 = WarrantyRules.DedupeKey(deviceId, warrantyUntil, userId);
        var key2 = WarrantyRules.DedupeKey(deviceId, warrantyUntil, userId);

        Assert.Equal(key1, key2);
    }

    [Fact]
    public void DedupeKey_DifferentDevice_ProducesDifferentKey()
    {
        var userId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var warrantyUntil = new DateOnly(2026, 6, 20);

        var key1 = WarrantyRules.DedupeKey(Guid.Parse("11111111-1111-1111-1111-111111111111"), warrantyUntil, userId);
        var key2 = WarrantyRules.DedupeKey(Guid.Parse("33333333-3333-3333-3333-333333333333"), warrantyUntil, userId);

        Assert.NotEqual(key1, key2);
    }
}
