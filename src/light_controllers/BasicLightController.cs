using System.Collections.Generic;
using Godot;

namespace TouchGrass
{
    [Tool]
    [GlobalClass]
    public partial class BasicLightController : BaseLightController
    {
        [Export] public float Amplitude = 1.0f;
        private float _timeElapsedS = 0.0f;

        public override void _Ready()
        {
            base._Ready();
        }

        protected override void UpdateLights(List<LightChain> lightChains, List<Accelerometer> accelerometers, double delta)
        {
            _timeElapsedS += (float)delta;

            var brightness = (Mathf.Sin(_timeElapsedS) + 1) * 0.5f * Amplitude;

            foreach (var lightChain in lightChains)
            {
                lightChain.SetBrightness(brightness);
            }
        }
    }
}
