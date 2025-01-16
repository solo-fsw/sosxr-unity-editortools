using System.IO;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Encoder;
using UnityEditor.Recorder.Input;
using UnityEngine;
using UnityEngine.Video;


public class VideoEditor : MonoBehaviour
{



    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private float aspectRatio;
    private static readonly int BaseMapProperty = Shader.PropertyToID("_BaseMap");
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
        movieRecorderSettings.name = "MovieRecorderSettings";


        movieRecorderSettings.EncoderSettings = new CoreEncoderSettings
        {
            Codec = CoreEncoderSettings.OutputCodec.MP4,
            EncodingQuality = CoreEncoderSettings.VideoEncodingQuality.High
        };

        movieRecorderSettings.RecordMode = RecordMode.Manual;

        imageInputSettings = new RenderTextureInputSettings();
        movieRecorderSettings.ImageInputSettings = imageInputSettings;

        recorderControllerSettings.AddRecorderSettings(movieRecorderSettings);

        var videoPlayerObject = new GameObject("VideoPlayer");
        videoPlayerObject.transform.SetParent(gameObject.transform);
        videoPlayer = videoPlayerObject.AddComponent<VideoPlayer>();
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.aspectRatio = VideoAspectRatio.FitVertically;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;

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
        var videoExtension = ".mp4";
        var videoOutputPath = $"{videoDirectory}/{videoName}_Trimmed{videoExtension}";
        movieRecorderSettings.OutputFile = videoOutputPath;

        aspectRatio = (float) videoPlayer.clip.width / videoPlayer.clip.height;
        quad.transform.localScale = new Vector3(aspectRatio, 1, 1);

        recorderControllerSettings.FrameRate = (float) videoPlayer.clip.frameRate;

        ResetRenderTexture(new Vector2(aspectRatio, 1));

        _storedClip = videoPlayer.clip;
        videoPlayer.Prepare();
    }


    private void ResetRenderTexture(Vector2 size)
    {
        DestroyExistingTexture();

        renderTexture = new RenderTexture((int) videoPlayer.clip.width, (int) videoPlayer.clip.height, 24, RenderTextureFormat.ARGB32);
        renderTexture.name = "VideoRenderTexture";

        renderTexture.Create();

        previewMaterial.SetTexture(BaseMapProperty, renderTexture);

        videoPlayer.targetTexture = renderTexture;

        imageInputSettings = new RenderTextureInputSettings
        {
            OutputWidth = (int) size.x,
            OutputHeight = (int) size.y,
            RenderTexture = renderTexture
        };

        movieRecorderSettings.ImageInputSettings = imageInputSettings;
    }


    private void Update()
    {
        VideoClipChanged();

        MonitorPlaybackTime();

        if (videoPlayer.clip == null)
        {
            return;
        }

        Debug.Log($"Video Width: {videoPlayer.clip.width}, Height: {videoPlayer.clip.height}");
        Debug.Log($"RenderTexture Width: {renderTexture.width}, Height: {renderTexture.height}");
    }


    private string FormatTime(float timeInSeconds)
    {
        var minutes = Mathf.FloorToInt(timeInSeconds / 60);
        var seconds = Mathf.FloorToInt(timeInSeconds % 60);
        var milliseconds = Mathf.FloorToInt(timeInSeconds * 1000 % 1000);

        return timeInSeconds >= 60 ? $"{minutes:00}:{seconds:00}.{milliseconds:00}" : $"{seconds:00}.{milliseconds:000}";
    }


    private void StopRecording()
    {
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

        // if (videoPlayer.time >= endTrim)
        {
            //  videoPlayer.Stop();
            Debug.Log("Playback stopped as endTrim was reached.");
            //   StopRecording();
            //    GoToStart();
        }
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

        //videoPlayer.time = startTrim;

        if (!videoPlayer.isPrepared)
        {
            videoPlayer.Prepare();
            videoPlayer.prepareCompleted += _ => { videoPlayer.Play(); };
        }
        else
        {
            videoPlayer.Play();
        }


        recorderController.PrepareRecording();
        recorderController.StartRecording();


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

        // videoPlayer.time = startTrim;
        videoPlayer.Play();


        return true;
    }


    [ContextMenu(nameof(Pause))]
    private bool Pause()
    {
        videoPlayer.Pause();


        return true;
    }


    [ContextMenu(nameof(Stop))]
    private bool Stop()
    {
        videoPlayer.Stop();


        if (recorderController.IsRecording())
        {
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