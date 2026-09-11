using System.ComponentModel.DataAnnotations;

namespace SmartX.Shared.DataStructures;

public class DeploymentNode
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string NodeId { get; set; } = string.Empty;

    public bool IsEnabled { get; set; } = true;

    public bool IsConfigured { get; set; }

    public List<DeploymentNode> Children { get; set; } = [];
}