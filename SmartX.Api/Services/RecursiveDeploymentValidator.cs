using SmartX.Shared.Models;

namespace SmartX.Api.Services;

public sealed class RecursiveDeploymentValidator
{
    /// <summary>
    /// Recursively searches a deployment hierarchy for a target node.
    ///
    /// A node is only considered valid when every node on the path from
    /// the root to the target is configured.
    ///
    /// The method uses depth-first traversal and backtracking so that
    /// the returned path contains only the successful hierarchy branch.
    /// </summary>
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

        var visited = new HashSet<Guid>();

        return ValidateRecursive(
            node,
            targetNodeName.Trim(),
            path,
            visited);
    }

    private static bool ValidateRecursive(
        DeploymentNode node,
        string targetNodeName,
        List<string> path,
        HashSet<Guid> visited)
    {
        // Protects the recursive traversal against malformed cyclic
        // deployment structures.
        if (!visited.Add(node.Id))
        {
            return false;
        }

        // Every node on a valid deployment path must be configured.
        if (!node.IsConfigured)
        {
            visited.Remove(node.Id);
            return false;
        }

        path.Add(node.Name);

        // Base case: the requested node has been found.
        if (string.Equals(
            node.Name,
            targetNodeName,
            StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        // Recursive case: depth-first traversal of child deployments.
        for (var childIndex = 0;
             childIndex < node.Children.Count;
             childIndex++)
        {
            var child = node.Children[childIndex];

            if (child is null)
            {
                continue;
            }

            if (ValidateRecursive(
                child,
                targetNodeName,
                path,
                visited))
            {
                return true;
            }
        }

        // Backtrack so the path represents only the active branch.
        path.RemoveAt(path.Count - 1);
        visited.Remove(node.Id);

        return false;
    }
}