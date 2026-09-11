using System.ComponentModel.DataAnnotations;

namespace SmartX.Shared.Models;

public class DeploymentLocation
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string NodeId { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Facility { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Zone { get; set; } = string.Empty;

    [MaxLength(100)]
    public string SubZone { get; set; } = string.Empty;

    public Guid? ParentLocationId { get; set; }

    public DeploymentLocation? ParentLocation { get; set; }

    public List<DeploymentLocation> ChildLocations { get; set; } = [];
}