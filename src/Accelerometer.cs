using Godot;
using System;

namespace TouchGrass
{
    public partial class Accelerometer : RigidBody3D
    {
        [Export] public string Identifier;
        [Export]
        public Vector3 Acceleration { get; private set; }

        private Vector3 _previousVelocity;

        public override void _Ready()
        {
            base._Ready();

            _previousVelocity = LinearVelocity;
        }

        public override void _PhysicsProcess(double delta)
        {
            base._PhysicsProcess(delta);

            Acceleration = (LinearVelocity - _previousVelocity) / (float)delta;



            _previousVelocity = LinearVelocity;
        }
    }
}
