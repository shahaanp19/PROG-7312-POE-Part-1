using SmartX.Shared.Enums;
using SmartX.Shared.Models;
using SmartX.Shared.Validators;

namespace SmartX.Tests.Validation;

public class TelemetryValidatorTests
{
    private readonly TelemetryValidator _validator;

    public TelemetryValidatorTests()
    {
        var thresholdValidator = new TelemetryThresholdValidator();
        _validator = new TelemetryValidator(thresholdValidator);
    }

    [Fact]
    public void Validate_ShouldAcceptNormalTelemetry()
    {
        var metric = CreateMetric();

        var result = _validator.Validate(metric, 25);

        Assert.True(result.IsValid);
        Assert.False(result.IsAnomaly);
        Assert.Equal(AlertSeverity.Info, result.Severity);
        Assert.Empty(result.Errors);
        Assert.Empty(result.Warnings);
    }

    [Fact]
    public void Validate_ShouldDetectValueAboveExpectedRange()
    {
        var metric = CreateMetric(
            minimum: 10,
            maximum: 40,
            warning: 50,
            critical: 70);

        var result = _validator.Validate(metric, 45);

        Assert.True(result.IsValid);
        Assert.True(result.IsAnomaly);
        Assert.Equal(AlertSeverity.Info, result.Severity);
        Assert.NotEmpty(result.Warnings);
    }

    [Fact]
    public void Validate_ShouldDetectWarningThreshold()
    {
        var metric = CreateMetric(
            minimum: 10,
            maximum: 100,
            warning: 70,
            critical: 90);

        var result = _validator.Validate(metric, 75);

        Assert.True(result.IsValid);
        Assert.True(result.IsAnomaly);
        Assert.Equal(AlertSeverity.Warning, result.Severity);
        Assert.NotEmpty(result.Warnings);
    }

    [Fact]
    public void Validate_ShouldDetectCriticalThreshold()
    {
        var metric = CreateMetric(
            minimum: 10,
            maximum: 100,
            warning: 70,
            critical: 90);

        var result = _validator.Validate(metric, 95);

        Assert.True(result.IsValid);
        Assert.True(result.IsAnomaly);
        Assert.Equal(AlertSeverity.Critical, result.Severity);
        Assert.NotEmpty(result.Warnings);
    }

    [Fact]
    public void Validate_ShouldTreatCriticalThresholdAsCritical()
    {
        var metric = CreateMetric(
            minimum: 10,
            maximum: 100,
            warning: 70,
            critical: 90);

        var result = _validator.Validate(metric, 90);

        Assert.True(result.IsAnomaly);
        Assert.Equal(AlertSeverity.Critical, result.Severity);
    }

    [Fact]
    public void Validate_ShouldRejectNaN()
    {
        var metric = CreateMetric();

        var result = _validator.Validate(metric, double.NaN);

        Assert.False(result.IsValid);
        Assert.True(result.IsAnomaly);
        Assert.Equal(AlertSeverity.Critical, result.Severity);
        Assert.Contains(
            result.Errors,
            error => error.Contains("finite"));
    }

    [Fact]
    public void Validate_ShouldRejectPositiveInfinity()
    {
        var metric = CreateMetric();

        var result = _validator.Validate(
            metric,
            double.PositiveInfinity);

        Assert.False(result.IsValid);
        Assert.True(result.IsAnomaly);
        Assert.Equal(AlertSeverity.Critical, result.Severity);
    }

    [Fact]
    public void Validate_ShouldRejectNegativeInfinity()
    {
        var metric = CreateMetric();

        var result = _validator.Validate(
            metric,
            double.NegativeInfinity);

        Assert.False(result.IsValid);
        Assert.True(result.IsAnomaly);
        Assert.Equal(AlertSeverity.Critical, result.Severity);
    }

    [Fact]
    public void Validate_ShouldRejectDisabledMetric()
    {
        var metric = CreateMetric();
        metric.IsEnabled = false;

        var result = _validator.Validate(metric, 25);

        Assert.False(result.IsValid);
        Assert.False(result.IsAnomaly);
        Assert.Contains(
            result.Errors,
            error => error.Contains("disabled"));
    }

    [Fact]
    public void Validate_ShouldRejectInvalidThresholdConfiguration()
    {
        var metric = CreateMetric(
            minimum: 100,
            maximum: 10,
            warning: 70,
            critical: 90);

        var result = _validator.Validate(metric, 50);

        Assert.False(result.IsValid);
        Assert.True(result.IsAnomaly);
        Assert.Equal(AlertSeverity.Critical, result.Severity);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Validate_ShouldRejectWarningAboveCriticalThreshold()
    {
        var metric = CreateMetric(
            minimum: 0,
            maximum: 100,
            warning: 90,
            critical: 70);

        var result = _validator.Validate(metric, 50);

        Assert.False(result.IsValid);
        Assert.True(result.IsAnomaly);
        Assert.Equal(AlertSeverity.Critical, result.Severity);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Validate_ShouldThrowWhenMetricIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _validator.Validate(null!, 25));
    }

    [Fact]
    public void Constructor_ShouldThrowWhenThresholdValidatorIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TelemetryValidator(null!));
    }

    private static TelemetryMetric CreateMetric(
        double minimum = 10,
        double maximum = 100,
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