using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OBJ_IO.Plugins.Extension;
using UnityEngine;

// ReSharper disable StringLiteralTypo

/*
 * Currently only supports Triangular Meshes
 */

namespace OBJ_IO.Plugins.Mesh.OBJ
{
    public static class ObjLoader
    {
        //------------------------------------------------------------------------------------------------------------
        private static ObjData _mObjData;

        //------------------------------------------------------------------------------------------------------------
        private static ObjGroup _mCurrentGroup;

        #region PROCESSORS

        //------------------------------------------------------------------------------------------------------------
        private static readonly Dictionary<string, Action<string>> MParseObjActionDictionary = new()
        {
            {
                "mtllib", _ =>
                {
                    /*Load MTL*/
                }
            },
            {
                "usemtl", lEntry =>
                {
                    PushObjGroupIfNeeded();
                    _mCurrentGroup.MMaterial = _mObjData.MMaterials.SingleOrDefault(lX =>
                        lX.MName.EqualsInvariantCultureIgnoreCase(lEntry));
                }
            },
            {
                "v",
                lEntry => { _mObjData.MVertices.Add(Extension.Utils.ParseVector3String(lEntry)); }
            },
            {
                "vn",
                lEntry => { _mObjData.MNormals.Add(Extension.Utils.ParseVector3String(lEntry)); }
            },
            {
                "vt",
                lEntry => { _mObjData.MUvs.Add(Extension.Utils.ParseVector2String(lEntry)); }
            },
            {
                "vt2",
                lEntry => { _mObjData.MUv2S.Add(Extension.Utils.ParseVector2String(lEntry)); }
            },
            {
                "vc",
                lEntry =>
                {
                    _mObjData.MColors.Add(Extension.Utils.ParseVector4String(lEntry).ToColor());
                }
            },
            { "f", PushObjFace },
            { "g", PushObjGroup },
        };

        //------------------------------------------------------------------------------------------------------------

        #endregion

        #region PUBLIC_INTERFACE

        //------------------------------------------------------------------------------------------------------------
        public static ObjData LoadObj(Stream lStream)
        {
            _mObjData = new ObjData();
            _mCurrentGroup = null;

            var lLineStreamReader = new StreamReader(lStream);

            while (!lLineStreamReader.EndOfStream)
            {
                var lCurrentLine = lLineStreamReader.ReadLine();

                if (lCurrentLine != null && (StringExt.IsNullOrWhiteSpace(lCurrentLine) ||
                                             lCurrentLine[0] == '#'))
                {
                    continue;
                }

                if (lCurrentLine == null) continue;
                var lFields = lCurrentLine.Trim().Split(null, 2);
                if (lFields.Length < 2)
                {
                    continue;
                }

                var lKeyword = lFields[0].Trim();
                var lData = lFields[1].Trim();

                MParseObjActionDictionary.TryGetValue(lKeyword.ToLowerInvariant(), out var lAction);

                lAction?.Invoke(lData);
            }

            var lObjData = _mObjData;
            _mObjData = null;

            return lObjData;
        }

        //------------------------------------------------------------------------------------------------------------
        public static void ExportObj(ObjData lData, Stream lStream)
        {
            var lLineStreamWriter = new StreamWriter(lStream);

            lLineStreamWriter.WriteLine(
                $"# File exported by Unity3D version {Application.unityVersion}");

            for (var lCount = 0; lCount < lData.MVertices.Count; ++lCount)
            {
                lLineStreamWriter.WriteLine(
                    $"v {lData.MVertices[lCount].x:n8} {lData.MVertices[lCount].y:n8} {lData.MVertices[lCount].z:n8}");
            }

            for (var lCount = 0; lCount < lData.MUvs.Count; ++lCount)
            {
                lLineStreamWriter.WriteLine(
                    $"vt {lData.MUvs[lCount].x:n5} {lData.MUvs[lCount].y:n5}");
            }

            for (var lCount = 0; lCount < lData.MUv2S.Count; ++lCount)
            {
                lLineStreamWriter.WriteLine(
                    $"vt2 {lData.MUvs[lCount].x:n5} {lData.MUvs[lCount].y:n5}");
            }

            for (var lCount = 0; lCount < lData.MNormals.Count; ++lCount)
            {
                lLineStreamWriter.WriteLine(
                    $"vn {lData.MNormals[lCount].x:n8} {lData.MNormals[lCount].y:n8} {lData.MNormals[lCount].z:n8}");
            }

            for (var lCount = 0; lCount < lData.MColors.Count; ++lCount)
            {
                lLineStreamWriter.WriteLine(
                    $"vc {lData.MColors[lCount].r:n8} {lData.MColors[lCount].g:n8} {lData.MColors[lCount].b:n8} {lData.MColors[lCount].a:n8}");
            }

            foreach (var t in lData.MGroups)
            {
                lLineStreamWriter.WriteLine($"g {t.MName}");

                foreach (var t1 in t.Faces)
                {
                    lLineStreamWriter.WriteLine(
                        $"f {t1.ToString(0)} {t1.ToString(1)} {t1.ToString(2)}");
                }
            }

            lLineStreamWriter.Flush();
        }

        #endregion

        //------------------------------------------------------------------------------------------------------------
        private static void PushObjGroup(string lGroupName)
        {
            _mCurrentGroup = new ObjGroup(lGroupName);
            _mObjData.MGroups.Add(_mCurrentGroup);
        }

        //------------------------------------------------------------------------------------------------------------
        private static void PushObjGroupIfNeeded()
        {
            if (_mCurrentGroup == null)
            {
                PushObjGroup("default");
            }
        }

        //------------------------------------------------------------------------------------------------------------
        private static void PushObjFace(string lFaceLine)
        {
            PushObjGroupIfNeeded();

            var vertices = lFaceLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var face = new ObjFace();

            foreach (var vertexString in vertices)
            {
                face.ParseVertex(vertexString);
            }

            _mCurrentGroup.AddFace(face);
        }
    }
}