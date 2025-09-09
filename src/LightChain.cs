using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TouchGrass
{


    [Tool]
    [GlobalClass]
    public partial class LightChain : Node3D, ILight
    {
        [Export(PropertyHint.Range, "0,1")]
        public float Brightness
        {
            get => _brightness;
            private set => SetBrightness(value);
        }
        [Export] public Path3D Path { get; set; }
        [Export] public Node LightContainer { get; set; }
        [Export] public Node3D LightNode { get; set; }
        [Export(PropertyHint.Range, "0,100,or_greater")]
        public int LightCount { get; set; }

        private float _brightness = 1.0f;
        private Light _light => LightNode as Light;
        private List<Light> _lights = new();


        public override void _EnterTree()
        {
            base._EnterTree();

            if (Path == null) throw new Exception("No path selected");
            if (LightContainer == null) throw new Exception("No Light Container selected");
            if (_light == null) throw new Exception("No light selected");

            // Using 'Connect' automatically releases signal when node is freed
            Path.Connect(Path3D.SignalName.CurveChanged, Callable.From(UpdateLightPositions));


            GodotUtils.CollectNodes(LightContainer, _lights, skipHidden: true);
        }

        public override void _Ready()
        {
            base._Ready();

            UpdateLightPositions();
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
            SetLightCount(LightCount);
        }


        public void SetBrightness(float brightness)
        {
            _brightness = brightness;
            foreach (var light in _lights)
            {
                light.SetBrightness(brightness);
            }
        }

        public void AddLight()
        {
            var lightClone = _light.Duplicate() as Light;
            lightClone.Name = $"Light_{_lights.Count + 1}";
            lightClone.Visible = true;
            lightClone.SetBrightness(Brightness);

            _lights.Add(lightClone);
            LightContainer.AddChild(lightClone);

            // Ensure node appears in editor scene heirarchy
            if (Engine.IsEditorHint())
            {
                var sceneRoot = GetTree().EditedSceneRoot;
                GodotUtils.SetNodeOwner(lightClone, sceneRoot);
            }

            UpdateLightPositions();
        }

        public void RemoveLight()
        {
            if (_lights.Count == 0) return;
            var light = _lights.Last();
            _lights.Remove(light);
            light.QueueFree();

            UpdateLightPositions();
        }

        private void SetLightCount(int lightCount)
        {
            while (_lights.Count > Math.Max(lightCount, 0)) RemoveLight();
            while (_lights.Count < lightCount) AddLight();
        }

        private void UpdateLightPositions()
        {
            var curve = Path.GetCurve();
            var curveLength = curve.GetBakedLength();
            var pointOffset = curveLength / _lights.Count;
            for (int i = 0; i < _lights.Count; i++)
            {
                var light = _lights[i];
                light.GlobalPosition = curve.SampleBaked((i + 1) * pointOffset);
            }
        }
    }
}

