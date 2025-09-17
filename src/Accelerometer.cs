using Godot;
using System;


[Tool]
[GlobalClass]
public partial class Accelerometer : RigidBody3D
{
    [Export] public Vector3 Acceleration { get; private set; }
    [Export] public float SpringConstant = 1.0f;

    private Vector3 _previousVelocity;
    private Vector3 _origin;

    public override void _Ready()
    {
        base._Ready();

        _previousVelocity = LinearVelocity;
        _origin = GlobalPosition;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        Acceleration = (LinearVelocity - _previousVelocity) / (float)delta;

        _previousVelocity = LinearVelocity;

        // Apply spring force to return the cattail to it's origin after a disturbance
        // TODO: Move this to another component
        var toOrigin = _origin - GlobalPosition;
        var elasticPotential = 0.5f * toOrigin.LengthSquared() * SpringConstant;
        ConstantForce = toOrigin.Normalized() * elasticPotential;
    }
}
