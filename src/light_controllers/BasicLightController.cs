using System;
using Godot;

namespace TouchGrass
{
    [Tool]
    [GlobalClass]
    public partial class BasicLightController : BaseLightController
    {
        [Export] public float ForceAttenuation = 1.0f;
        private float _timeElapsedS = 0.0f;

        public override void _Ready()
        {
            base._Ready();

            // Add one-time initializations here
        }

        protected override void UpdateLights(double delta)
        {
            // Add animation logic here.
            // This is called once per frame.

            _timeElapsedS += (float)delta;

            foreach (var lightChain in _lightChains)
            {
                var totalEnergy = 0f;
                var chainCurve = lightChain.GetCurve();

                foreach (var accelerometer in _accelerometers)
                {
                    var distanceToChain = chainCurve.GetClosestOffset(accelerometer.Position);
                    totalEnergy += Mathf.Pow(distanceToChain, ForceAttenuation) * accelerometer.Acceleration.Length();
                }


                lightChain.SetBrightness(totalEnergy);
            }
        }
    }
}
