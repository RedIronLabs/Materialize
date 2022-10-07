// ReSharper disable NonReadonlyMemberInGetHashCode

namespace OBJ_IO.Plugins.Mesh.OBJ
{
    public class ObjFaceVertex
    {
        //------------------------------------------------------------------------------------------------------------
        public int MVertexIndex = -1;
        public int MUvIndex = -1;
        public int MUv2Index = -1;
        public int MNormalIndex = -1;
        public int MColorIndex = -1;

        public override int GetHashCode()
        {
            return MVertexIndex ^ MUvIndex ^ MUv2Index ^ MNormalIndex ^ MColorIndex;
        }

        public override bool Equals(object obj)
        {
            var faceVertex = (ObjFaceVertex)obj;
            return faceVertex != null && MVertexIndex == faceVertex.MVertexIndex &&
                   MUvIndex == faceVertex.MUvIndex && MUv2Index == faceVertex.MUv2Index &&
                   MNormalIndex == faceVertex.MNormalIndex;
        }
    }
}