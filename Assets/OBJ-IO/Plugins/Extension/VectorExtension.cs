using UnityEngine;

namespace OBJ_IO.Plugins.Extension
{
    //------------------------------------------------------------------------------------------------------------
    public static class Vector3Ext
    {
        //------------------------------------------------------------------------------------------------------------
        public static readonly Vector3 IgnoreX = new(0f, 1f, 1f);
        public static readonly Vector3 IgnoreY = new(1f, 0f, 1f);
        public static readonly Vector3 IgnoreZ = new(1f, 1f, 0f);
        
        //------------------------------------------------------------------------------------------------------------
        public static Color ToColor(this Vector3 lVector)
        {
            return new Color(lVector.x, lVector.y, lVector.z);
        }
    }

    //------------------------------------------------------------------------------------------------------------
    public static class Vector4Ext
    {
        //------------------------------------------------------------------------------------------------------------
        public static Color ToColor(this Vector4 lVector)
        {
            return new Color(lVector.x, lVector.y, lVector.z, lVector.w);
        }
    }
}
