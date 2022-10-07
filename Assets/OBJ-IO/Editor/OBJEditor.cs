using System.IO;
using OBJ_IO.Plugins.Extension;
using OBJ_IO.Plugins.Mesh.OBJ;
using UnityEditor;
using UnityEngine;

namespace OBJ_IO.Editor
{
	public class ObjWindow : EditorWindow
	{
		//------------------------------------------------------------------------------------------------------------
		private MeshFilter _mMeshFilter; 
	
		//------------------------------------------------------------------------------------------------------------
		[MenuItem("OBJ-IO/OBJ Mesh Exporter")]
		public static void Execute()
		{
			GetWindow<ObjWindow>();
		}
	
		//------------------------------------------------------------------------------------------------------------
		private void OnGUI()
		{
			_mMeshFilter = (MeshFilter)EditorGUILayout.ObjectField("MeshFilter", _mMeshFilter, typeof(MeshFilter), true);
		
			if (_mMeshFilter != null)
			{
				if (GUILayout.Button("Export OBJ"))
				{
					var lOutputPath = EditorUtility.SaveFilePanel("Save Mesh as OBJ", "", _mMeshFilter.name + ".obj", "obj");
				
					if (File.Exists(lOutputPath))
					{
						File.Delete(lOutputPath);
					}
				
					var lStream = new FileStream(lOutputPath, FileMode.Create);
					var lObjData = _mMeshFilter.sharedMesh.EncodeObj();
					ObjLoader.ExportObj(lObjData, lStream);
					lStream.Close();
				}
			}
			else
			{
				GUILayout.Label("Please provide a MeshFilter");
			}
		}
	}
}
