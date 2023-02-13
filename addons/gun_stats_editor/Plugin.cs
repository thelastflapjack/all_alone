using Godot;


namespace GunStatsEditor
{
    [Tool]
    public class Plugin : EditorPlugin
    {
        
        private EditorDock _dock;

        public override void _EnterTree()
        {
            _dock = ResourceLoader.Load<PackedScene>(
                "res://addons/gun_stats_editor/dock/EditorDock.tscn"
            ).Instance<EditorDock>();

            AddControlToBottomPanel(_dock, "Gun Stats");
        }

        public override void _ExitTree()
        {
            RemoveControlFromBottomPanel(_dock);
            _dock.QueueFree();
        }
    }
}

