using Godot;
using System;
using System.Collections.Generic;

[Tool]
[GlobalClass]
public abstract partial class BaseLightController : Node
{
    [Export] public bool Enabled = true;
    public List<Accelerometer> Accelerometers = new();
    public List<LightChain> LightChains = new();

    public override void _Ready()
    {
        base._Ready();

        var sceneTree = GetTree();
        GodotUtils.CollectNodes(this, LightChains, recursive: true, skipHidden: true);
        GodotUtils.CollectNodes(this, Accelerometers, recursive: true, skipHidden: true);
        LightChains.Sort((a, b) => a.Priority.CompareTo(b.Priority));
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Enabled)
        {
            UpdateLights(delta);
        }
    }

    protected abstract void UpdateLights(double delta);

}
