using System;
using Godot;

namespace TouchGrass
{
    [Tool]
    [GlobalClass]
    public partial class Light : Node3D
    {
        [Export] public float Brightness { get; private set; }
        [Export] public Light3D LightNode { get; set; }

        public override void _EnterTree()
        {
            base._EnterTree();

            if (LightNode == null) throw new Exception("No light selected");
        }

        public void SetBrightness(float brightness)
        {
            Brightness = brightness;
            LightNode.LightEnergy = brightness;
        }
    }
}
