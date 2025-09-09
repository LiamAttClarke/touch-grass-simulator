using System;
using Godot;

namespace TouchGrass
{
    [Tool]
    [GlobalClass]
    public partial class Light : Node3D
    {
        [Export(PropertyHint.Range, "0.0, 16.0, or_greater")]
        public float MaxEnergy = 16f;
        [Export(PropertyHint.Range, "0.0,1.0")] public float Brightness { get; private set; }
        [Export] public Light3D LightNode { get; set; }

        public override void _EnterTree()
        {
            base._EnterTree();

            if (LightNode == null) throw new Exception("No light selected");
        }

        public void SetBrightness(float brightness)
        {
            Brightness = Mathf.Clamp(brightness, 0.0f, 1.0f);
            LightNode.LightEnergy = MaxEnergy * brightness;
        }
    }
}
