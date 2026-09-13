using SmartX.Api.Services;
using SmartX.Shared.Models;

namespace SmartX.Tests.Services;

public class RecursiveDeploymentValidatorTests
{
    private readonly RecursiveDeploymentValidator _validator = new();

    [Fact]
    public void ValidateNode_ShouldFindRootNode()
    {
        // Arrange
        var root = CreateNode(
            "Facility A",
            true);

        // Act
        var result =
            _validator.ValidateNode(
                root,
                "Facility A",
                out var path);

        // Assert
        Assert.True(result);

        Assert.Equal(
            new[] { "Facility A" },
            path);
    }

    [Fact]
    public void ValidateNode_ShouldFindDeepNestedNode()
    {
        // Arrange
        var root = CreateNode("Facility A", true);

        var zone = CreateNode("Zone 1", true);
        var subZone = CreateNode("Sub-Zone B", true);
        var node = CreateNode("Node 42", true);

        root.Children.Add(zone);
        zone.Children.Add(subZone);
        subZone.Children.Add(node);

        // Act
        var result =
            _validator.ValidateNode(
                root,
                "Node 42",
                out var path);

        // Assert
        Assert.True(result);

        Assert.Equal(
            new[]
            {
                "Facility A",
                "Zone 1",
                "Sub-Zone B",
                "Node 42"
            },
            path);
    }

    [Fact]
    public void ValidateNode_ShouldReturnFalseWhenNodeDoesNotExist()
    {
        // Arrange
        var root = CreateNode("Facility A", true);

        root.Children.Add(
            CreateNode("Zone 1", true));

        // Act
        var result =
            _validator.ValidateNode(
                root,
                "Missing Node",
                out var path);

        // Assert
        Assert.False(result);
        Assert.Empty(path);
    }

    [Fact]
    public void ValidateNode_ShouldRejectUnconfiguredTarget()
    {
        // Arrange
        var root = CreateNode("Facility A", true);

        root.Children.Add(
            CreateNode("Zone 1", false));

        // Act
        var result =
            _validator.ValidateNode(
                root,
                "Zone 1",
                out var path);

        // Assert
        Assert.False(result);
        Assert.Empty(path);
    }

    [Fact]
    public void ValidateNode_ShouldRejectPathContainingUnconfiguredParent()
    {
        // Arrange
        var root = CreateNode("Facility A", true);

        var zone = CreateNode("Zone 1", false);
        var node = CreateNode("Node 42", true);

        root.Children.Add(zone);
        zone.Children.Add(node);

        // Act
        var result =
            _validator.ValidateNode(
                root,
                "Node 42",
                out var path);

        // Assert
        Assert.False(result);
        Assert.Empty(path);
    }

    [Fact]
    public void ValidateNode_ShouldBacktrackBetweenBranches()
    {
        // Arrange
        var root = CreateNode("Facility A", true);

        var zone1 = CreateNode("Zone 1", true);
        var zone2 = CreateNode("Zone 2", true);

        var wrongNode = CreateNode("Wrong Node", true);
        var targetNode = CreateNode("Target Node", true);

        zone1.Children.Add(wrongNode);
        zone2.Children.Add(targetNode);

        root.Children.Add(zone1);
        root.Children.Add(zone2);

        // Act
        var result =
            _validator.ValidateNode(
                root,
                "Target Node",
                out var path);

        // Assert
        Assert.True(result);

        Assert.Equal(
            new[]
            {
                "Facility A",
                "Zone 2",
                "Target Node"
            },
            path);
    }

    [Fact]
    public void ValidateNode_ShouldHandleCaseInsensitiveTargetNames()
    {
        // Arrange
        var root =
            CreateNode("Facility A", true);

        root.Children.Add(
            CreateNode("Zone 1", true));

        // Act
        var result =
            _validator.ValidateNode(
                root,
                "zone 1",
                out var path);

        // Assert
        Assert.True(result);

        Assert.Equal(
            new[]
            {
                "Facility A",
                "Zone 1"
            },
            path);
    }

    [Fact]
    public void ValidateNode_ShouldTrimTargetName()
    {
        // Arrange
        var root =
            CreateNode("Facility A", true);

        root.Children.Add(
            CreateNode("Zone 1", true));

        // Act
        var result =
            _validator.ValidateNode(
                root,
                "  Zone 1  ",
                out var path);

        // Assert
        Assert.True(result);

        Assert.Equal(
            new[]
            {
                "Facility A",
                "Zone 1"
            },
            path);
    }

    [Fact]
    public void ValidateNode_ShouldRejectBlankTargetName()
    {
        // Arrange
        var root =
            CreateNode("Facility A", true);

        // Act
        var result =
            _validator.ValidateNode(
                root,
                "   ",
                out var path);

        // Assert
        Assert.False(result);
        Assert.Empty(path);
    }

    [Fact]
    public void ValidateNode_ShouldPreventCycles()
    {
        // Arrange
        var root =
            CreateNode("Facility A", true);

        var zone =
            CreateNode("Zone 1", true);

        root.Children.Add(zone);

        // Create a cycle:
        // Facility A -> Zone 1 -> Facility A
        zone.Children.Add(root);

        // Act
        var result =
            _validator.ValidateNode(
                root,
                "Missing Node",
                out var path);

        // Assert
        Assert.False(result);
        Assert.Empty(path);
    }

    [Fact]
    public void ValidateNode_ShouldThrowWhenRootIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            _validator.ValidateNode(
                null!,
                "Node",
                out _));
    }

    private static DeploymentNode CreateNode(
        string name,
        bool isConfigured)
    {
        return new DeploymentNode
        {
            Id = Guid.NewGuid(),
            Name = name,
            IsConfigured = isConfigured,
            Children = new List<DeploymentNode>()
        };
    }
}