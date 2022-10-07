
using System.Collections.Generic;

namespace OBJ_IO.Plugins.Mesh.OBJ
{
    public class ObjGroup 
    {
        //------------------------------------------------------------------------------------------------------------
        private readonly List<ObjFace> _mFaces = new();
        
        //------------------------------------------------------------------------------------------------------------
        public ObjGroup(string lName)
        {
            MName = lName;
        }

        //------------------------------------------------------------------------------------------------------------
        public string MName { get; private set; }
        public ObjMaterial MMaterial { get; set; }

        //------------------------------------------------------------------------------------------------------------
        public IList<ObjFace> Faces { get { return _mFaces; } }

        //------------------------------------------------------------------------------------------------------------
        public void AddFace(ObjFace lFace)
        {
            _mFaces.Add(lFace);
        }
    }
}