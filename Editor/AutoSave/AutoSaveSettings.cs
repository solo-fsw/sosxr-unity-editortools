using UnityEngine;


namespace Tarodev
{
    /// <summary>
    /// Configuration for the AutoSave feature.
    /// <para>
    /// This ScriptableObject is created in the project to control when and how Unity scenes are auto-saved.
    /// </para>
    /// <para>
    /// Configuration is exposed in Project Settings via this asset.
    /// </para>
    /// </summary>
    [CreateAssetMenu(fileName = "AutoSaveConfig", menuName = "SOSXR/AutoSaveConfig")]
    public class AutoSaveConfig : ScriptableObject
    {
        /// <summary>
        /// Enable or disable the auto-save feature.
        /// </summary>
        [Tooltip("Enable auto save functionality")]
        public bool Enabled;

        /// <summary>
        /// Frequency, in minutes, at which the editor will auto-save open scenes.
        /// </summary>
        [Tooltip("The frequency in minutes auto save will activate")] [Min(1)]
        public int Frequency = 1;

        /// <summary>
        /// When true, logs a message each time an auto-save occurs.
        /// </summary>
        [Tooltip("Log a message every time the scene is auto saved")]
        public bool Logging;
    }
}
