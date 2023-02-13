using Godot;
using System.Collections.Generic;


namespace GameGeneral.LOD
{
    public class Target : MeshInstance
    {
        [Export]
        private Mesh _lodMesh2 = null;
        [Export]
        private Mesh _lodMesh3 = null;

        private List<Mesh> _lodMeshes = new List<Mesh>();

        public override void _Ready()
        {
            _lodMeshes.Add(Mesh);
            _lodMeshes.Add(_lodMesh2);
            _lodMeshes.Add(_lodMesh3);
        }
        
        public void SetLOD(int level)
        {
            Mesh = _lodMeshes[level - 1];
        }
    }
}

