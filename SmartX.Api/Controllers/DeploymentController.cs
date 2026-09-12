using Microsoft.AspNetCore.Mvc;
using SmartX.Shared.DataStructures;
using SmartX.Shared.Validators;

namespace SmartX.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeploymentController : ControllerBase
{
    private readonly DeploymentTreeValidator _validator;

    public DeploymentController(DeploymentTreeValidator validator)
    {
        _validator = validator;
    }

    [HttpPost("validate")]
    public ActionResult<object> Validate(
        [FromBody] DeploymentNode deploymentTree)
    {
        if (deploymentTree is null)
        {
            return BadRequest(new
            {
                valid = false,
                message = "Deployment tree is required."
            });
        }

        bool isValid = _validator.Validate(deploymentTree);

        return Ok(new
        {
            valid = isValid,
            nodeId = deploymentTree.NodeId,
            validatedAtUtc = DateTime.UtcNow
        });
    }
}

//References
//tdykstra (2024). Create web APIs with ASP.NET Core. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-10.0 [Accessed 12 Sept. 2026].
//wadepickett (2025). Tutorial: Create a controller-based web API with ASP.NET Core. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-10.0&tabs=visual-studio [Accessed 12 Sept. 2026].
//tdykstra (2025). Model validation in ASP.NET Core MVC. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-10.0 [Accessed 12 Sept. 2026].
//tdykstra (2024). Dependency injection in ASP.NET Core. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-10.0 [Accessed 12 Sept. 2026].