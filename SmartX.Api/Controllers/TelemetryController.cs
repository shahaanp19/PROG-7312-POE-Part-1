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

    public TelemetryController(
        TelemetryIngestionService ingestionService,
        RecursiveDeploymentValidator deploymentValidator)
    {
        _ingestionService = ingestionService;
        _deploymentValidator = deploymentValidator;
    }

    // ---------------------------------------------------------
    // TELEMETRY HEALTH
    // ---------------------------------------------------------

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

    // ---------------------------------------------------------
    // TELEMETRY INGESTION
    // ---------------------------------------------------------

    [HttpPost("ingest")]
    public ActionResult<TelemetryIngestionResult> Ingest(
        [FromBody] TelemetryIngestionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var metric = new TelemetryMetric
        {
            SensorDeviceId = request.SensorDeviceId,
            MetricName = request.MetricName,
            Unit = request.Unit,
            MinimumExpectedValue = request.MinimumExpectedValue,
            MaximumExpectedValue = request.MaximumExpectedValue,
            WarningThreshold = request.WarningThreshold,
            CriticalThreshold = request.CriticalThreshold,
            IsEnabled = request.IsEnabled
        };

        var result = _ingestionService.Process(
            metric,
            request.Value);

        return Ok(result);
    }

    // ---------------------------------------------------------
    // SECTION 3
    // GENERICS & OPERATOR OVERLOADING
    // ---------------------------------------------------------

    [HttpGet("generic-demo")]
    public IActionResult GenericDemo()
    {
        var sensorId = Guid.NewGuid();

        // Fully qualified type names are intentional.
        // This avoids conflicts with any other TelemetryPacket
        // definitions in the solution.

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

        // Operator overloading demonstrations.
        var aggregate = readingOne + readingTwo;
        var delta = readingOne - readingTwo;

        return Ok(new
        {
            demonstration = "Generics and operator overloading",

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
                    operation = "readingOne + readingTwo",
                    result = aggregate
                },

                subtraction = new
                {
                    operation = "readingOne - readingTwo",
                    result = delta
                }
            }
        });
    }

    // ---------------------------------------------------------
    // SECTION 4
    // RECURSIVE DEPLOYMENT VALIDATION
    // ---------------------------------------------------------

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
                                                    Name = "Temperature Sensor",
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

        const string targetNode = "Temperature Sensor";

        var isValid =
            _deploymentValidator.ValidateNode(
                facility,
                targetNode,
                out var path);

        return Ok(new
        {
            demonstration = "Recursive deployment validation",
            targetNode,
            isValid,
            hierarchyDepth = path.Count,
            path,

            validationResult =
                isValid
                    ? "Target exists on a fully configured deployment path."
                    : "Target is unavailable on a fully configured deployment path."
        });
    }

    // ---------------------------------------------------------
    // SECTION 4
    // RECURSIVE VALIDATION FAILURE DEMO
    // ---------------------------------------------------------

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

        const string targetNode = "Hidden Sensor";

        var isValid =
            _deploymentValidator.ValidateNode(
                facility,
                targetNode,
                out var path);

        return Ok(new
        {
            demonstration = "Recursive validation failure handling",
            targetNode,
            isValid,
            hierarchyDepth = path.Count,
            path,

            expectedResult = false,

            reason =
                "The target exists below an unconfigured deployment node, " +
                "so the recursive validator correctly rejects the path."
        });
    }

    // ---------------------------------------------------------
    // SECTION 4
    // RECURSIVE VALIDATION MISSING TARGET DEMO
    // ---------------------------------------------------------

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

        const string targetNode = "Nonexistent Sensor";

        var isValid =
            _deploymentValidator.ValidateNode(
                facility,
                targetNode,
                out var path);

        return Ok(new
        {
            demonstration = "Recursive missing-target handling",
            targetNode,
            isValid,
            hierarchyDepth = path.Count,
            path,

            expectedResult = false,

            validationResult =
                "The requested target does not exist in the deployment hierarchy."
        });
    }

    // ---------------------------------------------------------
    // SECTION 4
    // HISTORICAL TELEMETRY BATCH PROCESSING
    // ---------------------------------------------------------

    [HttpGet("historical-batch-demo")]
    public IActionResult HistoricalBatchDemo(
        [FromServices] TelemetryBatchProcessor batchProcessor)
    {
        float[][] historicalBatches =
        {
            new float[] { 21.5f, 22.1f, 22.8f },
            new float[] { 23.2f, 24.0f },
            new float[] { 24.7f, 25.1f, 25.6f, 26.0f }
        };

        // Process jagged array data into a generic List.
        var packets =
            batchProcessor.ProcessHistoricalBatches(
                historicalBatches);

        // Convert the jagged array into a rectangular
        // multidimensional matrix.
        var matrix =
            batchProcessor.CreateTelemetryMatrix(
                historicalBatches);

        var matrixRows = matrix.GetLength(0);
        var matrixColumns = matrix.GetLength(1);

        // Convert the multidimensional matrix into a
        // collection suitable for JSON serialization.
        var matrixData = new List<float[]>(matrixRows);

        for (var row = 0; row < matrixRows; row++)
        {
            var rowData = new float[matrixColumns];

            for (var column = 0;
                 column < matrixColumns;
                 column++)
            {
                rowData[column] = matrix[row, column];
            }

            matrixData.Add(rowData);
        }

        var batchSizes =
            historicalBatches
                .Select(batch => batch?.Length ?? 0)
                .ToArray();

        var totalReadings =
            batchSizes.Sum();

        return Ok(new
        {
            demonstration = "Jagged arrays, multidimensional arrays and generic collections",

            sourceData = new
            {
                batchCount = historicalBatches.Length,
                batchSizes,
                totalReadings
            },

            collectionProcessing = new
            {
                readingsTransferredToList = packets.Count,
                collectionType =
                    "List<SmartX.Shared.Generics.TelemetryPacket<float>>"
            },

            packets,

            matrix = new
            {
                rows = matrixRows,
                columns = matrixColumns,
                data = matrixData
            },

            validation = new
            {
                packetCountMatchesSource =
                    packets.Count == totalReadings,

                matrixDimensionsValid =
                    matrixRows == historicalBatches.Length &&
                    matrixColumns == batchSizes.Max()
            }
        });
    }
}