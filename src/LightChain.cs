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
        [Export(PropertyHint.Range, "0,1")] public float Brightness { get; private set; }
        [Export] public Path3D Path { get; set; }
        [Export] public Node LightContainer { get; set; }
        [Export] public Node3D LightNode { get; set; }
        [Export(PropertyHint.Range, "0,100,or_greater")]
        public int LightCount { get; set; }

        private Light Light => LightNode as Light;
        List<Light> Lights = new();


        public override void _EnterTree()
        {
            base._EnterTree();

            if (Path == null) throw new Exception("No path selected");
            if (LightContainer == null) throw new Exception("No Light Container selected");
            if (Light == null) throw new Exception("No light selected");

            Path.CurveChanged += UpdateLightPositions;


            GodotUtils.CollectNodes(LightContainer, Lights, skipHidden: true);
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
            Brightness = brightness;
            foreach (var light in Lights)
            {
                light.SetBrightness(brightness);
            }
        }

        public void AddLight()
        {
            GD.Print("Add Light");
            var lightClone = Light.Duplicate() as Light;
            lightClone.Name = $"Light_{Lights.Count + 1}";
            lightClone.Visible = true;
            Lights.Add(lightClone);
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
            if (Lights.Count == 0) return;
            var light = Lights.Last();
            Lights.Remove(light);
            light.QueueFree();

            UpdateLightPositions();
        }

        private void SetLightCount(int lightCount)
        {
            while (Lights.Count > Math.Max(lightCount, 0)) RemoveLight();
            while (Lights.Count < lightCount) AddLight();
        }

        private void UpdateLightPositions()
        {
            var curve = Path.GetCurve();
            var curveLength = curve.GetBakedLength();
            var pointOffset = curveLength / Lights.Count;
            for (int i = 0; i < Lights.Count; i++)
            {
                var light = Lights[i];
                light.GlobalPosition = curve.SampleBaked((i + 1) * pointOffset);
            }
        }
    }
}

