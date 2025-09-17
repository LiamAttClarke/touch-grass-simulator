using Godot;
using static Godot.Camera3D;


public partial class PlayerController : RayCast3D
{
    [Export] public BaseLightController LightController;
    [Export] public float ImpulseRadius = 1.0f;
    [Export] public float ImpulseMagnitude = 1.0f;
    [Export] public Camera3D Camera;
    [Export] public Node3D OrbitTarget;
    [Export] public Node3D Cursor;
    [Export] public float RayLength = 1000;
    // Camera movement
    [Export(PropertyHint.Range, "0, 1.0")] private float CameraDrag = 0.1f;
    [Export] private float HorizontalCameraSpeed = 1f;
    [Export] private float VerticalCameraSpeed = 1f;
    [Export] private float CameraZoomIncrement = 0.1f;
    [Export(PropertyHint.Range, "0, 1.0")] private float CameraZoomSpeed = 0.1f;
    [Export] private float MinZoom = 0.1f;
    [Export] private float MaxZoom = 1.0f;
    [Export] private float MinHeight = 0.0f;
    [Export] private float MaxHeight = 10.0f;
    private float _cameraHeight = 0f;
    private float _cameraRadius = 128f;
    private float _cameraAzimuth = 0f;
    private float _cameraAltitude = Mathf.DegToRad(30f);
    private float _cameraZoom = 0.5f;
    private float _targetZoom = 0.5f;
    private bool _isDraggingCamera = false;
    private bool _isMovingCamera = false;
    private Vector2 _lastPointerPosition;
    private Vector2 _cameraVelocity;

    // Raycasting
    private Vector3 _targetPosition = Vector3.Zero;
    private Vector3 _targetNormal = Vector3.Zero;

    private float cameraRotationalSpeed
    {
        get
        {
            return HorizontalCameraSpeed / _cameraRadius;
        }
    }

    public override void _Ready()
    {
        base._Ready();

        Cursor.Scale = new Vector3(ImpulseRadius, ImpulseRadius, ImpulseRadius);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        UpdateCamera();
        CastRay();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent)
        {
            switch (keyEvent.Keycode)
            {
                case Key.A:
                    _cameraVelocity.X = -cameraRotationalSpeed;
                    _isMovingCamera = keyEvent.Pressed;
                    break;
                case Key.D:
                    _cameraVelocity.X = cameraRotationalSpeed;
                    _isMovingCamera = keyEvent.Pressed;
                    break;
            }
        }
        if (@event is InputEventMouseButton mouseButtonEvent)
        {
            switch (mouseButtonEvent.ButtonIndex)
            {
                case MouseButton.Left:
                    if (mouseButtonEvent.Pressed)
                    {
                        ApplyImpulse(_targetPosition);
                    }
                    break;
                case MouseButton.Right:
                    if (mouseButtonEvent.Pressed)
                    {
                        _isDraggingCamera = true;
                        _lastPointerPosition = mouseButtonEvent.Position;
                    }
                    else
                    {
                        _isDraggingCamera = false;
                    }
                    break;
                case MouseButton.WheelUp:
                    _targetZoom = Mathf.Max(0.0f, _targetZoom + mouseButtonEvent.Factor * CameraZoomIncrement);
                    break;
                case MouseButton.WheelDown:
                    _targetZoom = Mathf.Min(1.0f, _targetZoom - mouseButtonEvent.Factor * CameraZoomIncrement);
                    break;

            }
        }
        else if (@event is InputEventMouseMotion mouseMotionEvent)
        {
            if (_isDraggingCamera)
            {
                var viewportSize = GetViewport().GetVisibleRect().Size;
                var normalizedPointerVelocity = (mouseMotionEvent.Position - _lastPointerPosition) / viewportSize;
                _cameraVelocity = new Vector2(
                    normalizedPointerVelocity.X * cameraRotationalSpeed,
                    normalizedPointerVelocity.Y * VerticalCameraSpeed
                );
                _lastPointerPosition = mouseMotionEvent.Position;
            }
        }
        _isMovingCamera = _isMovingCamera || _isDraggingCamera;
    }

    private void UpdateCamera()
    {
        // Update camera position/orientation
        _cameraAzimuth -= _cameraVelocity.X;
        _cameraHeight = Mathf.Clamp(_cameraHeight + _cameraVelocity.Y, MinHeight, MaxHeight);
        var offset = new Vector3(
            _cameraRadius * Mathf.Cos(_cameraAzimuth),
            _cameraHeight + _cameraRadius * Mathf.Sin(_cameraAltitude),
            -_cameraRadius * Mathf.Sin(_cameraAzimuth)
        );
        Camera.GlobalPosition = OrbitTarget.GlobalPosition + offset;
        Camera.LookAt(OrbitTarget.GlobalPosition + new Vector3(0, _cameraHeight, 0));
        _cameraZoom = Mathf.Lerp(_cameraZoom, _targetZoom, CameraZoomSpeed);
        if (Camera.Projection == ProjectionType.Perspective)
        {
            Camera.Fov = Mathf.Lerp(MaxZoom, MinZoom, _cameraZoom);
        }
        else if (Camera.Projection == ProjectionType.Orthogonal)
        {
            Camera.Size = Mathf.Lerp(MinZoom, MaxZoom, _cameraZoom);
        }

        // Apply drag to camera
        _cameraVelocity *= 1f - CameraDrag;
    }

    private void CastRay()
    {
        var spaceState = GetWorld3D().DirectSpaceState;
        var mousePosition = GetViewport().GetMousePosition();

        var origin = Camera.ProjectRayOrigin(mousePosition);
        var end = origin + Camera.ProjectRayNormal(mousePosition) * RayLength;
        var raycastQuery = PhysicsRayQueryParameters3D.Create(origin, end);

        var raycastResult = spaceState.IntersectRay(raycastQuery);


        if (raycastResult.Count > 0)
        {
            _targetPosition = (Vector3)raycastResult["position"];

            // Set cursor position
            SetCursor(_targetPosition);
        }
        else
        {
            DisableCursor();
        }
    }

    private void SetCursor(Vector3 worldPos)
    {
        Cursor.GlobalPosition = worldPos;
        Cursor.Visible = true;
    }

    private void DisableCursor()
    {
        Cursor.Visible = false;
    }

    private void ApplyImpulse(Vector3 position)
    {
        foreach (var accelerometer in LightController.Accelerometers)
        {
            if (accelerometer.GlobalPosition.DistanceTo(position) < ImpulseRadius)
            {
                var relativePos = position - accelerometer.GlobalPosition;
                var force = relativePos.Normalized() * -ImpulseMagnitude;
                accelerometer.ApplyImpulse(force);
            }
        }
    }
}
