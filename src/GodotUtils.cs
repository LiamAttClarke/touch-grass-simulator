using System.Collections.Generic;
using Godot;

public static class GodotUtils
{

    public static void CollectNodes<T>(Node parent, List<T> outList, bool skipHidden = false) where T : Node3D
    {
        foreach (var node in parent.GetChildren())
        {
            if (node is T)
            {
                var targetNode = (T)node;
                if (skipHidden && !targetNode.IsVisibleInTree()) continue;
                outList.Add(targetNode);
            }
        }
    }

    public static IEnumerable<Node> FindDescendants(Node parent)
    {
        var unvistedChildren = new Queue<Node>();
        foreach (var child in parent.GetChildren())
        {
            unvistedChildren.Enqueue(child);
        }
        while (unvistedChildren.Count > 0)
        {
            var nextChild = unvistedChildren.Dequeue();
            foreach (var child in nextChild.GetChildren())
            {
                unvistedChildren.Enqueue(child);
            }
            yield return nextChild;
        }
    }

    public static void SetNodeOwner(Node node, Node owner)
    {
        node.Owner = owner;
        foreach (var child in FindDescendants(node))
        {
            child.Owner = owner;
        }
    }

    public static void DeleteChildren(Node parent)
    {
        foreach (var node in parent.GetChildren())
        {
            node.Free();
        }
    }
}
