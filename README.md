# SMART-X IoT Mesh Ecosystem

## Part 1 — Data Ingestion and Validation Gateway

**Module:** PROG-7312 POE Part 1  
**Project:** Smart-X IoT Mesh Ecosystem  

---

## Project Overview

Smart-X is an IoT Mesh Ecosystem designed to provide a scalable platform for monitoring, validating and managing connected IoT devices.

Part 1 implements the **Smart-X Data Ingestion and Validation Gateway**, providing the foundation for sensor registration, telemetry validation, anomaly detection, file attachment handling and operator-facing dashboard interaction.

The solution is divided into separate application layers so that the client dashboard communicates with the backend through a RESTful API rather than directly accessing backend services.


---


## Key Features

### Sensor Registration

Sensor registration supports:

- Sensor name
- MAC address / unique device identifier
- Sensor category
- Deployment location
- Description
- Active/inactive state

Supported sensor categories include:

- Environmental
- Power Consumption
- Actuator

Validation is performed before a sensor is registered to prevent invalid sensor information from entering the system.

### Telemetry Ingestion Pipeline

Incoming telemetry follows a structured validation and processing pipeline:

1. Receives telemetry metric.
2. Checks whether validation is enabled.
3. Verifies that the incoming value is finite.
4. Checks the expected operating range.
5. Checks the warning threshold.
6. Checks the critical threshold.
7. Identifies anomalies.
8. Assigns severity.
9. Produces a structured ingestion result.
10. Records processing information for dashboard feedback.

---

## Telemetry Validation

The Smart-X telemetry validation system checks incoming sensor values against configured operating limits.

### Expected Operating Range

Each telemetry metric can define a minimum and maximum expected operating range.

Values outside the configured range are classified as anomalies.

### Warning Threshold

When a telemetry value reaches or exceeds the configured warning threshold, the system identifies the event as a warning condition.

### Critical Threshold

When a telemetry value reaches or exceeds the configured critical threshold, the system identifies the event as a critical condition.

### Invalid Numeric Values

The telemetry validation process rejects:

- `NaN`
- Positive infinity
- Negative infinity

This prevents invalid floating-point values from entering downstream processing.

---

## Generic Telemetry Architecture

Smart-X uses a generic telemetry architecture based on `TelemetryPacket<T>`.

The generic design allows different telemetry payload types to be processed without unnecessary boxing or unboxing.

Benefits include:

- Compile-time type safety
- Reusable telemetry processing
- Support for heterogeneous IoT telemetry
- Strongly typed payload handling
- Reduced duplication between telemetry types

A generic architecture allows the same processing pipeline to work with different sensor payload structures while retaining their underlying types.

---

## Data Structures and Processing

The project demonstrates multiple C# data structures and processing techniques, including:

- Generic types
- Collections
- Sensor registries
- Telemetry history structures
- Structured telemetry models
- Nested deployment structures
- Validation result objects
- Sensor attachment metadata
- Dashboard engagement records

Raw historical telemetry may use multidimensional or jagged arrays before collection-based processing where required by the assessment.

Collections are then used to provide safer and more maintainable application-level data management.

---

## Shared Model

The `SmartX.Shared.Models` project contains strongly typed models shared between the API and Web applications.

The shared model layer provides structures for:

- Sensors
- Telemetry
- Dashboard engagement
- User issues
- Attachments
- Validation results
- Deployment information
- Configuration information

The models use strongly typed C# properties and collections. `init`-only properties are used where appropriate, while `IReadOnlyList<T>` is used where collections should be exposed as read-only data.

---

## Operator Overloading

Sensor-related data structures support operator overloading where appropriate.

Operator overloading allows objects to be compared using natural C# operators while keeping comparison logic within the relevant data structure.

This demonstrates object-oriented programming principles and allows domain-specific data structures to provide meaningful comparison behaviour.

---

## Recursive Deployment Validation

Smart-X contains a recursive deployment validation structure for nested device deployments and configuration profiles.

The validator checks whether the root deployment and all nested child deployments are present, enabled and configured.


### Complexity

For `N` deployment nodes:

- **Time complexity:** `O(N)`
- **Call-stack complexity:** `O(H)`, where `H` is the maximum hierarchy depth

---

## Sensor Attachments

The Smart-X platform supports sensor-related configuration and deployment attachments.

Supported attachment use cases include:

- Configuration files
- Deployment photographs
- Hardware logs

The upload interface provides:

- File type selection
- File extension validation
- File size validation
- Upload status feedback
- Uploaded file metadata
- Attachment information
- Encryption-related metadata where provided by the backend

The client currently limits uploads to a maximum size of **10 MB**.

---

## Web Dashboard

The Smart-X Web application provides an operator-facing dashboard implemented using Blazor and Razor Components.

The dashboard provides:

- Gateway status
- Gateway connectivity checking
- Sensor management
- Dynamic engagement information
- Incident information
- Priority attention queues
- Incident triage
- Issue reporting
- Live incident information
- Recent engagement activity
- System feedback
- Feature availability indicators

The interface uses a professional navy blue, blue, grey and white visual theme with clear feedback loops between operator actions and system state.

---

## Dynamic Engagement

The dashboard dynamically presents system engagement information, including:

- Total issues
- Open issues
- Critical issues
- Warning issues
- Informational issues
- Resolved issues
- Priority attention information
- Current attention level
- Recent engagement activity

The displayed engagement state changes according to gateway and incident state.

This provides actionable feedback to operators rather than displaying static dashboard information.

---

## Incident Triage

The dashboard provides incident triage functionality for reviewing and managing reported issues.

Supported operations include:

- Search
- Severity filtering
- Status filtering
- Sorting
- Priority ordering
- Resolution actions

Supported severity levels are:

- Critical
- Warning
- Information

The triage interface prioritises incidents requiring operator attention while allowing resolved and historical activity to remain visible.

---

## API Controllers

The Smart-X backend uses controller-based ASP.NET Core Web API architecture.

Controllers provide endpoints for:

- Sensor management
- Telemetry ingestion
- Dashboard engagement
- Attachments
- Health and status

Attribute routing is used with HTTP method attributes such as:

- `[HttpGet]`
- `[HttpPost]`
- `[HttpPut]`

Controllers delegate business operations to application services rather than implementing the underlying business logic directly.

This maintains separation between HTTP request handling and application logic.

### Dashboard API Endpoints

| HTTP Method | Endpoint | Purpose |
|---|---|---|
| GET | `/api/dashboard/engagement` | Retrieve dashboard engagement snapshot |
| GET | `/api/dashboard/issues` | Retrieve dashboard issues |
| GET | `/api/dashboard/issues/count` | Retrieve number of open issues |
| POST | `/api/dashboard/issues` | Create dashboard issue |
| PUT | `/api/dashboard/issues/{issueId}/resolve` | Resolve dashboard issue |

The dashboard controller uses dependency injection for the dashboard service and logging.

Expected responses include:

- `200 OK`
- `201 Created`
- `400 Bad Request`
- `404 Not Found`

Validation errors are returned through ASP.NET Core validation responses.

---

## Dependency Injection

Smart-X uses the built-in ASP.NET Core dependency injection container.

Services are registered using appropriate lifetimes such as:

- Scoped
- Singleton

Examples of dependency-injected services include:

- Telemetry validation
- Telemetry ingestion
- Sensor registry
- Sensor attachment
- Dashboard engagement
- Telemetry history
- Telemetry batch processing
- Recursive deployment validation

Constructor injection is used to provide dependencies to controllers and services.

This improves:

- Testability
- Maintainability
- Separation of concerns
- Service composition
- Dependency management

---

## Global Exception Handling

The Smart-X API contains custom global exception handling middleware.

The middleware:

1. Executes the next request pipeline component.
2. Detects unhandled exceptions.
3. Logs the exception.
4. Records the HTTP method and request path.
5. Returns HTTP `500 Internal Server Error`.
6. Returns a controlled JSON response.
7. Prevents internal exception details from being exposed to the client.

---

## API Health Monitoring

The Smart-X API exposes a health endpoint:

```text
GET /health
```

A successful health response includes:

- Application name
- Current status
- UTC timestamp

The Smart-X Web dashboard uses this endpoint to provide gateway connectivity feedback.

The health endpoint allows the frontend to determine whether the API gateway is reachable without relying on a normal application operation.

---

## HTTP Client Integration

The Blazor Web application communicates with the API using `IHttpClientFactory`.

A named HTTP client called `SmartXApi` is configured with a configuration-driven API base URL.

The Web application uses asynchronous HTTP operations including:

- `GetAsync`
- `GetFromJsonAsync`
- `PostAsJsonAsync`
- `PutAsync`

The client handles:

- Successful responses
- Validation failures
- Timeouts
- Connection failures

A request timeout is configured to prevent the user interface from waiting indefinitely for an unavailable API.

The health-check request uses:

```csharp
GetAsync("health", ...)
```

rather than an empty request path.

---

## Configuration

The Smart-X Web application reads the API base URL from application configuration.

The application validates that the configured API base URL is an absolute URI.

This avoids hard-coding API addresses directly into application logic and makes the system easier to configure between development and deployment environments.

---

## Swagger 

Swagger/OpenAPI documentation is enabled for the Smart-X API during development.

The Swagger interface can be accessed at:

```text
https://localhost:7280/swagger
```
Swagger provides an interactive method for verifying API functionality without requiring the Web dashboard.

---

## Error Handling

The Smart-X solution uses multiple levels of error handling.

### API

The API uses global exception middleware to catch unexpected server-side exceptions.

### Web Application

The Web application provides user-facing error feedback when API requests fail.

### Routing

Invalid application routes are redirected to not-found handling where appropriate.

### Expected Errors

Expected client-side or validation errors are returned as appropriate `4xx` responses.

### Unexpected Errors

Unexpected server-side failures return a controlled `500` response without exposing internal implementation details.

---

## Security and Application Configuration

The Smart-X solution includes basic application security and environment-specific configuration.

### Web Application

The Web application uses:

- HTTPS redirection
- Antiforgery protection
- HTTPS development endpoints
- Environment-specific error handling
- Configuration-based API URLs
- HSTS outside Development

### API

The API uses:

- HTTPS redirection
- CORS
- Global exception middleware
- Controller routing

Sensitive values should not be committed to source control.

The repository uses `.gitignore` to prevent inappropriate development and environment files from being committed.

---

## CORS Configuration

The API exposes a CORS policy to allow the Smart-X Web application to communicate with the Smart-X API across origins when required.

The policy supports the HTTP methods, headers and cross-origin requests required by the development application.

The current development configuration may be permissive to simplify local integration.

For production deployment, CORS should be restricted to explicitly trusted origins.

---

## Validation Architecture

Validation is distributed across multiple areas of the Smart-X solution.

### Sensor Validation

Validates sensor registration information such as:

- Required fields
- Sensor identity
- Category
- Configuration

### Telemetry Validation

Validates:

- Numeric values
- Finite values
- Operating ranges
- Warning thresholds
- Critical thresholds

### Deployment Validation

Recursively validates nested deployment structures.

### File Validation

Validates:

- File extension
- File type
- File size

### Model Validation

ASP.NET Core model binding and validation are used to validate API request models.

The validation architecture prevents invalid information from entering downstream processing.

---

## Technology Stack

| Technology | Purpose |
|---|---|
| C# | Primary programming language |
| .NET | Application platform |
| ASP.NET Core Web API | Backend REST API |
| Blazor | Web application framework |
| Razor Components | Interactive UI components |
| HTTP | Client-server communication |
| REST API | Backend API architecture |
| JSON | Data interchange |
| Swagger/OpenAPI | API documentation |
| Dependency Injection | Service composition |
| `IHttpClientFactory` | HTTP client management |
| Generic C# Types | Strongly typed reusable telemetry processing |
| Collections | Data management |
| Recursive Algorithms | Deployment validation |
| Git | Version control |
| GitHub | Repository and source control |

---

## Prerequisites

The following software is required:

- .NET SDK compatible with the project target framework
- Git
- Visual Studio or Visual Studio Code
- Modern web browser

Verify the installed .NET SDK:

```bash
dotnet --version
```

Verify Git:

```bash
git --version
```

## Clone the Repository

Clone the GitHub repository:

```bash
git clone https://github.com/shahaanp19/PROG-7312-POE-Part-1.git
```

Move into the project directory:

```bash
cd PROG-7312-POE-Part-1
```

## Restore Dependencies

Restore all project dependencies:

```bash
dotnet restore
```

This restores the NuGet packages and project dependencies required by the solution.

## Build the Solution

Build the solution using:

```bash
dotnet build
```

A successful build confirms that the projects compile and that the required dependencies can be resolved.

## Run the Smart-X API

Start the API project with:

```bash
dotnet run --project SmartX.Api
```

The API exposes the configured HTTPS endpoint.

The default development API address is:

```text
https://localhost:7280/
```

## Run the Smart-X Web Application

Start the Web application using:

```bash
dotnet run --project SmartX.Web
```

The Blazor application communicates with the API using the configured `SmartXApi:BaseUrl` setting.

## Swagger API Documentation

Once the API is running, open:

```text
https://localhost:7280/swagger
```

Swagger can be used to inspect and test the API endpoints.

---

## API Health Check

The API health endpoint can be accessed at:

```text
https://localhost:7280/health
```

The endpoint is used by the dashboard to determine whether the Smart-X API gateway is available.

---


## Author

**Author:** Shahaan Pillay - ST10438099

**Module:** PROG-7312 POE Part 1

**Project:** Smart-X IoT Mesh Ecosystem

---


GitHub repository:

https://github.com/shahaanp19/PROG-7312-POE-Part-1.git

The repository contains the Smart-X API, Web application, shared models, validators, data structures, configuration and supporting documentation required for Part 1.
