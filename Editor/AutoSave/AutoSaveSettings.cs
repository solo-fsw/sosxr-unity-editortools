using SOSXR.EditorSpice.EditorScripts;
using UnityEditor;
using UnityEngine;


namespace Tarodev
{
    [InitializeOnLoad]
    public class AutoSaveSettings
    {
        static AutoSaveSettings()
        {
            if (_subscribed)
            {
                return;
            }

            ProjectSettingsProvider.OnGUIEvent += OnGUI;
            _subscribed = true;
        }


        public static bool Enabled
        {
            get => EditorPrefs.GetBool(EnabledKey, true);
            set => EditorPrefs.SetBool(EnabledKey, value);
        }

        public static int Frequency
        {
            get => EditorPrefs.GetInt(FrequencyKey, 1);
            set => EditorPrefs.SetInt(FrequencyKey, Mathf.Clamp(value, 1, 60));
        }

        private const string EnabledKey = "AutoSave_Enabled";
        private const string FrequencyKey = "AutoSave_Frequency";

        private static readonly bool _subscribed;


        private static void OnGUI()
        {
            GUILayout.Space(20);
            GUILayout.Label("Tarodev's Auto Save Settings", EditorStyles.boldLabel);

            Enabled = EditorGUILayout.Toggle("Enabled", Enabled);

            if (Enabled)
            {
                GUILayout.BeginHorizontal();
                Frequency = EditorGUILayout.IntSlider("Frequency", Frequency, 1, 60, GUILayout.Width(300));
                GUILayout.Label("minutes", GUILayout.Width(60));
                GUILayout.EndHorizontal();
            }
        }
    }
}