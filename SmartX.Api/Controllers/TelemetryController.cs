using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Services;
using SmartX.Shared.DTOs;
using SmartX.Shared.Models;

namespace SmartX.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TelemetryController : ControllerBase
{
    private readonly TelemetryIngestionService _ingestionService;
    private readonly RecursiveDeploymentValidator _deploymentValidator;
    private readonly SensorRegistryService _sensorRegistry;
    private readonly TelemetryBatchProcessor _batchProcessor;

    public TelemetryController(
        TelemetryIngestionService ingestionService,
        RecursiveDeploymentValidator deploymentValidator,
        SensorRegistryService sensorRegistry,
        TelemetryBatchProcessor batchProcessor)
    {
        _ingestionService = ingestionService;
        _deploymentValidator = deploymentValidator;
        _sensorRegistry = sensorRegistry;
        _batchProcessor = batchProcessor;
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "Healthy",
            service = "Telemetry API",
            timestampUtc = DateTime.UtcNow
        });
    }

    [HttpPost("ingest")]
    public ActionResult<TelemetryIngestionResult> Ingest(
        [FromBody] TelemetryIngestionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (request.SensorDeviceId == Guid.Empty)
        {
            return BadRequest(new
            {
                message =
                    "Sensor device identifier must be a valid GUID."
            });
        }

        if (!_sensorRegistry.Contains(
                request.SensorDeviceId))
        {
            return NotFound(new
            {
                message =
                    "The specified sensor is not registered.",
                sensorDeviceId =
                    request.SensorDeviceId
            });
        }

        if (string.IsNullOrWhiteSpace(
                request.MetricName))
        {
            return BadRequest(new
            {
                message =
                    "Telemetry metric name is required."
            });
        }

        if (string.IsNullOrWhiteSpace(
                request.Unit))
        {
            return BadRequest(new
            {
                message =
                    "Telemetry unit is required."
            });
        }

        if (double.IsNaN(request.Value) ||
            double.IsInfinity(request.Value))
        {
            return BadRequest(new
            {
                message =
                    "Telemetry value must be a finite number."
            });
        }

        if (request.MinimumExpectedValue >
            request.MaximumExpectedValue)
        {
            return BadRequest(new
            {
                message =
                    "Minimum expected value cannot exceed " +
                    "maximum expected value."
            });
        }

        if (request.WarningThreshold >
            request.CriticalThreshold)
        {
            return BadRequest(new
            {
                message =
                    "Warning threshold cannot exceed " +
                    "critical threshold."
            });
        }

        var metric = new TelemetryMetric
        {
            SensorDeviceId =
                request.SensorDeviceId,

            MetricName =
                request.MetricName.Trim(),

            Unit =
                request.Unit.Trim(),

            MinimumExpectedValue =
                request.MinimumExpectedValue,

            MaximumExpectedValue =
                request.MaximumExpectedValue,

            WarningThreshold =
                request.WarningThreshold,

            CriticalThreshold =
                request.CriticalThreshold,

            IsEnabled =
                request.IsEnabled
        };

        var result =
            _ingestionService.Process(
                metric,
                request.Value);

        if (!result.Success)
        {
            return UnprocessableEntity(result);
        }

        return Ok(result);
    }

    [HttpGet("generic-demo")]
    public IActionResult GenericDemo()
    {
        var sensorId = Guid.NewGuid();

        var temperature =
            new SmartX.Shared.Generics.TelemetryPacket<float>(
                sensorId,
                "Temperature",
                24.5f,
                "°C");

        var power =
            new SmartX.Shared.Generics.TelemetryPacket<int>(
                sensorId,
                "Power",
                850,
                "W");

        var relay =
            new SmartX.Shared.Generics.TelemetryPacket<bool>(
                sensorId,
                "Relay",
                true,
                "state");

        var readingOne =
            new MeterReading(
                sensorId,
                120.5,
                "W");

        var readingTwo =
            new MeterReading(
                sensorId,
                80.25,
                "W");

        var aggregate = readingOne + readingTwo;
        var delta = readingOne - readingTwo;

        return Ok(new
        {
            demonstration =
                "Generics and operator overloading",

            genericPackets = new
            {
                temperature,
                power,
                relay
            },

            operatorOverloading = new
            {
                addition = new
                {
                    operation =
                        "readingOne + readingTwo",
                    result = aggregate
                },

                subtraction = new
                {
                    operation =
                        "readingOne - readingTwo",
                    result = delta
                }
            }
        });
    }

    [HttpGet("recursive-validation-demo")]
    public IActionResult RecursiveValidationDemo()
    {
        var facility = new DeploymentNode
        {
            Name = "Facility A",
            NodeType = "Facility",
            IsConfigured = true,
            Children =
            {
                new DeploymentNode
                {
                    Name = "Zone 1",
                    NodeType = "Zone",
                    IsConfigured = true,
                    Children =
                    {
                        new DeploymentNode
                        {
                            Name = "Sub-Zone A",
                            NodeType = "Sub-Zone",
                            IsConfigured = true,
                            Children =
                            {
                                new DeploymentNode
                                {
                                    Name = "Area 1",
                                    NodeType = "Area",
                                    IsConfigured = true,
                                    Children =
                                    {
                                        new DeploymentNode
                                        {
                                            Name = "Cabinet 1",
                                            NodeType = "Cabinet",
                                            IsConfigured = true
                                        }
                                    }
                                }
                            }
                        },

                        new DeploymentNode
                        {
                            Name = "Sub-Zone B",
                            NodeType = "Sub-Zone",
                            IsConfigured = true,
                            Children =
                            {
                                new DeploymentNode
                                {
                                    Name = "Area 2",
                                    NodeType = "Area",
                                    IsConfigured = true,
                                    Children =
                                    {
                                        new DeploymentNode
                                        {
                                            Name = "Cabinet 2",
                                            NodeType = "Cabinet",
                                            IsConfigured = true,
                                            Children =
                                            {
                                                new DeploymentNode
                                                {
                                                    Name =
                                                        "Temperature Sensor",
                                                    NodeType = "Sensor",
                                                    IsConfigured = true
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },

                new DeploymentNode
                {
                    Name = "Zone 2",
                    NodeType = "Zone",
                    IsConfigured = true,
                    Children =
                    {
                        new DeploymentNode
                        {
                            Name = "Sub-Zone C",
                            NodeType = "Sub-Zone",
                            IsConfigured = true
                        }
                    }
                }
            }
        };

        const string targetNode =
            "Temperature Sensor";

        var isValid =
            _deploymentValidator.ValidateNode(
                facility,
                targetNode,
                out var path);

        return Ok(new
        {
            demonstration =
                "Recursive deployment validation",

            targetNode,

            isValid,

            hierarchyDepth =
                path.Count,

            path,

            validationResult =
                isValid
                    ? "Target exists on a fully configured deployment path."
                    : "Target is unavailable on a fully configured deployment path."
        });
    }

    [HttpGet("recursive-validation-failure-demo")]
    public IActionResult RecursiveValidationFailureDemo()
    {
        var facility = new DeploymentNode
        {
            Name = "Facility A",
            NodeType = "Facility",
            IsConfigured = true,
            Children =
            {
                new DeploymentNode
                {
                    Name = "Zone 1",
                    NodeType = "Zone",
                    IsConfigured = true,
                    Children =
                    {
                        new DeploymentNode
                        {
                            Name = "Unconfigured Zone",
                            NodeType = "Zone",
                            IsConfigured = false,
                            Children =
                            {
                                new DeploymentNode
                                {
                                    Name = "Hidden Sensor",
                                    NodeType = "Sensor",
                                    IsConfigured = true
                                }
                            }
                        }
                    }
                }
            }
        };

        const string targetNode =
            "Hidden Sensor";

        var isValid =
            _deploymentValidator.ValidateNode(
                facility,
                targetNode,
                out var path);

        return Ok(new
        {
            demonstration =
                "Recursive validation failure handling",

            targetNode,

            isValid,

            hierarchyDepth =
                path.Count,

            path,

            expectedResult = false,

            reason =
                "The target exists below an unconfigured deployment node, " +
                "so the recursive validator correctly rejects the path."
        });
    }

    [HttpGet("recursive-validation-missing-demo")]
    public IActionResult RecursiveValidationMissingDemo()
    {
        var facility = new DeploymentNode
        {
            Name = "Facility A",
            NodeType = "Facility",
            IsConfigured = true,
            Children =
            {
                new DeploymentNode
                {
                    Name = "Zone 1",
                    NodeType = "Zone",
                    IsConfigured = true
                },

                new DeploymentNode
                {
                    Name = "Zone 2",
                    NodeType = "Zone",
                    IsConfigured = true
                }
            }
        };

        const string targetNode =
            "Nonexistent Sensor";

        var isValid =
            _deploymentValidator.ValidateNode(
                facility,
                targetNode,
                out var path);

        return Ok(new
        {
            demonstration =
                "Recursive missing-target handling",

            targetNode,

            isValid,

            hierarchyDepth =
                path.Count,

            path,

            expectedResult = false,

            validationResult =
                "The requested target does not exist in the deployment hierarchy."
        });
    }

    [HttpGet("historical-batch-demo")]
    public IActionResult HistoricalBatchDemo()
    {
        // Jagged array:
        // each batch owns only the memory required for its readings.
        // This avoids padding every batch to the size of the largest batch.
        float[][] historicalBatches =
        {
            new float[] { 21.5f, 22.1f, 22.8f },
            new float[] { 23.2f, 24.0f },
            new float[] { 24.7f, 25.1f, 25.6f, 26.0f }
        };

        var packets =
            _batchProcessor.ProcessHistoricalBatches(
                historicalBatches);

        // Rectangular representation for matrix-oriented processing.
        var matrix =
            _batchProcessor.CreateTelemetryMatrix(
                historicalBatches);

        // Contiguous one-dimensional representation for sequential
        // processing and bulk operations.
        var flattened =
            _batchProcessor.FlattenTelemetryBatches(
                historicalBatches);

        var matrixRows =
            matrix.GetLength(0);

        var matrixColumns =
            matrix.GetLength(1);

        var matrixData =
            new List<float[]>(matrixRows);

        for (var row = 0;
             row < matrixRows;
             row++)
        {
            var rowData =
                new float[matrixColumns];

            for (var column = 0;
                 column < matrixColumns;
                 column++)
            {
                rowData[column] =
                    matrix[row, column];
            }

            matrixData.Add(rowData);
        }

        var batchSizes =
            new int[historicalBatches.Length];

        var totalReadings = 0;

        for (var index = 0;
             index < historicalBatches.Length;
             index++)
        {
            var batch =
                historicalBatches[index];

            var size =
                batch?.Length ?? 0;

            batchSizes[index] =
                size;

            totalReadings +=
                size;
        }

        var largestBatchSize = 0;

        for (var index = 0;
             index < batchSizes.Length;
             index++)
        {
            if (batchSizes[index] >
                largestBatchSize)
            {
                largestBatchSize =
                    batchSizes[index];
            }
        }

        return Ok(new
        {
            demonstration =
                "Jagged arrays, multidimensional arrays, contiguous buffers and generic collections",

            sourceData = new
            {
                batchCount =
                    historicalBatches.Length,

                batchSizes,

                totalReadings,

                largestBatchSize
            },

            memoryLayout = new
            {
                jaggedRepresentation =
                    "Variable-length arrays with no padding between batches",

                rectangularRepresentation =
                    "Row-major multidimensional array for matrix-oriented processing",

                contiguousRepresentation =
                    "One-dimensional float buffer for sequential processing",

                flattenedElementCount =
                    flattened.Length
            },

            collectionProcessing = new
            {
                readingsTransferredToList =
                    packets.Count,

                collectionType =
                    "List<SmartX.Shared.Generics.TelemetryPacket<float>>"
            },

            packets,

            flattened,

            matrix = new
            {
                rows =
                    matrixRows,

                columns =
                    matrixColumns,

                data =
                    matrixData
            },

            validation = new
            {
                packetCountMatchesSource =
                    packets.Count == totalReadings,

                flattenedCountMatchesSource =
                    flattened.Length == totalReadings,

                matrixDimensionsValid =
                    matrixRows ==
                        historicalBatches.Length &&
                    matrixColumns ==
                        largestBatchSize
            }
        });
    }
}

//References
//tdykstra (2024). Create web APIs with ASP.NET Core. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-10.0 [Accessed 12 Sept. 2026].
//wadepickett (2025). Tutorial: Create a controller-based web API with ASP.NET Core. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-10.0&tabs=visual-studio [Accessed 12 Sept. 2026].
//tdykstra (2025). Model validation in ASP.NET Core MVC. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-10.0 [Accessed 12 Sept. 2026].
//tdykstra (2024). Dependency injection in ASP.NET Core. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-10.0 [Accessed 12 Sept. 2026].