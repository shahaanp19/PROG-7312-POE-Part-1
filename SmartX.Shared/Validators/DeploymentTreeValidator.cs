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


//References
//BillWagner (2026). Nullable reference types - C#. [online] MicrosoftLearn. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/null-safety/nullable-reference-types [Accessed 12 Sept. 2026].
//dotnet-bot (2026). IReadOnlyList Interface (System.Collections.Generic). [online] MicrosoftLearn. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1?view=net-10.0 [Accessed 12 Sept. 2026].