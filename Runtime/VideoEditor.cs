using System;
using System.IO;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Encoder;
using UnityEditor.Recorder.Input;
using UnityEngine;
using UnityEngine.Video;


public class VideoEditor : MonoBehaviour
{
    private static readonly int BaseMapProperty = Shader.PropertyToID("_BaseMap");
    private RenderTexture renderTexture;
    private Vector2 aspectRatio = new(1, 1);
    private VideoPlayer videoPlayer;
    private Camera previewCamera;
    private GameObject quad;
    private Material previewMaterial;
    private RecorderControllerSettings recorderControllerSettings;
    private MovieRecorderSettings movieRecorderSettings;
    private RenderTextureInputSettings imageInputSettings;
    private RecorderController recorderController;
    private VideoClip _storedClip;


    [ContextMenu(nameof(Awake))]
    private void Awake()
    {
        recorderControllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
        recorderController = new RecorderController(recorderControllerSettings);

        movieRecorderSettings = ScriptableObject.CreateInstance<MovieRecorderSettings>();

        movieRecorderSettings.EncoderSettings = new CoreEncoderSettings
        {
            Codec = CoreEncoderSettings.OutputCodec.MP4,
            EncodingQuality = CoreEncoderSettings.VideoEncodingQuality.High
        };

        movieRecorderSettings.RecordMode = RecordMode.Manual;

        recorderControllerSettings.AddRecorderSettings(movieRecorderSettings);

        var videoPlayerObject = new GameObject("VideoPlayer");
        videoPlayerObject.transform.SetParent(gameObject.transform);
        videoPlayer = videoPlayerObject.AddComponent<VideoPlayer>();
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.aspectRatio = VideoAspectRatio.FitVertically;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;

        var cameraObj = new GameObject("VideoPreviewCamera");
        cameraObj.transform.SetParent(gameObject.transform);
        previewCamera = cameraObj.AddComponent<Camera>();
        previewCamera.transform.position = Vector3.zero;
        previewCamera.depth = 100;

        previewMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit")); // Change to Unlit
        previewMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        previewMaterial.SetFloat("_Surface", 0);
        previewMaterial.SetFloat("_Blend", 0);

        quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.transform.SetParent(gameObject.transform);
        quad.transform.position = new Vector3(0, 0, 1);
        var rend = quad.GetComponent<MeshRenderer>();
        rend.material = previewMaterial;
    }


    private void VideoClipChanged()
    {
        if (videoPlayer.clip == null || videoPlayer.clip == _storedClip)
        {
            return;
        }

        var videoPath = AssetDatabase.GetAssetPath(videoPlayer.clip);
        var videoName = Path.GetFileNameWithoutExtension(videoPath);
        var videoDirectory = Path.GetDirectoryName(videoPath);
        var videoOutputPath = $"{videoDirectory}/{videoName}_Trimmed";
        var videoExtension = ".mp4";
        var pathWithExtension = videoOutputPath + videoExtension;
        Debug.Log("PathWithExtension: " + pathWithExtension);

        if (File.Exists(pathWithExtension))
        {
            Debug.LogWarning("Video already exists.");
            videoOutputPath = $"{videoDirectory}/{videoName}_Trimmed_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}";
            Debug.Log("New path: " + videoOutputPath);
        }

        movieRecorderSettings.OutputFile = videoOutputPath;

        Debug.Log(videoPlayer.clip.width + "x" + videoPlayer.clip.height);
        aspectRatio.x = (float) Math.Round((float) videoPlayer.clip.width / videoPlayer.clip.height, 3);
        aspectRatio.y = 1;
        Debug.Log(aspectRatio.x + "x" + aspectRatio.y);
        quad.transform.localScale = new Vector3(aspectRatio.x, aspectRatio.y, 1);

        recorderControllerSettings.FrameRate = (float) videoPlayer.clip.frameRate;

        ResetRenderTexture();

        _storedClip = videoPlayer.clip;
        videoPlayer.Prepare();

        Debug.Log("Video clip changed.");
    }


    private void ResetRenderTexture()
    {
        DestroyExistingTexture();

        renderTexture = new RenderTexture((int) videoPlayer.clip.width, (int) videoPlayer.clip.height, 24, RenderTextureFormat.ARGB32);
        renderTexture.name = "VideoRenderTexture";

        renderTexture.Create();

        previewMaterial.SetTexture(BaseMapProperty, renderTexture);

        videoPlayer.targetTexture = renderTexture;

        imageInputSettings = new RenderTextureInputSettings
        {
            OutputWidth = (int) videoPlayer.clip.width,
            OutputHeight = (int) videoPlayer.clip.height,
            RenderTexture = renderTexture
        };

        movieRecorderSettings.ImageInputSettings = imageInputSettings;

        Debug.Log("Render texture reset.");
    }


    private void Update()
    {
        VideoClipChanged();
    }


    private void StopRecording()
    {
        if (!recorderController.IsRecording())
        {
            Debug.LogWarning("Recorder is not currently recording.");

            return;
        }

        recorderController.StopRecording();
        Debug.Log($"Recording stopped. Video saved to: {movieRecorderSettings.OutputFile}.mp4");
    }


    private void GoToStart()
    {
        videoPlayer.time = 0;
        videoPlayer.Play();
        videoPlayer.Pause();
    }


    [ContextMenu(nameof(Record))]
    private bool Record()
    {
        if (videoPlayer.clip == null)
        {
            Debug.LogWarning("No clip assigned to the VideoPlayer.");

            return false;
        }
        
        recorderController.PrepareRecording();
        recorderController.StartRecording();
        Debug.Log("Recording started.");

        if (!videoPlayer.isPrepared)
        {
            videoPlayer.Prepare();
            videoPlayer.prepareCompleted += _ => { videoPlayer.Play(); };
            Debug.LogWarning("We were not prepared.");
        }
        else
        {
            videoPlayer.Play();
            Debug.Log("We were prepared to start playing.");
        }
        
        return true;
    }


    [ContextMenu(nameof(Play))]
    private bool Play()
    {
        if (videoPlayer.clip == null)
        {
            Debug.LogWarning("No clip assigned to the VideoPlayer.");

            return false;
        }

        if (!videoPlayer.isPrepared)
        {
            videoPlayer.Prepare();
            videoPlayer.prepareCompleted += _ => { videoPlayer.Play(); };
            Debug.LogWarning("We were not prepared.");
        }
        else
        {
            videoPlayer.Play();
            Debug.Log("We were prepared.");
        }

        return true;
    }


    [ContextMenu(nameof(Pause))]
    private bool Pause()
    {
        if (!videoPlayer.isPlaying)
        {
            Debug.LogWarning("VideoPlayer is not playing.");

            return false;
        }

        videoPlayer.Pause();

        return true;
    }


    [ContextMenu(nameof(Stop))]
    private bool Stop()
    {
        if (!videoPlayer.isPlaying)
        {
            Debug.LogWarning("VideoPlayer is not playing.");

            return false;
        }

        videoPlayer.Stop();

        if (recorderController.IsRecording())
        {
            Debug.Log("Recording stopped.");

            StopRecording();
        }

        GoToStart();

        return true;
    }


    private void OnDisable()
    {
        DestroyExistingTexture();
    }


    private void DestroyExistingTexture()
    {
        if (renderTexture == null)
        {
            return;
        }

        renderTexture.Release();
        DestroyImmediate(renderTexture);
    }
}