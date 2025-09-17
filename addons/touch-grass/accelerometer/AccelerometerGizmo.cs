using Godot;
using System;

public partial class AccelerometerGizmo : EditorNode3DGizmoPlugin
{
    private const string ICON_PATH = "res://addons/touch-grass/accelerometer/gizmo-icon.svg";


    public AccelerometerGizmo()
    {
        var iconTexture = GD.Load<Texture2D>(ICON_PATH);
        CreateIconMaterial("icon", iconTexture);
    }

    public override string _GetGizmoName()
    {
        return "Accelerometer Gizmo";
    }

    public override bool _HasGizmo(Node3D forNode3D)
    {
        return forNode3D is Accelerometer;
    }

    public override void _Redraw(EditorNode3DGizmo gizmo)
    {
        base._Redraw(gizmo);

        gizmo.Clear();

        var iconMaterial = GetMaterial("icon");

        gizmo.AddUnscaledBillboard(iconMaterial, defaultScale: 0.05f);
    }
}
