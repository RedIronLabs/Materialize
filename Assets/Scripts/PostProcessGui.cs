#region

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

#endregion

public class PostProcessGui : MonoBehaviour
{
    private const int MinHeight = 40;
    [SerializeField] private PostProcessVolume Volume;
    [SerializeField] private GameObject TestObject;

    private bool _enablePostProcess = true;
    private bool _initialized;
    private PostProcessProfile _profile;

    //Bloom
    private Bloom _bloom;
    private bool _bloomEnabled;
    private float _bloomIntensity;
    private float _bloomThreshold;
    private float _lensDirtIntensity;

    //Depth of Field
    private DepthOfField _depthOfField;
    private bool _dofEnabled;
    private float _dofFocalLength;
    private float _dofFocalDistance;
    private bool _autoFocus = true;

    //AmbientOcclusion
    private AmbientOcclusion _ambientOcclusion;
    private float _ambientOcclusionIntensity;

    //Vignette
    private Vignette _vignette;
    private bool _vignetteEnabled;
    private float _vignetteIntensity;
    private float _vignetteSmoothness;

    private Rect _windowRect = new(350, 350, 300, MinHeight);
    public GameObject MainCamera;

    // Use this for initialization
    private void Start()
    {
        var volume = Volume.GetComponent<PostProcessVolume>();
        // ReSharper disable once InvertIf
        if (volume)
        {
            var orgProfile = volume.profile;
            var inst = Instantiate(orgProfile);
            _profile = inst;
            volume.profile = inst;
        }

        Initialize();
    }

    private void Update()
    {
        if (_enablePostProcess)
        {
            if (_depthOfField)
            {
                if (_autoFocus)
                {
                    _depthOfField.focusDistance.value =
                        _autoFocus ? AutoFocus() : _dofFocalDistance;
                }
            }

            PostProcessOn();
        } else
        {
            PostProcessOff();
        }
    }


    // ReSharper disable Unity.PerformanceAnalysis
    private void Initialize()
    {
        if (_initialized) return;

        _bloom = _profile.GetSetting<Bloom>();
        if (_bloom)
        {
            _bloomEnabled = _bloom.enabled.value;
            _bloomIntensity = _bloom.intensity.value;
            _bloomThreshold = _bloom.threshold;
            //Lens Dirt
            _lensDirtIntensity = _bloom.dirtIntensity.value;
        }

        _depthOfField = _profile.GetSetting<DepthOfField>();
        if (_depthOfField)
        {
            _dofEnabled = _depthOfField.enabled.value;
            _dofFocalLength = _depthOfField.focalLength;
            _dofFocalDistance = _depthOfField.focusDistance;
        }

        _ambientOcclusion = _profile.GetSetting<AmbientOcclusion>();
        if (_ambientOcclusion)
        {
            _ambientOcclusionIntensity = _ambientOcclusion.intensity.value;
        }

        _vignette = _profile.GetSetting<Vignette>();
        if (_vignette)
        {
            _vignetteEnabled = _vignette.enabled.value;
            _vignetteIntensity = _vignette.intensity.value;
            _vignetteSmoothness = _vignette.smoothness.value;
        }

        _initialized = true;
    }

    private void UpdateValues()
    {
        if (_bloom)
        {
            _bloom.enabled.value = _bloomEnabled;
            _bloom.intensity.value = _bloomIntensity;
            _bloom.threshold.value = _bloomThreshold;
            _bloom.dirtIntensity.value = _lensDirtIntensity;
        }

        if (_depthOfField)
        {
            _depthOfField.enabled.value = _dofEnabled;
            _depthOfField.focalLength.value = _dofFocalLength;
        }

        if (_ambientOcclusion)
        {
            _ambientOcclusion.intensity.value = _ambientOcclusionIntensity;
        }

        // ReSharper disable once InvertIf
        if (_vignette)
        {
            _vignette.enabled.value = _vignetteEnabled;
            _vignette.intensity.value = _vignetteIntensity;
            _vignette.smoothness.value = _vignetteSmoothness;
        }
    }

    public void PostProcessOn() { Volume.enabled = true; }

    public void PostProcessOff() { Volume.enabled = false; }

    private float AutoFocus()
    {
        var dist = Vector3.Dot(TestObject.transform.position - MainCamera.transform.position,
            MainCamera.transform.forward);

        return dist;
    }

    private void DoMyWindow(int windowId)
    {
        const int offsetX = 10;
        var offsetY = 30;

        _enablePostProcess = GUI.Toggle(new Rect(offsetX, offsetY, 280, 30), _enablePostProcess,
            "Enable Post Process");
        offsetY += 30;

        if (_enablePostProcess)
        {
            //Bloom
            _bloomEnabled = GUI.Toggle(new Rect(offsetX, offsetY, 150, 20), _bloomEnabled,
                "Use Bloom");
            offsetY += 30;
            if (_bloomEnabled)
            {
                GuiHelper.Slider(new Rect(offsetX, offsetY, 280, 50), "Bloom Intensity",
                    _bloomIntensity, out _bloomIntensity, 0.0f, 8.0f);
                offsetY += 40;

                GuiHelper.Slider(new Rect(offsetX, offsetY, 280, 50), "Bloom Threshold",
                    _bloomThreshold, out _bloomThreshold, 0.0f, 1.0f);
                offsetY += 40;

                GuiHelper.Slider(new Rect(offsetX, offsetY, 280, 50), "Lens Dirt Intensity",
                    _lensDirtIntensity, out _lensDirtIntensity, 0.0f, 5.0f);
                offsetY += 45;
            }

            //DoF
            _dofEnabled = GUI.Toggle(new Rect(offsetX, offsetY, 150, 20), _dofEnabled,
                "Use Depth of Field");
            offsetY += 30;
            if (_dofEnabled)
            {
                GuiHelper.Slider(new Rect(offsetX, offsetY, 280, 50), "DoF Focal Length",
                    _dofFocalLength, out _dofFocalLength, 0.0f, 200.0f);
                offsetY += 40;

                _autoFocus = GUI.Toggle(new Rect(offsetX, offsetY, 150, 20), _autoFocus,
                    "Use Auto Focus");
                offsetY += 30;

                if (!_autoFocus)
                {
                    GuiHelper.Slider(new Rect(offsetX, offsetY, 280, 50), "DoF Focal Distance",
                        _dofFocalDistance, out _dofFocalDistance, 0.0f, 20.0f);
                    offsetY += 40;
                }

                offsetY += 5;
            }

            //Ambient Occlusion
            GuiHelper.Slider(new Rect(offsetX, offsetY, 280, 50), "Ambient Occlusion Intensity",
                _ambientOcclusionIntensity, out _ambientOcclusionIntensity, 0.0f, 4.0f);
            offsetY += 45;

            //Vignette
            _vignetteEnabled = GUI.Toggle(new Rect(offsetX, offsetY, 150, 20), _vignetteEnabled,
                "Use Vignette");
            offsetY += 30;
            if (_vignetteEnabled)
            {
                GuiHelper.Slider(new Rect(offsetX, offsetY, 280, 50), "Vignette Intensity",
                    _vignetteIntensity, out _vignetteIntensity, 0.0f, 8.0f);
                offsetY += 40;

                GuiHelper.Slider(new Rect(offsetX, offsetY, 280, 50), "Vignette Smoothness",
                    _vignetteSmoothness, out _vignetteSmoothness, 0.0f, 1.0f);
                offsetY += 45;
            }
        }

        if (GUI.Button(new Rect(offsetX + 150, offsetY, 130, 30), "Close"))
            gameObject.SetActive(false);

        _windowRect.height = MinHeight + offsetY;
        UpdateValues();
        GUI.DragWindow();
    }

    private void OnGUI() { _windowRect = GUI.Window(19, _windowRect, DoMyWindow, "Post Process"); }
}