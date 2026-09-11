using SmartX.Shared.Models;

namespace SmartX.Api.Services;

public sealed class RecursiveDeploymentValidator
{
    public bool ValidateNode(
        DeploymentNode node,
        string targetNodeName,
        out List<string> path)
    {
        ArgumentNullException.ThrowIfNull(node);

        path = new List<string>();

        if (string.IsNullOrWhiteSpace(targetNodeName))
        {
            return false;
        }

        return ValidateRecursive(
            node,
            targetNodeName.Trim(),
            path);
    }

    private static bool ValidateRecursive(
        DeploymentNode node,
        string targetNodeName,
        List<string> path)
    {
        // A node must be configured before it can
        // form part of a valid deployment path.
        if (!node.IsConfigured)
        {
            return false;
        }

        // Add the current node to the active traversal path.
        path.Add(node.Name);

        // Base case:
        // the requested node has been found.
        if (string.Equals(
            node.Name,
            targetNodeName,
            StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        // Recursive case:
        // recursively search every child node.
        foreach (var child in node.Children)
        {
            if (ValidateRecursive(
                child,
                targetNodeName,
                path))
            {
                return true;
            }
        }

        // Backtrack:
        // remove the current node before returning to
        // the parent and trying another branch.
        path.RemoveAt(path.Count - 1);

        return false;
    }
}