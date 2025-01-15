using System;
using System.IO;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Encoder;
using UnityEditor.Recorder.Input;
using UnityEngine;
using UnityEngine.Video;


[Serializable]
public class VideoEditor : EditorWindow
{
    // Serialized fields to persist data
    [SerializeField] private VideoClip videoClip;
    [SerializeField] private Vector2Int videoSize = new(1920, 1080);
    [SerializeField] private Vector2 aspectRatio = new(1, 1);
    [SerializeField] private float startTrim;
    [SerializeField] private float endTrim;
    [SerializeField] private float currentTime;
    [SerializeField] private string videoPlayerGUID;
    [SerializeField] private string cameraGUID;
    [SerializeField] private string quadGUID;
    private static readonly int BaseMapProperty = Shader.PropertyToID("_BaseMap");

    // Non-serialized runtime references
    private RenderTexture renderTexture;
    private VideoPlayer videoPlayer;
    private Camera previewCamera;
    private GameObject quad;
    private Material previewMaterial;
    private RecorderController recorderController;
    private RecorderControllerSettings recorderControllerSettings;
    private MovieRecorderSettings movieRecorderSettings;
    private RenderTextureInputSettings imageInputSettings;
    private bool isMonitoringPlayback;
    private bool isPaused;
    private float savedStartTime;
    private float savedEndTime;
    private Color originalColor;
    private float newDuration;

    protected const float ButtonWidth = 150f;
    private const string PreviewMaterialPath = "Materials/VideoPreviewMaterial";


    [MenuItem("SOSXR/Video Editor")]
    public static void ShowWindow()
    {
        GetWindow<VideoEditor>("Video Editor");
    }


    private void OnEnable()
    {
        originalColor = GUI.backgroundColor;

        InitializeRecorder();

        RestoreOrCreatePreviewObjects();

        ResetRenderTexture(videoSize);

        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }


    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            SaveObjectGUIDs(); // Save GUIDs before objects are destroyed
        }
        else if (state == PlayModeStateChange.EnteredEditMode) // Restore objects after play mode
        {
            RestoreOrCreatePreviewObjects();
            ResetRenderTexture(videoSize);
        }
    }


    private void SaveObjectGUIDs()
    {
        if (videoPlayer != null)
        {
            videoPlayerGUID = GlobalObjectId.GetGlobalObjectIdSlow(videoPlayer.gameObject).ToString();
        }

        if (previewCamera != null)
        {
            cameraGUID = GlobalObjectId.GetGlobalObjectIdSlow(previewCamera.gameObject).ToString();
        }

        if (quad != null)
        {
            quadGUID = GlobalObjectId.GetGlobalObjectIdSlow(quad).ToString();
        }
    }


    private void InitializeRecorder()
    {
        recorderControllerSettings = CreateInstance<RecorderControllerSettings>();
        recorderController = new RecorderController(recorderControllerSettings);

        movieRecorderSettings = CreateInstance<MovieRecorderSettings>();
        movieRecorderSettings.name = "MovieRecorderSettings";

        // Store recorded video in the same folder where the original video is located
        if (videoClip != null)
        {
            var videoPath = AssetDatabase.GetAssetPath(videoClip);
            var videoName = Path.GetFileNameWithoutExtension(videoPath);
            var videoDirectory = Path.GetDirectoryName(videoPath);
            var videoExtension = ".mp4";
            var videoOutputPath = $"{videoDirectory}/{videoName}_Trimmed{videoExtension}";
            movieRecorderSettings.OutputFile = videoOutputPath;
        }

        movieRecorderSettings.EncoderSettings = new CoreEncoderSettings
        {
            Codec = CoreEncoderSettings.OutputCodec.MP4,
            EncodingQuality = CoreEncoderSettings.VideoEncodingQuality.High
        };

        movieRecorderSettings.RecordMode = RecordMode.Manual;

        imageInputSettings = new RenderTextureInputSettings();
        movieRecorderSettings.ImageInputSettings = imageInputSettings;

        recorderControllerSettings.AddRecorderSettings(movieRecorderSettings);
    }


    private void RestoreOrCreatePreviewObjects()
    {
        // Try to restore video player
        if (string.IsNullOrEmpty(videoPlayerGUID) || !TryRestoreObject(videoPlayerGUID, out var videoObj))
        {
            videoObj = new GameObject("VideoPreviewPlayer");
            videoPlayer = videoObj.AddComponent<VideoPlayer>();
        }
        else
        {
            videoPlayer = videoObj.GetComponent<VideoPlayer>();
        }

        SetupVideoPlayer();

        // Try to restore camera
        if (string.IsNullOrEmpty(cameraGUID) || !TryRestoreObject(cameraGUID, out var cameraObj))
        {
            cameraObj = new GameObject("VideoPreviewCamera");
            previewCamera = cameraObj.AddComponent<Camera>();
        }
        else
        {
            previewCamera = cameraObj.GetComponent<Camera>();
        }

        SetupCamera();

        // Create or load material
        CreateOrLoadMaterial();

        // Try to restore quad
        if (string.IsNullOrEmpty(quadGUID) || !TryRestoreObject(quadGUID, out var quadObj))
        {
            quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        }
        else
        {
            quad = quadObj;
        }

        SetupQuad();
    }


    private bool TryRestoreObject(string guid, out GameObject obj)
    {
        obj = null;

        if (string.IsNullOrEmpty(guid))
        {
            return false;
        }

        GlobalObjectId.TryParse(guid, out var id);
        obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(id) as GameObject;

        return obj != null;
    }


    private void SetupVideoPlayer()
    {
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.aspectRatio = VideoAspectRatio.FitVertically; // Add this
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None; // Add this
        
        if (videoClip == null)
        {
            return;
        }

        videoPlayer.clip = videoClip;
        videoPlayer.targetTexture = renderTexture;
        videoPlayer.Prepare(); // Add this

        var videoPath = AssetDatabase.GetAssetPath(videoClip);
        var videoName = Path.GetFileNameWithoutExtension(videoPath);
        var videoDirectory = Path.GetDirectoryName(videoPath);
        // var videoExtension = ".mp4";
        var videoOutputPath = $"{videoDirectory}/{videoName}_Trimmed";

        if (File.Exists(videoOutputPath+".mp4"))
        {
            videoOutputPath = string.Concat(videoOutputPath, "_1");
        }

        if (movieRecorderSettings != null)
        {
            movieRecorderSettings.OutputFile = videoOutputPath;
        }
    }


    private void SetupCamera()
    {
        previewCamera.transform.position = Vector3.zero;
        previewCamera.depth = 100;
    }


    private void CreateOrLoadMaterial()
    {
        previewMaterial = AssetDatabase.LoadAssetAtPath<Material>(PreviewMaterialPath);

        if (previewMaterial == null)
        {
            previewMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit")); // Change to Unlit
            previewMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            previewMaterial.SetFloat("_Surface", 0);
            previewMaterial.SetFloat("_Blend", 0);
        }
    }


    private void SetupQuad()
    {
        quad.transform.position = new Vector3(0, 0, 1.25f);
        quad.transform.localScale = new Vector3(aspectRatio.x, aspectRatio.y, 1);
        quad.transform.rotation = Quaternion.Euler(0, 0, 0);

        var renderer = quad.GetComponent<MeshRenderer>();
        renderer.material = previewMaterial;
    }


    private void ResetRenderTexture(Vector2Int size)
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            DestroyImmediate(renderTexture);
        }

        renderTexture = new RenderTexture(size.x, size.y, 24, RenderTextureFormat.ARGB32);
        renderTexture.Create();

        if (previewMaterial != null)
        {
            previewMaterial.SetTexture(BaseMapProperty, renderTexture);
        }

        if (videoPlayer != null)
        {
            videoPlayer.targetTexture = renderTexture;
        }

        UpdateRecorderSettings(size);
    }


    private void UpdateRecorderSettings(Vector2Int size)
    {
        if (movieRecorderSettings != null)
        {
            imageInputSettings = new RenderTextureInputSettings
            {
                OutputWidth = size.x,
                OutputHeight = size.y,
                RenderTexture = renderTexture
            };

            movieRecorderSettings.ImageInputSettings = imageInputSettings;
            movieRecorderSettings.OutputFormat = MovieRecorderSettings.VideoRecorderOutputFormat.MP4;
        }
    }


    private void OnDisable()
    {
        SaveObjectGUIDs();
        StopMonitoringPlayback();
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;

        if (renderTexture != null)
        {
            renderTexture.Release();
            DestroyImmediate(renderTexture);
        }
    }


    private void OnGUI()
    {
        EditorGUILayout.LabelField("Video Editor", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();

        videoClip = (VideoClip) EditorGUILayout.ObjectField("Video Clip", videoClip, typeof(VideoClip), false);

        if (EditorGUI.EndChangeCheck())
        {
            videoSize.x = (int) videoClip.width;
            videoSize.y = (int) videoClip.height;

            ResetRenderTexture(videoSize);

            videoPlayer.clip = videoClip;
            var x = (float) Math.Round((float) videoSize.x / videoSize.y, 2);
            aspectRatio = new Vector2(x, 1);
            quad.transform.localScale = new Vector3(aspectRatio.x, aspectRatio.y, 1);
            startTrim = 0;
            currentTime = 0;
            endTrim = (float) videoClip.length;
            recorderControllerSettings.FrameRate = (float) videoClip.frameRate;
        }

        EditorGUI.BeginChangeCheck();

        videoSize = EditorGUILayout.Vector2IntField("Video Size", videoSize);

        if (EditorGUI.EndChangeCheck())
        {
            ResetRenderTexture(videoSize);
        }

        aspectRatio = EditorGUILayout.Vector2Field("Aspect Ratio", aspectRatio);

        if (videoPlayer == null)
        {
            return;
        }
        if (videoClip == null || videoPlayer.clip == null)
        {
            return;
        }

        CustomInspectorContent();
    }


    protected void CustomInspectorContent()
    {
        if (!Mathf.Approximately(savedStartTime, startTrim))
        {
            videoPlayer.time = startTrim;
            savedStartTime = startTrim;
            videoPlayer.Play();
            videoPlayer.Pause();
            currentTime = 0;
        }

        if (!Mathf.Approximately(savedEndTime, endTrim))
        {
            if (Event.current.type == EventType.MouseDrag)
            {
                videoPlayer.time = endTrim;
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                savedEndTime = endTrim;
                videoPlayer.time = startTrim;
                videoPlayer.Play();
                videoPlayer.Pause();
            }

            currentTime = 0;
        }

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (Application.isPlaying)
        {
            if (DrawRecordButton(videoPlayer))
            {
                return;
            }
        }


        if (DrawPlayButton(videoPlayer))
        {
            return;
        }

        if (DrawPauseButton(videoPlayer))
        {
            return;
        }


        if (DrawStopButton(videoPlayer))
        {
            return;
        }

        GUI.backgroundColor = originalColor;

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        DrawTimeControls();
        DrawControlsSection();

        Repaint();
    }


    private void DrawTimeControls()
    {
        if (videoPlayer.clip == null)
        {
            return;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("", GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        var totalLength = FormatTime((float) videoPlayer.clip.length);
        EditorGUILayout.LabelField($"Original Duration: {totalLength}", GUILayout.Width(200));
        EditorGUILayout.LabelField($"Trim Range: {FormatTime(startTrim)} - {FormatTime(endTrim)}", GUILayout.Width(200));

        if (videoPlayer.isPlaying)
        {
            currentTime = Mathf.Round((float) ((videoPlayer.time - startTrim) * 100f)) / 100f;
        }

        EditorGUILayout.LabelField($"Current Time: {FormatTime(currentTime)}", GUILayout.Width(200));

        newDuration = endTrim - startTrim;
        EditorGUILayout.LabelField($"New Duration: {FormatTime(newDuration)}", GUILayout.Width(200));

        EditorGUILayout.EndHorizontal();
    }


    private string FormatTime(float timeInSeconds)
    {
        var minutes = Mathf.FloorToInt(timeInSeconds / 60);
        var seconds = Mathf.FloorToInt(timeInSeconds % 60);
        var milliseconds = Mathf.FloorToInt(timeInSeconds * 1000 % 1000);

        return timeInSeconds >= 60 ? $"{minutes:00}:{seconds:00}.{milliseconds:00}" : $"{seconds:00}.{milliseconds:000}";
    }


    private void DrawControlsSection()
    {
        EditorGUILayout.Space();

        EditorGUILayout.MinMaxSlider("Trim", ref startTrim, ref endTrim, 0f, (float) videoPlayer.clip.length);
    }


    private void StartMonitoringPlayback()
    {
        if (!videoPlayer.isPlaying)
        {
            return;
        }

        // Add this debug check
        if (!videoPlayer.isPrepared)
        {
            Debug.LogWarning("Video player is not prepared!");

            return;
        }

        if (!isMonitoringPlayback)
        {
            isMonitoringPlayback = true;
            EditorApplication.update += MonitorPlaybackTime;
        }
    }


    private void StopMonitoringPlayback()
    {
        if (isMonitoringPlayback)
        {
            isMonitoringPlayback = false;
            EditorApplication.update -= MonitorPlaybackTime;

            StopRecording();
        }
    }


    private void StopRecording()
    {
        var videoPath = AssetDatabase.GetAssetPath(videoClip);
        var videoName = Path.GetFileNameWithoutExtension(videoPath);
        var videoDirectory = Path.GetDirectoryName(videoPath);
        // var videoExtension = ".mp4";
        var videoOutputPath = $"{videoDirectory}/{videoName}_Trimmed";

        if (File.Exists(videoOutputPath+".mp4"))
        {
            // add current minute and second to the file name
            var currentTime = DateTime.Now;
            videoOutputPath = string.Concat(videoOutputPath, currentTime.Minute, currentTime.Second);
            Debug.LogWarning("File already exists. Saving to: " + videoOutputPath);
        }

        if (movieRecorderSettings != null)
        {
            movieRecorderSettings.OutputFile = videoOutputPath;
        }
        
        if (recorderController.IsRecording())
        {
            recorderController.StopRecording();
            Debug.Log($"Recording stopped. Video saved to: {movieRecorderSettings.OutputFile}");
        }
        else
        {
            Debug.LogWarning("Recorder is not currently recording.");
        }
    }


    private void MonitorPlaybackTime()
    {
        if (!videoPlayer.isPlaying)
        {
            return;
        }

        if (videoPlayer.time >= endTrim)
        {
            videoPlayer.Stop();
            Debug.Log("Playback stopped as endTrim was reached.");
            StopMonitoringPlayback();
            GoToStart();
        }
    }


    private void GoToStart()
    {
        currentTime = 0;
        videoPlayer.time = startTrim;
        videoPlayer.Play();
        videoPlayer.Pause();
    }


    private bool DrawRecordButton(VideoPlayer editorTarget)
    {
        GUI.backgroundColor = editorTarget.isPlaying && !isPaused && recorderController.IsRecording() ? Color.green : originalColor;

        if (GUILayout.Button("Record", GUILayout.Width(ButtonWidth)))
        {
            if (editorTarget.clip == null)
            {
                Debug.LogWarning("No clip assigned to the VideoPlayer.");

                return true;
            }

            editorTarget.time = startTrim;
            editorTarget.Play();
            isPaused = false;

            if (recorderController == null)
            {
                Debug.LogWarning("RecorderController is null.");
            }
            else
            {
                recorderController.PrepareRecording();
                recorderController.StartRecording();
            }

            StartMonitoringPlayback(); // Start checking playback time
        }

        return false;
    }


    private bool DrawPlayButton(VideoPlayer editorTarget)
    {
        GUI.backgroundColor = editorTarget.isPlaying && !isPaused ? Color.green : originalColor;

        if (GUILayout.Button("Play", GUILayout.Width(ButtonWidth)))
        {
            if (editorTarget.clip == null)
            {
                Debug.LogWarning("No clip assigned to the VideoPlayer.");

                return true;
            }

            editorTarget.time = startTrim;
            editorTarget.Play();
            isPaused = false;
            StartMonitoringPlayback(); // Start checking playback time
        }

        return false;
    }


    private bool DrawPauseButton(VideoPlayer editorTarget)
    {
        GUI.backgroundColor = isPaused ? Color.yellow : originalColor;

        if (GUILayout.Button("Pause", GUILayout.Width(ButtonWidth)))
        {
            if (!editorTarget.isPlaying)
            {
                Debug.LogWarning("VideoPlayer is not playing.");

                return true;
            }

            editorTarget.Pause();
            isPaused = true;
        }

        return false;
    }


    private bool DrawStopButton(VideoPlayer editorTarget)
    {
        GUI.backgroundColor = !editorTarget.isPlaying && !isPaused ? originalColor : Color.red;

        if (GUILayout.Button("Stop", GUILayout.Width(ButtonWidth)))
        {
            editorTarget.Stop();
            isPaused = false;

            StopMonitoringPlayback(); // Stop checking playback time
            GoToStart();
        }

        return false;
    }
}