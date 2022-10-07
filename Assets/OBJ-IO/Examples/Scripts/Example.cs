using System.IO;
using OBJ_IO.Plugins.Extension;
using OBJ_IO.Plugins.Mesh.OBJ;
using UnityEngine;

namespace OBJ_IO.Examples.Scripts
{
	[RequireComponent(typeof(MeshFilter))]
	public class Example : MonoBehaviour
	{
		//------------------------------------------------------------------------------------------------------------	
		private const string InputPath = @"Assets/OBJ-IO/Examples/Meshes/Teapot.obj";
		private const string OutputPath = @"Assets/OBJ-IO/Examples/Meshes/Teapot_Modified.obj";

		//------------------------------------------------------------------------------------------------------------	
		private void Start()
		{
			//	Load the OBJ in
			var lStream = new FileStream(InputPath, FileMode.Open);
			var lObjData = ObjLoader.LoadObj(lStream);
			var lMeshFilter = GetComponent<MeshFilter>();
			lMeshFilter.mesh.LoadObj(lObjData);
			lStream.Close();

			//	Wiggle Vertices in Mesh
			var lVertices = lMeshFilter.mesh.vertices;
			for (var lCount = 0; lCount < lVertices.Length; ++lCount)
			{
				lVertices[lCount] += Vector3.up * Mathf.Sin(lVertices[lCount].x) * 4f;
			}
			lMeshFilter.mesh.vertices = lVertices;

			//	Export the new Wiggled Mesh
			if (File.Exists(OutputPath))
			{
				File.Delete(OutputPath);
			}
			lStream = new FileStream(OutputPath, FileMode.Create);
			lObjData = lMeshFilter.mesh.EncodeObj();
			ObjLoader.ExportObj(lObjData, lStream);
			lStream.Close();
		}
	}
}