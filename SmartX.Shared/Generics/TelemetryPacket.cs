using System.ComponentModel.DataAnnotations;
using SmartX.Shared.Enums;

namespace SmartX.Shared.Generics;

public class TelemetryPacket<T>
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid SensorDeviceId { get; set; }

    [Required]
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

    [Required]
    public T Value { get; set; } = default!;

    [Required]
    [MaxLength(100)]
    public string MetricName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Unit { get; set; } = string.Empty;

    public TelemetryDataType DataType { get; set; }

    public bool IsValid { get; set; } = true;

    public bool IsAnomaly { get; set; }

    public AlertSeverity? AlertSeverity { get; set; }

    public TelemetryPacket(
        Guid sensorDeviceId,
        string metricName,
        T value,
        string unit)
    {
        if (sensorDeviceId == Guid.Empty)
        {
            throw new ArgumentException(
                "Sensor device ID cannot be empty.",
                nameof(sensorDeviceId));
        }

        if (string.IsNullOrWhiteSpace(metricName))
        {
            throw new ArgumentException(
                "Metric name is required.",
                nameof(metricName));
        }

        if (string.IsNullOrWhiteSpace(unit))
        {
            throw new ArgumentException(
                "Unit is required.",
                nameof(unit));
        }

        SensorDeviceId = sensorDeviceId;
        MetricName = metricName.Trim();
        Value = value;
        Unit = unit.Trim();
        TimestampUtc = DateTime.UtcNow;
    }

    public TelemetryPacket()
    {
    }
}