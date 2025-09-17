using System.Collections.Generic;
using Godot;

public static class GodotUtils
{
    public static void CollectNodes<T>(Node parent, List<T> outList, bool recursive = false, bool skipHidden = false) where T : Node
    {
        var unvistedChildren = new Queue<Node>();
        foreach (var child in parent.GetChildren())
        {
            unvistedChildren.Enqueue(child);
        }
        while (unvistedChildren.Count > 0)
        {
            var nextChild = unvistedChildren.Dequeue();
            if (skipHidden && nextChild is Node3D && !(nextChild as Node3D).IsVisibleInTree())
            {
                continue;
            }
            else if (nextChild is T)
            {
                outList.Add(nextChild as T);
            }
            else if (recursive)
            {
                foreach (var child in nextChild.GetChildren())
                {
                    unvistedChildren.Enqueue(child);
                }
            }
        }
    }
}
