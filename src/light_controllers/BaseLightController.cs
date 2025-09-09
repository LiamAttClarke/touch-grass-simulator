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
        private List<Accelerometer> _accelerometers = new();
        private List<LightChain> _lightChains = new();

        public override void _Ready()
        {
            base._Ready();

            var sceneTree = GetTree();
            var currentScene = Engine.IsEditorHint() ? sceneTree.EditedSceneRoot : sceneTree.CurrentScene;
            GodotUtils.CollectNodes(currentScene, _accelerometers, skipHidden: true);
            GodotUtils.CollectNodes(currentScene, _lightChains, skipHidden: true);
            _lightChains.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }

        public override void _Process(double delta)
        {
            base._Process(delta);

            if (Enabled)
            {
                UpdateLights(_lightChains, _accelerometers, delta);
            }
        }

        protected abstract void UpdateLights(List<LightChain> lightChains, List<Accelerometer> accelerometers, double delta);

    }
}
