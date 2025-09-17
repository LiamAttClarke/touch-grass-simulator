using System;
using System.Collections.Generic;
using Godot;

[Tool]
[GlobalClass]
public partial class FlickerFadeLightController : BaseLightController
{
    class LightState
    {
        public Accelerometer Accelerometer;
        public LightChain LightChain;
        public float Energy = 0f;

        public LightState(Accelerometer accelerometer, LightChain lightChain, float energy = 0f)
        {
            Accelerometer = accelerometer;
            LightChain = lightChain;
            Energy = energy;
        }
    }


    [Export] public float DissipationRate = 0.1f;

    private List<LightState> lightStates = new();
    private float _timeElapsed = 0f;


    public override void _Ready()
    {
        base._Ready();

        if (LightChains.Count == 0) throw new Exception("At least one LightChain must be provided.");
        if (Accelerometers.Count == 0) throw new Exception("At least one Accelerometer must be provided.");

        // Find nearest LightChain to each accelerometer
        foreach (var accelerometer in Accelerometers)
        {
            LightChain nearestLightChain = null;
            var shortestDistance = float.PositiveInfinity;
            foreach (var lightChain in LightChains)
            {
                var distance = lightChain.GetShortestDistance(accelerometer.GlobalPosition);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestLightChain = lightChain;
                }
            }
            if (nearestLightChain == null) throw new Exception("No LightChain found.");
            lightStates.Add(new LightState(accelerometer, nearestLightChain));
        }
    }

    protected override void UpdateLights(double delta)
    {
        foreach (var lightState in lightStates)
        {
            var acceleration = Mathf.Min(lightState.Accelerometer.Acceleration.Length(), 1f);

            lightState.Energy = Mathf.Max(acceleration, lightState.Energy);

            lightState.Energy -= DissipationRate * (float)delta;

            lightState.Energy = Mathf.Max(0f, lightState.Energy);

            // var x = (1f - lightState.Energy) * 5f;
            // var brightness = (1 + Mathf.Cos(x * x) * Mathf.Cos(x * 2)) / (1 + x * x);

            lightState.LightChain.SetBrightness(lightState.Energy);

            // GD.Print($"e: {lightState.Energy}, a: {acceleration}, b: {brightness}");
        }
    }
}
