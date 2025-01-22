using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(VideoEditor))]
public class VideoEditorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var videoEditor = (VideoEditor) target;

        if (GUILayout.Button(nameof(videoEditor.Record)))
        {
            videoEditor.Record();
        }

        if (GUILayout.Button(nameof(videoEditor.Play)))
        {
            videoEditor.Play();
        }
        
        if (GUILayout.Button(nameof(videoEditor.Pause)))
        {
            videoEditor.Pause();
        }
        
        if (GUILayout.Button(nameof(videoEditor.Stop)))
        {
            videoEditor.Stop();
        }
    }
}