using Godot;
using System;
using System.Collections;
using System.Collections.Generic;


namespace GameGeneral
{
    [Tool]
    public class NavMeshInstTool : NavigationMeshInstance
    {
        [Export]
        private bool UpdateSwitch
        {
            set{
                UpdateBake();
            }
            get{ return true; }
        }

        public override void _Ready()
        {
            if (!Engine.EditorHint)
            {
                GetNode<MeshInstance>("NavArea").QueueFree();
            }
        }

        private void UpdateBake()
        {
            if (Engine.EditorHint)
            {
                MeshInstance boundingBox = GetNode<MeshInstance>("NavArea");
                AABB filterAabb = new AABB();
                filterAabb = boundingBox.GetAabb();
                // Vector3 overflow = new Vector3(Navmesh.AgentRadius, 0, Navmesh.AgentRadius) * 2;
                // filterAabb.Position -= overflow;
                // filterAabb.End += overflow;
                Navmesh.FilterBakingAabb = filterAabb;
                GD.Print("Baking");
                BakeNavigationMesh();
            }
        }
    }
}

