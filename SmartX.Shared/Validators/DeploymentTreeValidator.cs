using SmartX.Shared.DataStructures;

namespace SmartX.Shared.Validators;

public class DeploymentTreeValidator
{
    public bool Validate(DeploymentNode? node)
    {
        if (node is null)
        {
            return false;
        }

        if (!node.IsEnabled || !node.IsConfigured)
        {
            return false;
        }

        return ValidateChildrenRecursively(node.Children);
    }

    private bool ValidateChildrenRecursively(
        IReadOnlyList<DeploymentNode> children)
    {
        if (children.Count == 0)
        {
            return true;
        }

        foreach (DeploymentNode child in children)
        {
            if (!child.IsEnabled || !child.IsConfigured)
            {
                return false;
            }

            if (!ValidateChildrenRecursively(child.Children))
            {
                return false;
            }
        }

        return true;
    }
}