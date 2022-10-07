#region

using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using Materialize.General;
using UnityEngine;
using Utility;
using Logger = Utility.Logger;

#endregion

public enum MapType
{
    Height,
    Diffuse,
    DiffuseOriginal,
    Metallic,
    Smoothness,
    Normal,
    Edge,
    Ao,
    Property
}

public enum FileFormat
{
    Png,
    Jpg,
    Tga,
    Exr
}

public class ProjectObject
{
    public string AoMapPath;

    public AoSettings AoSettings;
    public string DiffuseMapOriginalPath;
    public string DiffuseMapPath;
    public string EdgeMapPath;

    public EdgeSettings EdgeSettings;

    public EditDiffuseSettings EditDiffuseSettings;
    public HeightFromDiffuseSettings HeightFromDiffuseSettings;
    public string HeightMapPath;

    public MaterialSettings MaterialSettings;
    public string MetallicMapPath;

    public MetallicSettings MetallicSettings;

    public NormalFromHeightSettings NormalFromHeightSettings;
    public string NormalMapPath;
    public string SmoothnessMapPath;

    public SmoothnessSettings SmoothnessSettings;
}

public class SaveLoadProject : MonoBehaviour
{
    private char _pathChar;
    private ProjectObject _thisProject;
    [SerializeField] private MainGui MainGUI;

    [HideInInspector] public bool Busy;

    // Use this for initialization
    private void Start()
    {
        if (MainGUI == null)
        {
            MainGUI = FindObjectOfType<MainGui>();
        }

        if (Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.WindowsPlayer)
            _pathChar = '\\';
        else
            _pathChar = '/';

        _thisProject = new ProjectObject();
    }

    public void LoadProject(string pathToFile)
    {
        Debug.Log("Loading Project: " + pathToFile);

        var serializer = new XmlSerializer(typeof(ProjectObject));
        var stream = new FileStream(pathToFile, FileMode.Open);
        _thisProject = serializer.Deserialize(stream) as ProjectObject;
        stream.Close();
        MainGui.Instance.HeightFromDiffuseGuiScript.SetValues(_thisProject);
        MainGui.Instance.EditDiffuseGuiScript.SetValues(_thisProject);
        MainGui.Instance.NormalFromHeightGuiScript.SetValues(_thisProject);
        MainGui.Instance.MetallicGuiScript.SetValues(_thisProject);
        MainGui.Instance.SmoothnessGuiScript.SetValues(_thisProject);
        MainGui.Instance.EdgeFromNormalGuiScript.SetValues(_thisProject);
        MainGui.Instance.AoFromNormalGuiScript.SetValues(_thisProject);
        MainGui.Instance.MaterialGuiScript.SetValues(_thisProject);

        MainGui.Instance.ClearAllTextures();

        StartCoroutine(LoadAllTextures(pathToFile));
    }

    public void SaveProject(string pathToFile)
    {
        if (pathToFile.Contains("."))
            pathToFile =
                pathToFile.Substring(0, pathToFile.LastIndexOf(".", StringComparison.Ordinal));

        Debug.Log("Saving Project: " + pathToFile);

        var extension = MainGui.Instance.SelectedFormat.ToString().ToLower();
        var projectName = pathToFile.Substring(pathToFile.LastIndexOf(_pathChar) + 1);
        Debug.Log("Project Name " + projectName);

        MainGui.Instance.HeightFromDiffuseGuiScript.GetValues(_thisProject);
        if (MainGui.Instance.HeightMap != null)
            _thisProject.HeightMapPath = projectName + "_height." + extension;
        else
            _thisProject.HeightMapPath = "null";

        MainGui.Instance.EditDiffuseGuiScript.GetValues(_thisProject);
        if (MainGui.Instance.DiffuseMap != null)
            _thisProject.DiffuseMapPath = projectName + "_diffuse." + extension;
        else
            _thisProject.DiffuseMapPath = "null";

        if (MainGui.Instance.DiffuseMapOriginal != null)
            _thisProject.DiffuseMapOriginalPath = projectName + "_diffuseOriginal." + extension;
        else
            _thisProject.DiffuseMapOriginalPath = "null";

        MainGui.Instance.NormalFromHeightGuiScript.GetValues(_thisProject);
        if (MainGui.Instance.NormalMap != null)
            _thisProject.NormalMapPath = projectName + "_normal." + extension;
        else
            _thisProject.NormalMapPath = "null";

        MainGui.Instance.MetallicGuiScript.GetValues(_thisProject);
        if (MainGui.Instance.MetallicMap != null)
            _thisProject.MetallicMapPath = projectName + "_metallic." + extension;
        else
            _thisProject.MetallicMapPath = "null";

        MainGui.Instance.SmoothnessGuiScript.GetValues(_thisProject);
        if (MainGui.Instance.SmoothnessMap != null)
            _thisProject.SmoothnessMapPath = projectName + "_smoothness." + extension;
        else
            _thisProject.SmoothnessMapPath = "null";

        MainGui.Instance.EdgeFromNormalGuiScript.GetValues(_thisProject);
        if (MainGui.Instance.EdgeMap != null)
            _thisProject.EdgeMapPath = projectName + "_edge." + extension;
        else
            _thisProject.EdgeMapPath = "null";

        MainGui.Instance.AoFromNormalGuiScript.GetValues(_thisProject);
        if (MainGui.Instance.AoMap != null)
            _thisProject.AoMapPath = projectName + "_ao." + extension;
        else
            _thisProject.AoMapPath = "null";

        MainGui.Instance.MaterialGuiScript.GetValues(_thisProject);

        var serializer = new XmlSerializer(typeof(ProjectObject));
        var stream = new FileStream(pathToFile + ".mtz", FileMode.Create);
        serializer.Serialize(stream, _thisProject);
        stream.Close();

        SaveAllFiles(pathToFile);
    }

    public void SaveAllFiles(string pathToFile) { StartCoroutine(SaveAllTextures(pathToFile)); }

    public void SaveFile(string pathToFile, Texture2D textureToSave)
    {
        StartCoroutine(SaveTexture(textureToSave, pathToFile));
    }

    public void PasteFile(MapType mapTypeToLoad)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return;
        const string filePrefix = "file:///";
        string pathToFile;

        var pathToTextFile = Path.GetTempFileName();
        BashRunner.Run($"xclip -selection clipboard -t TARGETS -o > {pathToTextFile}");
        var bashOut = File.ReadAllText(pathToTextFile);

        Debug.Log($"Out : {bashOut}");
        File.Delete(pathToTextFile);

        if (bashOut.Contains("image/png"))
        {
            pathToFile = Path.GetTempFileName() + ".png";
            BashRunner.Run($"xclip -selection clipboard -t image/png -o > {pathToFile}");
        } else
        {
            BashRunner.Run($"xclip -selection clipboard -o > {pathToTextFile}");
            bashOut = File.ReadAllText(pathToTextFile);

            if (!bashOut.Contains(filePrefix)) return;
            var supported = MainGui.LoadFormats.Any(format => bashOut.Contains(format));
            if (!supported) return;

            var firstIndex = bashOut.IndexOf("file:///", StringComparison.Ordinal);
            var lastIndex = bashOut.IndexOf("\n", firstIndex, StringComparison.Ordinal);
            var length = lastIndex - firstIndex;
            pathToFile = bashOut.Substring(firstIndex, length);
            pathToFile = pathToFile.Replace("file:///", "/");
            Debug.Log("Path " + pathToFile);
        }

        File.Delete(pathToTextFile);


        StartCoroutine(LoadTexture(mapTypeToLoad, pathToFile));
    }

    public void CopyFile(Texture2D textureToSave) { }

    //==============================================//
    //			Texture Saving Coroutines			//
    //==============================================//


    private IEnumerator SaveAllTextures(string pathToFile)
    {
        var path = pathToFile.Substring(0, pathToFile.LastIndexOf(_pathChar) + 1);
        yield return StartCoroutine(SaveTexture(MainGui.Instance.HeightMap,
            path + _thisProject.HeightMapPath));

        yield return StartCoroutine(SaveTexture(MainGui.Instance.DiffuseMap,
            path + _thisProject.DiffuseMapPath));

        yield return StartCoroutine(SaveTexture(MainGui.Instance.DiffuseMapOriginal,
            path + _thisProject.DiffuseMapOriginalPath));

        yield return StartCoroutine(SaveTexture(MainGui.Instance.NormalMap,
            path + _thisProject.NormalMapPath));

        yield return StartCoroutine(SaveTexture(MainGui.Instance.MetallicMap,
            path + _thisProject.MetallicMapPath));

        yield return StartCoroutine(SaveTexture(MainGui.Instance.SmoothnessMap,
            path + _thisProject.SmoothnessMapPath));

        yield return StartCoroutine(SaveTexture(MainGui.Instance.EdgeMap,
            path + _thisProject.EdgeMapPath));

        yield return StartCoroutine(SaveTexture(MainGui.Instance.AoMap,
            path + _thisProject.AoMapPath));
    }

    public IEnumerator SaveTexture(string extension, Texture2D textureToSave, string pathToFile)
    {
        yield return StartCoroutine(SaveTexture(textureToSave, pathToFile + "." + extension));
    }

    private IEnumerator SaveTexture(Texture2D textureToSave, string pathToFile)
    {
        if (!textureToSave || string.IsNullOrEmpty(pathToFile)) yield break;
        if (MainGui.Instance.ScaleTexture)
        {
            //TextureScale.BilinearScale(_textureToSave);
            textureToSave = TextureScale.Bilinear(textureToSave, int.Parse(MainGui.Instance.XSize),
                int.Parse(MainGui.Instance.YSize));
        }

        Debug.Log($"Salvando {textureToSave} como {pathToFile}");
        if (!pathToFile.Contains("."))
            pathToFile = $"{pathToFile}.{MainGui.Instance.SelectedFormat}";

        var fileIndex = pathToFile.LastIndexOf('.');
        var extension = pathToFile.Substring(fileIndex + 1, pathToFile.Length - fileIndex - 1);

        switch (extension)
        {
            case "png":
            {
                var pngBytes = textureToSave.EncodeToPNG();
                File.WriteAllBytes(pathToFile, pngBytes);
                break;
            }
            case "jpg":
            {
                var jpgBytes = textureToSave.EncodeToJPG();
                File.WriteAllBytes(pathToFile, jpgBytes);
                break;
            }
            case "tga":
            {
                var tgaBytes = textureToSave.EncodeToTGA();
                File.WriteAllBytes(pathToFile, tgaBytes);
                break;
            }
            case "exr":
            {
                var exrBytes = textureToSave.EncodeToEXR();
                File.WriteAllBytes(pathToFile, exrBytes);
                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(extension), extension, null);
        }

        Resources.UnloadUnusedAssets();


        yield return new WaitForSeconds(0.1f);
    }

    //==============================================//
    //			Texture Loading Coroutines			//
    //==============================================//

    private IEnumerator LoadAllTextures(string pathToFile)
    {
        pathToFile = pathToFile.Substring(0, pathToFile.LastIndexOf(_pathChar) + 1);

        if (_thisProject.HeightMapPath != "null")
            StartCoroutine(LoadTexture(MapType.Height, pathToFile + _thisProject.HeightMapPath));

        while (Busy) yield return new WaitForSeconds(0.01f);

        if (_thisProject.DiffuseMapOriginalPath != "null")
            StartCoroutine(LoadTexture(MapType.DiffuseOriginal,
                pathToFile + _thisProject.DiffuseMapOriginalPath));

        while (Busy) yield return new WaitForSeconds(0.01f);

        if (_thisProject.DiffuseMapPath != "null")
            StartCoroutine(LoadTexture(MapType.Diffuse, pathToFile + _thisProject.DiffuseMapPath));

        while (Busy) yield return new WaitForSeconds(0.01f);

        if (_thisProject.NormalMapPath != "null")
            StartCoroutine(LoadTexture(MapType.Normal, pathToFile + _thisProject.NormalMapPath));

        while (Busy) yield return new WaitForSeconds(0.01f);

        if (_thisProject.MetallicMapPath != "null")
            StartCoroutine(LoadTexture(MapType.Metallic,
                pathToFile + _thisProject.MetallicMapPath));

        while (Busy) yield return new WaitForSeconds(0.01f);

        if (_thisProject.SmoothnessMapPath != "null")
            StartCoroutine(LoadTexture(MapType.Smoothness,
                pathToFile + _thisProject.SmoothnessMapPath));

        while (Busy) yield return new WaitForSeconds(0.01f);

        if (_thisProject.EdgeMapPath != "null")
            StartCoroutine(LoadTexture(MapType.Edge, pathToFile + _thisProject.EdgeMapPath));

        while (Busy) yield return new WaitForSeconds(0.01f);

        if (_thisProject.AoMapPath != "null")
            StartCoroutine(LoadTexture(MapType.Ao, pathToFile + _thisProject.AoMapPath));

        while (Busy) yield return new WaitForSeconds(0.01f);

        yield return new WaitForSeconds(0.01f);
    }

    public IEnumerator LoadTexture(MapType textureToLoad, string pathToFile)
    {
        Busy = true;

        Texture2D newTexture = null;
        pathToFile = Uri.UnescapeDataString(pathToFile);


        if (File.Exists(pathToFile))
        {
            newTexture = GetTextureFromFile(pathToFile);
        }

        if (!newTexture) yield break;
        newTexture.anisoLevel = 9;


        switch (textureToLoad)
        {
            case MapType.Height:
                MainGui.Instance.HeightMap = newTexture;
                break;
            case MapType.Diffuse:
                MainGui.Instance.DiffuseMap = newTexture;
                break;
            case MapType.DiffuseOriginal:
                MainGui.Instance.DiffuseMapOriginal = newTexture;
                break;
            case MapType.Normal:
                MainGui.Instance.NormalMap = newTexture;
                break;
            case MapType.Metallic:
                MainGui.Instance.MetallicMap = newTexture;
                break;
            case MapType.Smoothness:
                MainGui.Instance.SmoothnessMap = newTexture;
                break;
            case MapType.Edge:
                MainGui.Instance.EdgeMap = newTexture;
                break;
            case MapType.Ao:
                MainGui.Instance.AoMap = newTexture;
                break;
            case MapType.Property:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(textureToLoad), textureToLoad, null);
        }

        MainGui.Instance.SetLoadedTexture(textureToLoad);

        Resources.UnloadUnusedAssets();


        yield return new WaitForSeconds(0.01f);

        Busy = false;
    }

    private static ProgramEnums.FileFormat GetFormat(string path)
    {
        var format = path.Substring(path.LastIndexOf(".", StringComparison.Ordinal) + 1, 3);
        Logger.Log($"Carregando {format}");

        if (!Enum.TryParse(format, true, out ProgramEnums.FileFormat fileFormat))
            return ProgramEnums.FileFormat.Invalid;

        Logger.Log("Tipo encontrado : " + fileFormat);
        return fileFormat;
    }

    private static Texture2D LoadPngBmpJpg(string path)
    {
        var newTexture = new Texture2D(2, 2);
        if (!File.Exists(path)) return null;

        var fileData = File.ReadAllBytes(path);

        newTexture.LoadImage(fileData);

        return newTexture;
    }


    public static Texture2D GetTextureFromFile(string pathToFile)
    {
        pathToFile = Uri.UnescapeDataString(pathToFile);

        var fileFormat = GetFormat(pathToFile);

        Texture2D newTexture = null;
        switch (fileFormat)
        {
            case ProgramEnums.FileFormat.Png:
            case ProgramEnums.FileFormat.Jpg:
            case ProgramEnums.FileFormat.Bmp:
                newTexture = LoadPngBmpJpg(pathToFile);
                break;
            case ProgramEnums.FileFormat.Tga:
                newTexture = TGALoader.LoadTGA(pathToFile);
                break;
            case ProgramEnums.FileFormat.Invalid:
                Logger.Log("Tipo de arquivo invalido " + fileFormat);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return newTexture;
    }
}