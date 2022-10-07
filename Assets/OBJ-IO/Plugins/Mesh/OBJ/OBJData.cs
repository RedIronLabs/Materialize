using System.Collections.Generic;
using UnityEngine;

namespace OBJ_IO.Plugins.Mesh.OBJ
{
    public class ObjData
    {
        //------------------------------------------------------------------------------------------------------------
        public List<Vector3> MVertices = new();
        public List<Vector3> MNormals = new();
        public List<Vector2> MUvs = new();
        public List<Vector2> MUv2S = new();
        public List<Color> MColors = new();

        //------------------------------------------------------------------------------------------------------------
        public readonly List<ObjMaterial> MMaterials = new();
        public readonly List<ObjGroup> MGroups = new();
    }
}