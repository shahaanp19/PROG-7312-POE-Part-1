using SmartX.Shared.Models;
using SmartX.Shared.Validators;

namespace SmartX.Tests.Validation;

public class TelemetryThresholdValidatorTests
{
    private readonly TelemetryThresholdValidator _validator = new();

    [Fact]
    public void IsWithinExpectedRange_ShouldReturnTrueForValueInsideRange()
    {
        var metric = CreateMetric(0, 100);

        var result = _validator.IsWithinExpectedRange(metric, 50);

        Assert.True(result);
    }

    [Fact]
    public void IsWithinExpectedRange_ShouldReturnTrueAtMinimumBoundary()
    {
        var metric = CreateMetric(0, 100);

        var result = _validator.IsWithinExpectedRange(metric, 0);

        Assert.True(result);
    }

    [Fact]
    public void IsWithinExpectedRange_ShouldReturnTrueAtMaximumBoundary()
    {
        var metric = CreateMetric(0, 100);

        var result = _validator.IsWithinExpectedRange(metric, 100);

        Assert.True(result);
    }

    [Fact]
    public void IsWithinExpectedRange_ShouldReturnFalseBelowMinimum()
    {
        var metric = CreateMetric(0, 100);

        var result = _validator.IsWithinExpectedRange(metric, -1);

        Assert.False(result);
    }

    [Fact]
    public void IsWithinExpectedRange_ShouldReturnFalseAboveMaximum()
    {
        var metric = CreateMetric(0, 100);

        var result = _validator.IsWithinExpectedRange(metric, 101);

        Assert.False(result);
    }

    [Fact]
    public void IsWarning_ShouldReturnTrueAtWarningThreshold()
    {
        var metric = CreateMetric(0, 100, 70, 90);

        var result = _validator.IsWarning(metric, 70);

        Assert.True(result);
    }

    [Fact]
    public void IsWarning_ShouldReturnTrueAboveWarningThreshold()
    {
        var metric = CreateMetric(0, 100, 70, 90);

        var result = _validator.IsWarning(metric, 80);

        Assert.True(result);
    }

    [Fact]
    public void IsWarning_ShouldReturnFalseBelowWarningThreshold()
    {
        var metric = CreateMetric(0, 100, 70, 90);

        var result = _validator.IsWarning(metric, 69.9);

        Assert.False(result);
    }

    [Fact]
    public void IsCritical_ShouldReturnTrueAtCriticalThreshold()
    {
        var metric = CreateMetric(0, 100, 70, 90);

        var result = _validator.IsCritical(metric, 90);

        Assert.True(result);
    }

    [Fact]
    public void IsCritical_ShouldReturnTrueAboveCriticalThreshold()
    {
        var metric = CreateMetric(0, 100, 70, 90);

        var result = _validator.IsCritical(metric, 95);

        Assert.True(result);
    }

    [Fact]
    public void IsCritical_ShouldReturnFalseBelowCriticalThreshold()
    {
        var metric = CreateMetric(0, 100, 70, 90);

        var result = _validator.IsCritical(metric, 89.9);

        Assert.False(result);
    }

    [Fact]
    public void IsAnomaly_ShouldReturnTrueOutsideExpectedRange()
    {
        var metric = CreateMetric(20, 80);

        var result = _validator.IsAnomaly(metric, 100);

        Assert.True(result);
    }

    [Fact]
    public void IsAnomaly_ShouldReturnFalseInsideExpectedRange()
    {
        var metric = CreateMetric(20, 80);

        var result = _validator.IsAnomaly(metric, 50);

        Assert.False(result);
    }

    [Fact]
    public void IsWithinExpectedRange_ShouldThrowWhenMetricIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _validator.IsWithinExpectedRange(null!, 50));
    }

    [Fact]
    public void IsWarning_ShouldThrowWhenMetricIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _validator.IsWarning(null!, 50));
    }

    [Fact]
    public void IsCritical_ShouldThrowWhenMetricIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _validator.IsCritical(null!, 50));
    }

    [Fact]
    public void IsAnomaly_ShouldThrowWhenMetricIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _validator.IsAnomaly(null!, 50));
    }

    private static TelemetryMetric CreateMetric(
        double minimum,
        double maximum,
        double warning = 70,
        double critical = 90)
    {
        return new TelemetryMetric
        {
            SensorDeviceId = Guid.NewGuid(),
            MetricName = "Temperature",
            Unit = "°C",
            MinimumExpectedValue = minimum,
            MaximumExpectedValue = maximum,
            WarningThreshold = warning,
            CriticalThreshold = critical,
            IsEnabled = true
        };
    }
}