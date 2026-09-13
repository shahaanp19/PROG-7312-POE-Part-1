using SmartX.Shared.Models;

namespace SmartX.Tests.Models;

public class TelemetryMetricTests
{
    [Fact]
    public void TelemetryMetric_ShouldStoreSensorIdentity()
    {
        var sensorId = Guid.NewGuid();

        var metric = new TelemetryMetric
        {
            SensorDeviceId = sensorId,
            MetricName = "Temperature",
            Unit = "°C"
        };

        Assert.Equal(sensorId, metric.SensorDeviceId);
    }

    [Fact]
    public void TelemetryMetric_ShouldStoreMetricMetadata()
    {
        var metric = new TelemetryMetric
        {
            SensorDeviceId = Guid.NewGuid(),
            MetricName = "Soil Moisture",
            Unit = "%",
            MinimumExpectedValue = 20,
            MaximumExpectedValue = 80
        };

        Assert.Equal("Soil Moisture", metric.MetricName);
        Assert.Equal("%", metric.Unit);
        Assert.Equal(20, metric.MinimumExpectedValue);
        Assert.Equal(80, metric.MaximumExpectedValue);
    }

    [Fact]
    public void TelemetryMetric_ShouldStoreThresholdConfiguration()
    {
        var metric = new TelemetryMetric
        {
            SensorDeviceId = Guid.NewGuid(),
            MetricName = "Power",
            Unit = "W",
            WarningThreshold = 1000,
            CriticalThreshold = 1500
        };

        Assert.Equal(1000, metric.WarningThreshold);
        Assert.Equal(1500, metric.CriticalThreshold);
    }

    [Fact]
    public void TelemetryMetric_ShouldBeEnabledByDefault()
    {
        var metric = new TelemetryMetric();

        Assert.True(metric.IsEnabled);
    }

    [Fact]
    public void TelemetryMetric_ShouldAllowDisablingValidation()
    {
        var metric = new TelemetryMetric
        {
            IsEnabled = false
        };

        Assert.False(metric.IsEnabled);
    }

    [Fact]
    public void TelemetryMetric_ShouldAllowEnvironmentalTelemetryConfiguration()
    {
        var metric = new TelemetryMetric
        {
            SensorDeviceId = Guid.NewGuid(),
            MetricName = "Soil Moisture",
            Unit = "%",
            MinimumExpectedValue = 0,
            MaximumExpectedValue = 100,
            WarningThreshold = 80,
            CriticalThreshold = 95
        };

        Assert.Equal("Soil Moisture", metric.MetricName);
        Assert.Equal("%", metric.Unit);
        Assert.Equal(0, metric.MinimumExpectedValue);
        Assert.Equal(100, metric.MaximumExpectedValue);
    }

    [Fact]
    public void TelemetryMetric_ShouldAllowPowerTelemetryConfiguration()
    {
        var metric = new TelemetryMetric
        {
            SensorDeviceId = Guid.NewGuid(),
            MetricName = "Power Consumption",
            Unit = "W",
            MinimumExpectedValue = 0,
            MaximumExpectedValue = 5000,
            WarningThreshold = 4000,
            CriticalThreshold = 4500
        };

        Assert.Equal("Power Consumption", metric.MetricName);
        Assert.Equal("W", metric.Unit);
        Assert.Equal(5000, metric.MaximumExpectedValue);
    }

    [Fact]
    public void TelemetryMetric_ShouldAllowActuatorTelemetryConfiguration()
    {
        var metric = new TelemetryMetric
        {
            SensorDeviceId = Guid.NewGuid(),
            MetricName = "Valve State",
            Unit = "state",
            IsEnabled = true
        };

        Assert.Equal("Valve State", metric.MetricName);
        Assert.Equal("state", metric.Unit);
        Assert.True(metric.IsEnabled);
    }
}