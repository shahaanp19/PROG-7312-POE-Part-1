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