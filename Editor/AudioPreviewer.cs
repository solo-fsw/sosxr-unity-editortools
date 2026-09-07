using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;


namespace SOSXR.EditorSpice.EditorScripts
{
    /// <summary>
    ///     Based on Warped Imagination: https://youtu.be/Gd8M1Ychis8?si=8QNBxDnJG4W1X8Cz
    ///     and https://www.youtube.com/watch?v=D8hOWOK8ByY
    /// </summary>
    public static class AudioPreviewer
    {
        private static Assembly AudioImporterAssembly => typeof(AudioImporter).Assembly;
        private static Type AudioUtil => AudioImporterAssembly.GetType("UnityEditor.AudioUtil");
#if UNITY_6000_3_OR_NEWER
        private static EntityId? _lastPlayedAudioClipId;
#else
        private static int? _lastPlayedAudioClipId;
#endif


        [OnOpenAsset]
#if UNITY_6000_3_OR_NEWER
        public static bool OnOpenAsset(EntityId entityId, int line)
        {
            var obj = EditorUtility.EntityIdToObject(entityId);
#else
        public static bool OnOpenAsset(int entityId, int line)
        {
            var obj = EditorUtility.InstanceIDToObject(entityId);
#endif

            if (obj is not AudioClip audioClip)
            {
                return false; // Unity has control of this asset
            }

            if (IsPreviewClipPlaying())
            {
                StopAllPreviewClips();

                if (_lastPlayedAudioClipId.HasValue && _lastPlayedAudioClipId != entityId)
                {
                    PlayPreviewClip(audioClip);
                }
            }
            else
            {
                PlayPreviewClip(audioClip);
            }

            _lastPlayedAudioClipId = entityId;

            return true; // I have control of this asset
        }


        public static void PlayPreviewClip(AudioClip audioClip)
        {
            var methodInfo = AudioUtil.GetMethod(
                "PlayPreviewClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new[] { typeof(AudioClip), typeof(int), typeof(bool) },
                null);

            methodInfo?.Invoke(null, new object[] { audioClip, 0, false });
        }


        public static bool IsPreviewClipPlaying()
        {
            var methodInfo = AudioUtil.GetMethod(
                "IsPreviewClipPlaying",
                BindingFlags.Static | BindingFlags.Public);

            return (bool)methodInfo?.Invoke(null, null)!;
        }


        public static void StopAllPreviewClips()
        {
            var methodInfo = AudioUtil.GetMethod(
                "StopAllPreviewClips",
                BindingFlags.Static | BindingFlags.Public);

            methodInfo?.Invoke(null, null);
        }
    }
}
