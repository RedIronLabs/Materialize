using System.Collections.Generic;
using OBJ_IO.Plugins.Mesh.OBJ;
using UnityEngine;

namespace OBJ_IO.Plugins.Extension
{
    public static class MeshExt
    {
        public static void LoadObj(this UnityEngine.Mesh lMesh, ObjData lData)
        {
            var lVertices = new List<Vector3>();
            var lNormals = new List<Vector3>();
            var lUVs = new List<Vector2>();
            var lIndices = new List<int>[lData.MGroups.Count];
            var lVertexIndexRemap = new Dictionary<ObjFaceVertex, int>();
            var lHasNormals = lData.MNormals.Count > 0;
            var lHasUVs = lData.MUvs.Count > 0;

            lMesh.subMeshCount = lData.MGroups.Count;
            for (var lGCount = 0; lGCount < lData.MGroups.Count; ++lGCount)
            {
                var lGroup = lData.MGroups[lGCount];
                lIndices[lGCount] = new List<int>();

                foreach (var lFace in lGroup.Faces)
                {
                    // Unity3d doesn't support non-triangle faces
                    // so we do simple fan triangulation
                    for (var lVCount = 1; lVCount < lFace.Count - 1; ++lVCount)
                    {
                        foreach (var i in new[] { 0, lVCount, lVCount + 1 })
                        {
                            var lFaceVertex = lFace[i];

                            if (!lVertexIndexRemap.TryGetValue(lFaceVertex, out var lVertexIndex))
                            {
                                lVertexIndexRemap[lFaceVertex] = lVertices.Count;
                                lVertexIndex = lVertices.Count;

                                lVertices.Add(lData.MVertices[lFaceVertex.MVertexIndex]);
                                if (lHasUVs)
                                {
                                    lUVs.Add(lData.MUvs[lFaceVertex.MUvIndex]);
                                }

                                if (lHasNormals)
                                {
                                    lNormals.Add(lData.MNormals[lFaceVertex.MNormalIndex]);
                                }
                            }

                            lIndices[lGCount].Add(lVertexIndex);
                        }
                    }
                }
            }

            lMesh.triangles = new int[] { };
            lMesh.vertices = lVertices.ToArray();
            lMesh.uv = lUVs.ToArray();
            lMesh.normals = lNormals.ToArray();
            if (!lHasNormals)
            {
                lMesh.RecalculateNormals();
            }

            lMesh.RecalculateTangents();

            for (var lGCount = 0; lGCount < lData.MGroups.Count; ++lGCount)
            {
                lMesh.SetTriangles(lIndices[lGCount].ToArray(), lGCount);
            }
        }

        //------------------------------------------------------------------------------------------------------------
        public static ObjData EncodeObj(this UnityEngine.Mesh lMesh)
        {
            var lData = new ObjData
            {
                MVertices = new List<Vector3>(lMesh.vertices),
                MUvs = new List<Vector2>(lMesh.uv),
                MNormals = new List<Vector3>(lMesh.normals),
                MUv2S = new List<Vector2>(lMesh.uv2),
                MColors = new List<Color>(lMesh.colors)
            };

            for (var lMCount = 0; lMCount < lMesh.subMeshCount; ++lMCount)
            {
                var lIndices = lMesh.GetTriangles(lMCount);
                var lGroup = new ObjGroup(lMesh.name + "_" + lMCount);

                for (var lCount = 0; lCount < lIndices.Length; lCount += 3)
                {
                    var lFace = new ObjFace();

                    var lFaceVertex = new ObjFaceVertex
                    {
                        MVertexIndex = lData.MVertices.Count > 0 ? lIndices[lCount] : -1,
                        MUvIndex = lData.MUvs.Count > 0 ? lIndices[lCount] : -1,
                        MNormalIndex = lData.MNormals.Count > 0 ? lIndices[lCount] : -1,
                        MUv2Index = lData.MUv2S.Count > 0 ? lIndices[lCount] : -1,
                        MColorIndex = lData.MColors.Count > 0 ? lIndices[lCount] : -1
                    };
                    lFace.AddVertex(lFaceVertex);

                    lFaceVertex = new ObjFaceVertex
                    {
                        MVertexIndex = lData.MVertices.Count > 0 ? lIndices[lCount + 1] : -1,
                        MUvIndex = lData.MUvs.Count > 0 ? lIndices[lCount + 1] : -1,
                        MNormalIndex = lData.MNormals.Count > 0 ? lIndices[lCount + 1] : -1,
                        MUv2Index = lData.MUv2S.Count > 0 ? lIndices[lCount + 1] : -1,
                        MColorIndex = lData.MColors.Count > 0 ? lIndices[lCount + 1] : -1
                    };
                    lFace.AddVertex(lFaceVertex);

                    lFaceVertex = new ObjFaceVertex
                    {
                        MVertexIndex = lData.MVertices.Count > 0 ? lIndices[lCount + 2] : -1,
                        MUvIndex = lData.MUvs.Count > 0 ? lIndices[lCount + 2] : -1,
                        MNormalIndex = lData.MNormals.Count > 0 ? lIndices[lCount + 2] : -1,
                        MUv2Index = lData.MUv2S.Count > 0 ? lIndices[lCount + 2] : -1,
                        MColorIndex = lData.MColors.Count > 0 ? lIndices[lCount + 2] : -1
                    };
                    lFace.AddVertex(lFaceVertex);

                    lGroup.AddFace(lFace);
                }

                lData.MGroups.Add(lGroup);
            }

            return lData;
        }
    }
}