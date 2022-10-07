
using UnityEngine;

namespace OBJ_IO.Plugins.Mesh.OBJ
{
    public class ObjMaterial
    {
        //------------------------------------------------------------------------------------------------------------
        public ObjMaterial(string lMaterialName)
        {
            MName = lMaterialName;
        }

        //------------------------------------------------------------------------------------------------------------
        public string MName;

        //------------------------------------------------------------------------------------------------------------
        public Color MAmbientColor;
        public Color MDiffuseColor;
        public Color MSpecularColor;
        public float MSpecularCoefficient;

        //------------------------------------------------------------------------------------------------------------
        public float MTransparency;

        //------------------------------------------------------------------------------------------------------------
        public int MIlluminationModel;

        //------------------------------------------------------------------------------------------------------------
        public string MAmbientTextureMap;
        public string MDiffuseTextureMap;

        //------------------------------------------------------------------------------------------------------------
        public string MSpecularTextureMap;
        public string MSpecularHighlightTextureMap;

        //------------------------------------------------------------------------------------------------------------
        public string MBumpMap;
        public string MDisplacementMap;
        public string MStencilDecalMap;

        //------------------------------------------------------------------------------------------------------------
        public string MAlphaTextureMap;
    }
}