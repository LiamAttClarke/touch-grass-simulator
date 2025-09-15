using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TouchGrass
{
    [Tool]
    [GlobalClass]
    public partial class LightChain : Path3D
    {
        [Export(PropertyHint.Range, "0,1")]
        public float Brightness
        {
            get => _brightness;
            private set => SetBrightness(value);
        }
        [Export] public Node LightContainer { get; set; }
        [Export] public Node3D LightNode { get; set; }
        [Export(PropertyHint.Range, "0,32,or_greater")]
        public int Priority { get; set; }
        [Export(PropertyHint.Range, "0,32,or_greater")]
        public int LightCount { get; set; }

        private float _brightness = 1.0f;
        private Light _light => LightNode as Light;
        private List<Light> _lights = new();

        public override void _EnterTree()
        {
            base._EnterTree();

            if (LightContainer == null) throw new Exception("No Light Container selected");
            if (_light == null) throw new Exception("No light selected");

            if (Engine.IsEditorHint())
            {
                // Using 'Node.Connect' automatically releases signal when node is freed
                Connect(Path3D.SignalName.CurveChanged, Callable.From(HandleCurveChanged));
                SetNotifyTransform(true);
            }
        }

        public override void _Ready()
        {
            base._Ready();

            GodotUtils.CollectNodes(LightContainer, _lights);
            UpdateLightPositions();
        }

        public override void _Process(double delta)
        {
            base._Process(delta);

            SetLightCount(LightCount);
        }


        public override void _Notification(int notification)
        {
            switch (notification)
            {
                case (int)NotificationTransformChanged:
                    UpdateLightPositions();
                    break;
            }
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
                lightClone.Owner = sceneRoot;
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

        public void SetLightCount(int lightCount)
        {
            if (_lights.Count == LightCount) return;
            if (!IsNodeReady()) return;
            while (_lights.Count > lightCount) RemoveLight();
            while (_lights.Count < lightCount) AddLight();
        }

        private void UpdateLightPositions()
        {
            var curve = GetCurve();
            var curveLength = curve.GetBakedLength();
            var pointOffset = curveLength / Math.Max(1, _lights.Count - 1);
            for (int i = 0; i < _lights.Count; i++)
            {
                var light = _lights[i];
                light.GlobalPosition = GlobalPosition + curve.SampleBaked(i * pointOffset);
            }
        }

        private void HandleCurveChanged()
        {
            // Ensure all point positions are in the Vector3.Up plane at point Transform.GlobalPosition
            var curve = GetCurve();
            for (int i = 0; i < curve.PointCount; i++)
            {
                var currentPosition = curve.GetPointPosition(i);
                var currentIn = curve.GetPointIn(i);
                var currentOut = curve.GetPointOut(i);

                var targetY = GlobalPosition.Y;

                var newPosition = new Vector3(currentPosition.X, targetY, currentPosition.Z);
                var newIn = new Vector3(currentIn.X, targetY, currentIn.Z);
                var newOut = new Vector3(currentOut.X, targetY, currentOut.Z);
                // Setting point properties will trigger a CurveChanged signal,
                // only do so if the value has actually changed to prevent an endless loop.
                if (newPosition != currentPosition) curve.SetPointPosition(i, newPosition);
                if (newIn != currentIn) curve.SetPointIn(i, newIn);
                if (newOut != currentOut) curve.SetPointOut(i, newOut);
            }

            UpdateLightPositions();
        }
    }
}

