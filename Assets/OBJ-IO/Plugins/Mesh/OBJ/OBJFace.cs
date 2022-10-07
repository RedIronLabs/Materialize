using System;
using System.Collections.Generic;
using OBJ_IO.Plugins.Extension;

namespace OBJ_IO.Plugins.Mesh.OBJ
{
    public class ObjFace
    {
        //------------------------------------------------------------------------------------------------------------
        private readonly List<ObjFaceVertex> _mVertices = new();

        //------------------------------------------------------------------------------------------------------------
        public void AddVertex(ObjFaceVertex lVertex)
        {
            _mVertices.Add(lVertex);
        }

        //------------------------------------------------------------------------------------------------------------
        public void ParseVertex(string lVertexString)
        {
            var fields = lVertexString.Split(new[] { '/' }, StringSplitOptions.None);

            var lIndex = fields[0].ParseInvariantInt();
            var faceVertex = new ObjFaceVertex
            {
                MVertexIndex = lIndex - 1
            };

            if (fields.Length > 1)
            {
                lIndex = fields[1].Length == 0 ? 0 : fields[1].ParseInvariantInt();
                faceVertex.MUvIndex = lIndex - 1;
            }

            if (fields.Length > 2)
            {
                lIndex = fields[2].Length == 0 ? 0 : fields[2].ParseInvariantInt();
                faceVertex.MNormalIndex = lIndex - 1;
            }

            if (fields.Length > 3)
            {
                lIndex = fields[3].Length == 0 ? 0 : fields[3].ParseInvariantInt();
                faceVertex.MUv2Index = lIndex - 1;
            }

            if (fields.Length > 4)
            {
                lIndex = fields[4].Length == 0 ? 0 : fields[4].ParseInvariantInt();
                faceVertex.MColorIndex = lIndex - 1;
            }

            AddVertex(faceVertex);
        }

        //------------------------------------------------------------------------------------------------------------
        public string ToString(int lIndex)
        {
            var lFaceVertex = _mVertices[lIndex];

            var lOutput = (lFaceVertex.MVertexIndex + 1).ToString();

            if (lFaceVertex.MUvIndex > -1)
            {
                lOutput += $"/{(lFaceVertex.MUvIndex + 1).ToString()}";
            }

            if (lFaceVertex.MNormalIndex > -1)
            {
                lOutput += $"/{(lFaceVertex.MNormalIndex + 1).ToString()}";
            }

            if (lFaceVertex.MUv2Index > -1)
            {
                lOutput += $"/{(lFaceVertex.MUv2Index + 1).ToString()}";
            }

            if (lFaceVertex.MColorIndex > -1)
            {
                lOutput += $"/{(lFaceVertex.MColorIndex + 1).ToString()}";
            }

            return lOutput;
        }

        //------------------------------------------------------------------------------------------------------------
        public ObjFaceVertex this[int i]
        {
            get { return _mVertices[i]; }
        }

        //------------------------------------------------------------------------------------------------------------
        public int Count
        {
            get { return _mVertices.Count; }
        }
    }
}
