using System;
using UnityEngine;

namespace OBJ_IO.Plugins.Extension
{
    //------------------------------------------------------------------------------------------------------------
    public static class Utils
    {
        public static Vector2 ParseVector2String(string lData, char lSeparator = ' ')
        {
            var lParts = lData.Split(new[] { lSeparator }, StringSplitOptions.RemoveEmptyEntries);

            var lX = lParts[0].ParseInvariantFloat();
            var lY = lParts[1].ParseInvariantFloat();

            return new Vector2(lX, lY);
        }

        //------------------------------------------------------------------------------------------------------------
        public static Vector3 ParseVector3String(string lData, char lSeparator = ' ')
        {
            var lParts = lData.Split(new[] { lSeparator }, StringSplitOptions.RemoveEmptyEntries);

            var lX = lParts[0].ParseInvariantFloat();
            var lY = lParts[1].ParseInvariantFloat();
            var lZ = lParts[2].ParseInvariantFloat();

            return new Vector3(lX, lY, lZ);
        }

        //------------------------------------------------------------------------------------------------------------
        public static Vector4 ParseVector4String(string lData, char lSeparator = ' ')
        {
            var lParts = lData.Split(new[] { lSeparator }, StringSplitOptions.RemoveEmptyEntries);

            var lX = lParts[0].ParseInvariantFloat();
            var lY = lParts[1].ParseInvariantFloat();
            var lZ = lParts[2].ParseInvariantFloat();
            var lW = lParts[3].ParseInvariantFloat();

            return new Vector4(lX, lY, lZ, lW);
        }

        //------------------------------------------------------------------------------------------------------------
        public static Quaternion ParseQuaternion(string lJsonData)
        {
            var lQuaternionArray = lJsonData.Replace("(", "").Replace(")", "").Replace(" ", "").Split(',');
            var lQuaternion = Quaternion.identity;

            if (float.TryParse(lQuaternionArray[0], out lQuaternion.x) == false)
            {
                return Quaternion.identity;
            }
            if (float.TryParse(lQuaternionArray[1], out lQuaternion.y) == false)
            {
                return Quaternion.identity;
            }
            if (float.TryParse(lQuaternionArray[2], out lQuaternion.z) == false)
            {
                return Quaternion.identity;
            }
            if (float.TryParse(lQuaternionArray[3], out lQuaternion.w) == false)
            {
                return Quaternion.identity;
            }

            return lQuaternion;
        }

        //------------------------------------------------------------------------------------------------------------
        public static string Vector3String(Vector3 lVector3)
        {
            return "(" +
                lVector3.x.ToString("f3") + "," +
                lVector3.y.ToString("f3") + "," +
                lVector3.z.ToString("f3") +
                ")";
        }

        //------------------------------------------------------------------------------------------------------------
        public static string Vector4String(Vector4 lVector4)
        {
            return "(" +
                lVector4.x.ToString("f3") + "," +
                lVector4.y.ToString("f3") + "," +
                lVector4.z.ToString("f3") + "," +
                lVector4.w.ToString("f3") +
                ")";
        }

        //------------------------------------------------------------------------------------------------------------
        public static string QuaternionString(Quaternion lQuaternion)
        {
            return "(" +
                lQuaternion.x.ToString("f3") + "," +
                lQuaternion.y.ToString("f3") + "," +
                lQuaternion.z.ToString("f3") + "," +
                lQuaternion.w.ToString("f3") +
                ")";
        }

        //------------------------------------------------------------------------------------------------------------
        public static int FirstInt(string lJsonData)
        {
            var lDigits = "";
            for (var lCount = 0; lCount < lJsonData.Length && Char.IsDigit(lJsonData[lCount]); ++lCount)
            {
                lDigits += lJsonData[lCount];
            }
            return int.Parse(lDigits);
        }
    }
}
