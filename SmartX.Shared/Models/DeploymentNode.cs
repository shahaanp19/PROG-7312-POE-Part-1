namespace SmartX.Shared.Models;

public sealed class DeploymentNode
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Name { get; init; } = string.Empty;

    public string NodeType { get; init; } = string.Empty;

    public bool IsConfigured { get; init; }

    public List<DeploymentNode> Children { get; init; } = new();
}