using Godot;
using System;


[Tool]
[GlobalClass]
public partial class Grid3D : Node3D
{
    [Export] int Columns = 1;
    [Export] int Rows = 1;
    [Export] float ColumnOffset = 1f;
    [Export] float RowOffset = 1f;
    [Export] Node3D SourceNode;
    Node3D[,] _grid;

    public override void _EnterTree()
    {
        base._EnterTree();

        _grid = new Node3D[Columns, Rows];
        UpdateGrid();
    }

    public void ClearGrid()
    {
        for (int x = 0; x < Columns; x++)
        {
            for (int z = 0; z < Rows; z++)
            {
                var element = _grid[x, z];
                if (element != null)
                {
                    element.Free();
                    _grid[x, z] = null;
                }
            }
        }
        // Remove orphans
        foreach (var child in GetChildren())
        {
            child.Free();
        }
    }

    public void UpdateGrid()
    {
        ClearGrid();
        if (SourceNode == null) return;

        for (int x = 0; x < Columns; x++)
        {
            for (int z = 0; z < Rows; z++)
            {
                SetElement(SourceNode, x, z);
            }
        }
    }

    private void SetElement(Node3D source, int x, int z)
    {
        var clone = SourceNode.Duplicate() as Node3D;
        clone.Name = source.Name + $"_{x}_{x}";
        clone.Position = new Vector3(x * ColumnOffset, 0, z * RowOffset);
        clone.Visible = true;
        AddChild(clone);
        _grid[x, z] = clone;

        if (Engine.IsEditorHint())
        {
            // Ensure node appears in editor scene heirarchy
            var sceneRoot = GetTree().EditedSceneRoot;
            if (Engine.IsEditorHint()) clone.Owner = sceneRoot;
        }
    }
}

