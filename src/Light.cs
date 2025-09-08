using Godot;

namespace TouchGrass
{
    [Tool]
    [GlobalClass]
    public partial class Light : Node3D
    {
        [Export] public float Brightness { get; private set; }

        public void SetBrightness(float brightness)
        {
            Brightness = brightness;
        }
    }
}
