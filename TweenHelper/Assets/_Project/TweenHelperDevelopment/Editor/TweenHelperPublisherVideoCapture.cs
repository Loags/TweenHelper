using System;
using System.Collections.Generic;
using System.IO;
using LB.TweenHelper.Demo;
using TMPro;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEngine;
using UnityEngine.UI;

public static class TweenHelperPublisherVideoCapture
{
    private const string MenuPath = "Tools/Tween Helper Dev/Validation/Capture Animation Gallery Stills";

    [MenuItem(MenuPath)]
    private static void CaptureAnimationGalleryStills()
    {
        if (!Application.isPlaying) throw new InvalidOperationException("Enter Play Mode in TweenHelperAnimationGallery before starting Recorder validation.");
        if (UnityEngine.Object.FindAnyObjectByType<AnimationGalleryRecorderValidationRunner>() != null)
        {
            throw new InvalidOperationException("Animation Gallery Recorder validation is already running.");
        }

        AnimationGalleryController controller = UnityEngine.Object.FindAnyObjectByType<AnimationGalleryController>();
        if (controller == null) throw new InvalidOperationException("AnimationGalleryController was not found in the active Play Mode scene.");

        var runnerObject = new GameObject("Animation Gallery Recorder Validation");
        runnerObject.AddComponent<AnimationGalleryRecorderValidationRunner>().Initialize(controller, GetOutputDirectory());
    }

    [MenuItem(MenuPath, true)]
    private static bool CanCaptureAnimationGalleryStills() => Application.isPlaying;

    private static string GetOutputDirectory()
    {
        string projectRoot = Path.GetFullPath(Directory.GetCurrentDirectory());
        string tempRoot = Path.GetFullPath(Path.Combine(projectRoot, "Temp"));
        string output = Path.GetFullPath(Path.Combine(tempRoot, "RoadmapValidation", "Recorder"));
        if (!output.StartsWith(tempRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Recorder validation output escaped the project Temp directory.");
        }

        return output;
    }
}

public sealed class AnimationGalleryRecorderValidationRunner : MonoBehaviour
{
    private static readonly Vector2Int[] Resolutions =
    {
        new Vector2Int(1920, 1080)
    };

    private AnimationGalleryController _controller;
    private Button[] _categoryButtons;
    private string _outputDirectory;
    private RecorderController _recorderController;
    private RecorderControllerSettings _controllerSettings;
    private ImageRecorderSettings _imageRecorderSettings;
    private int _resolutionIndex;
    private int _stateIndex;
    private int _frameWait;
    private bool _captureStarted;
    private bool _openDropdown;
    private Dropdown _openedDropdown;
    private readonly List<string> _capturedFiles = new List<string>();

    public void Initialize(AnimationGalleryController controller, string outputDirectory)
    {
        _controller = controller;
        _outputDirectory = outputDirectory;
        _categoryButtons = GetPrivateField<Button[]>(controller, "categoryButtons");
        if (_categoryButtons == null || _categoryButtons.Length != Enum.GetValues(typeof(AnimationGalleryCategory)).Length)
        {
            throw new InvalidOperationException("Gallery category-button wiring does not match the category enum.");
        }

        Directory.CreateDirectory(_outputDirectory);
        foreach (string existingFile in Directory.GetFiles(_outputDirectory, "*.png")) File.Delete(existingFile);

        ApplyState();
    }

    private void Update()
    {
        try
        {
            UpdateValidation();
        }
        catch (Exception exception)
        {
            enabled = false;
            Debug.LogException(new InvalidOperationException($"Animation Gallery Recorder validation failed in state {_stateIndex} at resolution {_resolutionIndex}.", exception));
        }
    }

    private void UpdateValidation()
    {
        if (_frameWait > 0)
        {
            _frameWait--;
            return;
        }

        if (!_captureStarted)
        {
            if (_openDropdown && _openedDropdown == null)
            {
                _openedDropdown = GetActiveOptionDropdown();
                if (_openedDropdown == null) throw new InvalidOperationException("Gallery validation could not find an active option dropdown.");
                _openedDropdown.Show();
                _frameWait = 2;
                return;
            }

            StartCapture();
            return;
        }

        if (_recorderController.IsRecording()) return;

        FinishCapture();
        AdvanceState();
    }

    private void ApplyState()
    {
        _openDropdown = false;
        if (_stateIndex < _categoryButtons.Length)
        {
            _categoryButtons[_stateIndex].onClick.Invoke();
            ApplyRepresentativeOption();
        }
        else if (_stateIndex == _categoryButtons.Length)
        {
            SelectEntry(AnimationGalleryCategory.UIRecipes, 4);
        }
        else
        {
            SelectEntry(AnimationGalleryCategory.Collections, 0);
            ApplyRepresentativeOption();
            _openDropdown = true;
        }

        _frameWait = 12;
        _captureStarted = false;
    }

    private void ApplyRepresentativeOption()
    {
        AnimationGalleryOptionView[] optionViews = UnityEngine.Object.FindObjectsByType<AnimationGalleryOptionView>(FindObjectsInactive.Exclude);
        foreach (AnimationGalleryOptionView optionView in optionViews)
        {
            TMP_Dropdown dropdown = optionView.GetComponentInChildren<TMP_Dropdown>(true);
            if (dropdown == null || !dropdown.gameObject.activeInHierarchy || dropdown.options.Count < 2) continue;
            dropdown.value = (_stateIndex + 1) % dropdown.options.Count;
            dropdown.onValueChanged.Invoke(dropdown.value);
            break;
        }
    }

    private void SelectEntry(AnimationGalleryCategory category, int index)
    {
        var showCategory = _controller.GetType().GetMethod("ShowCategory", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        var select = _controller.GetType().GetMethod("Select", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (showCategory == null || select == null) throw new InvalidOperationException("Required gallery selection methods were not found.");
        showCategory.Invoke(_controller, new object[] { category });
        var visibleEntries = GetPrivateField<List<AnimationGalleryEntry>>(_controller, "_visibleEntries");
        if (index < 0 || index >= visibleEntries.Count) throw new InvalidOperationException($"Gallery validation could not select visible entry {index} in {category}.");
        select.Invoke(_controller, new object[] { visibleEntries[index], true });
    }

    private static Dropdown GetActiveOptionDropdown()
    {
        AnimationGalleryOptionView[] optionViews = UnityEngine.Object.FindObjectsByType<AnimationGalleryOptionView>(FindObjectsInactive.Exclude);
        foreach (AnimationGalleryOptionView optionView in optionViews)
        {
            Dropdown dropdown = optionView.GetComponentInChildren<Dropdown>(true);
            if (dropdown != null && dropdown.gameObject.activeInHierarchy) return dropdown;
        }

        return null;
    }

    private void StartCapture()
    {
        Vector2Int resolution = Resolutions[_resolutionIndex];
        string fileName = $"{GetStateSlug()}-{resolution.x}x{resolution.y}";
        string outputFile = Path.Combine(_outputDirectory, fileName);

        _controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
        _imageRecorderSettings = ScriptableObject.CreateInstance<ImageRecorderSettings>();
        _imageRecorderSettings.name = "Tween Helper Gallery Validation Still";
        _imageRecorderSettings.Enabled = true;
        _imageRecorderSettings.OutputFormat = ImageRecorderSettings.ImageRecorderOutputFormat.PNG;
        _imageRecorderSettings.CaptureAlpha = false;
        _imageRecorderSettings.OutputFile = outputFile;
        _imageRecorderSettings.imageInputSettings = new GameViewInputSettings
        {
            OutputWidth = resolution.x,
            OutputHeight = resolution.y
        };

        _controllerSettings.AddRecorderSettings(_imageRecorderSettings);
        _controllerSettings.SetRecordModeToSingleFrame(0);
        _recorderController = new RecorderController(_controllerSettings);
        _recorderController.PrepareRecording();
        if (!_recorderController.StartRecording()) throw new InvalidOperationException($"Recorder could not start for {fileName}.");

        _capturedFiles.Add(outputFile + ".png");
        _captureStarted = true;
        _frameWait = 2;
    }

    private void FinishCapture()
    {
        if (_recorderController.IsRecording()) _recorderController.StopRecording();
        if (_openedDropdown != null)
        {
            _openedDropdown.Hide();
            _openedDropdown = null;
        }
        Destroy(_imageRecorderSettings);
        Destroy(_controllerSettings);
        _imageRecorderSettings = null;
        _controllerSettings = null;
        _recorderController = null;
        _captureStarted = false;
    }

    private void AdvanceState()
    {
        _stateIndex++;
        if (_stateIndex >= _categoryButtons.Length + 2)
        {
            _stateIndex = 0;
            _resolutionIndex++;
        }

        if (_resolutionIndex >= Resolutions.Length)
        {
            ValidateOutputs();
            Debug.Log($"Animation Gallery Recorder validation captured {_capturedFiles.Count} stills in {_outputDirectory}.");
            Destroy(gameObject);
            return;
        }

        ApplyState();
    }

    private void ValidateOutputs()
    {
        foreach (string file in _capturedFiles)
        {
            if (!File.Exists(file) || new FileInfo(file).Length == 0) throw new InvalidOperationException($"Recorder did not produce a valid still: {file}");
        }
    }

    private static T GetPrivateField<T>(object target, string fieldName)
    {
        var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (field == null) throw new InvalidOperationException($"Required gallery field was not found: {fieldName}");
        return (T)field.GetValue(target);
    }

    private static string GetSlug(AnimationGalleryCategory category)
    {
        string name = category.ToString();
        var characters = new List<char>(name.Length + 4);
        for (int i = 0; i < name.Length; i++)
        {
            char character = name[i];
            bool beginsWord = i > 0 && char.IsUpper(character) && (!char.IsUpper(name[i - 1]) || i + 1 < name.Length && char.IsLower(name[i + 1]));
            if (beginsWord) characters.Add('-');
            characters.Add(char.ToLowerInvariant(character));
        }

        return new string(characters.ToArray());
    }

    private string GetStateSlug()
    {
        if (_stateIndex < _categoryButtons.Length) return GetSlug((AnimationGalleryCategory)_stateIndex);
        return _stateIndex == _categoryButtons.Length ? "ui-hover" : "collections-option-menu";
    }
}
