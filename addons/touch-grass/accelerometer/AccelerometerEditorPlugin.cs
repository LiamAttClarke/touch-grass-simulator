using Godot;

[Tool]
partial class AccelerometerEditorPlugin : EditorPlugin
{
    private AccelerometerGizmo gizmo = new AccelerometerGizmo();

    public override void _EnterTree()
    {
        base._EnterTree();

        AddNode3DGizmoPlugin(gizmo);
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        RemoveNode3DGizmoPlugin(gizmo);
    }
}
