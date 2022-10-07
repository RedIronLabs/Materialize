using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

public class BatchUI : MonoBehaviour
{
    public MainGui MainGui;
   // public HeightFromDiffuseGui HeightmapCreator;
    public bool UseInitializeLocation;
    private bool _pathIsSet;
    private string _path;
    public bool ProcessPropertyMap;
    // Start is called before the first frame update
    private void Start()
    {
        MainGui = FindObjectOfType<MainGui>();
    }


    // ReSharper disable Unity.PerformanceAnalysis
    public void BatchLoadTextures()
    {
        StartCoroutine(BatchProcessTextures());
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator BatchProcessTextures()
    {
        var path = StandaloneFileBrowser.Runtime.StandaloneFileBrowser.OpenFolderPanel("Texture Files Location", "", false);
        //var path = StandaloneFileBrowser.SaveFilePanel("Texture Directory", "", "","");
        var s = Directory.GetFiles(path[0], "*.*").Where(g => g.EndsWith(".jpg") || g.EndsWith(".png"));
        foreach (var f in s)
        {
            //BatchProcessTextures(f);

            var data = File.ReadAllBytes(f);
            var tex = new Texture2D(2, 2);
            if (tex.LoadImage(data))
            {
                yield return StartCoroutine(BatchTextures(tex, f));

                //MainGui.HeightFromDiffuseGuiScript.StartCoroutine(ProcessHeight());
                //return null;
            }
        }
        //return null;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator BatchTextures(Texture2D T, string texName)
    {
        MainGui.DiffuseMapOriginal = T;
        MainGui.HeightFromDiffuseGuiObject.SetActive(true);
        MainGui.HeightFromDiffuseGuiScript.NewTexture();
        MainGui.HeightFromDiffuseGuiScript.DoStuff();
        //yield return MainGui.HeightFromDiffuseGuiScript.StartCoroutine(MainGui.HeightFromDiffuseGuiScript.ProcessDiffuse());
        yield return new WaitForSeconds(.1f);
        MainGui.HeightFromDiffuseGuiScript.StartProcessHeight();
        MainGui.CloseWindows();
        MainGui.FixSize();
        MainGui.NormalFromHeightGuiObject.SetActive(true);
        MainGui.NormalFromHeightGuiScript.NewTexture();
        MainGui.NormalFromHeightGuiScript.DoStuff();
        //yield return MainGui.NormalFromHeightGuiScript.StartCoroutine(MainGui.NormalFromHeightGuiScript.ProcessHeight());
        yield return new WaitForSeconds(.1f);
        MainGui.NormalFromHeightGuiScript.StartProcessNormal();
        yield return new WaitForEndOfFrame();
        MainGui.CloseWindows();
        MainGui.FixSize();
        MainGui.MetallicMap = new Texture2D(MainGui.HeightMap.width, MainGui.HeightMap.height);
        var theColor = new Color();
        for (var x = 0; x < MainGui.MetallicMap.width; x++)
        {
            for (var y = 0; y < MainGui.MetallicMap.height; y++)
            {
                theColor.r = 0;
                theColor.g = 0;
                theColor.b = 0;
                theColor.a = 255;
                MainGui.MetallicMap.SetPixel(x, y, theColor);
            }
        }
        // MainGui.MetallicGuiObject.SetActive(true);
        //MainGui.MetallicGuiScript.NewTexture();
        //MainGui.MetallicGuiScript.DoStuff();
        //yield return new WaitForSeconds(.1f);
        //MainGui.MetallicGuiScript.StartCoroutine(MainGui.MetallicGuiScript.ProcessMetallic());
        MainGui.MetallicMap.Apply();
        MainGui.CloseWindows();
        MainGui.FixSize();
        MainGui.SmoothnessGuiObject.SetActive(true);
        MainGui.SmoothnessGuiScript.NewTexture();
        MainGui.SmoothnessGuiScript.DoStuff();
        yield return new WaitForSeconds(.1f);
        MainGui.SmoothnessGuiScript.StartCoroutine(MainGui.SmoothnessGuiScript.ProcessSmoothness());
        MainGui.CloseWindows();
        MainGui.FixSize();
        MainGui.EdgeFromNormalGuiObject.SetActive(true);
        MainGui.EdgeFromNormalGuiScript.NewTexture();
        MainGui.EdgeFromNormalGuiScript.DoStuff();
        yield return new WaitForSeconds(.1f);
        MainGui.EdgeFromNormalGuiScript.StartCoroutine(MainGui.EdgeFromNormalGuiScript.ProcessEdge());
        MainGui.CloseWindows();
        MainGui.FixSize();
        MainGui.AoFromNormalGuiObject.SetActive(true);
        MainGui.AoFromNormalGuiScript.NewTexture();
        MainGui.AoFromNormalGuiScript.DoStuff();
        yield return new WaitForSeconds(.1f);
        MainGui.AoFromNormalGuiScript.StartCoroutine(MainGui.AoFromNormalGuiScript.ProcessAo());

        yield return new WaitForSeconds(.3f);
        
        var names = texName.Split( new[] { "/", "\\" }, StringSplitOptions.None).ToList();
        //Debug.Log(names);
        foreach(var s in names)
        {
            Debug.Log(s);
        }
        var defaultName = names[^1];
        Debug.Log(defaultName);
        names = defaultName.Split('.').ToList();
        defaultName = names[0];
        var nameWithOutExtension = defaultName;
        defaultName = defaultName + ".mtz";
        
        if (UseInitializeLocation)
        {
            if (!_pathIsSet)
            {
                _path = StandaloneFileBrowser.Runtime.StandaloneFileBrowser.SaveFilePanel("Save Project", MainGui.LastDirectory, defaultName, "mtz");
                //if (path.IsNullOrEmpty()) return;
                _pathIsSet = true;
                var lastBar = _path.LastIndexOf(MainGui.PathChar);
                MainGui.LastDirectory = _path.Substring(0, lastBar + 1);

            }
            else
            {
                var pathSplit = _path.Split(new[] { "/", "\\" }, StringSplitOptions.None).ToList();
                //PathSplit[PathSplit.Length - 1]
                pathSplit.RemoveAt(pathSplit.Count - 1);
                //Debug.Log(PathSplit);
                _path = string.Join("/" , pathSplit.ToArray());
                _path = _path+ "/" + defaultName;
                Debug.Log(defaultName);
                //var lastBar = path.LastIndexOf(MainGui._pathChar);
                //MainGui._lastDirectory = path.Substring(0, lastBar + 1);
            }
        }
        else
        {
            _path = StandaloneFileBrowser.Runtime.StandaloneFileBrowser.SaveFilePanel("Save Project", MainGui.LastDirectory, defaultName, "mtz");
            var lastBar = _path.LastIndexOf(MainGui.PathChar);
            MainGui.LastDirectory = _path[..(lastBar + 1)];
        }
        Debug.Log(_path);
        MainGui.SaveLoadProjectScript.SaveProject(_path);
        yield return new WaitForSeconds(1f);
        if (ProcessPropertyMap)
        {
            MainGui.ProcessPropertyMap();
            MainGui.SaveTextureFile(MapType.Property, _path, nameWithOutExtension);
        }
        //return null;
    }
}
