using Godot;
using System;
using System.Collections.Generic;

namespace TouchGrass
{
    [Tool]
    [GlobalClass]
    public abstract partial class BaseLightController : Node
    {
        [Export] public bool Enabled = true;
        protected List<Accelerometer> _accelerometers = new();
        protected List<LightChain> _lightChains = new();

        public override void _Ready()
        {
            base._Ready();

            var sceneTree = GetTree();
            var currentScene = Engine.IsEditorHint() ? sceneTree.EditedSceneRoot : sceneTree.CurrentScene;
            GodotUtils.CollectNodes(currentScene, _lightChains, recursive: true);
            GodotUtils.CollectNodes(currentScene, _accelerometers, recursive: true);
            _lightChains.Sort((a, b) => a.Priority.CompareTo(b.Priority));
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
}
