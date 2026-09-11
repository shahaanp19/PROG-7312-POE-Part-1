namespace SmartX.Shared.DTOs;

public class DeploymentLocationDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string NodeId { get; set; } = string.Empty;

    public string Facility { get; set; } = string.Empty;

    public string Zone { get; set; } = string.Empty;

    public string SubZone { get; set; } = string.Empty;

    public Guid? ParentLocationId { get; set; }

    public List<DeploymentLocationDto> ChildLocations { get; set; } = [];
}