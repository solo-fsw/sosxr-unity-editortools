using System;
using UnityEditor;
using UnityEngine;


namespace SOSXR.EditorSpice
{
    /// <summary>
    ///     Based on Warped Imagination: https://www.youtube.com/watch?v=Q6TK-1ewnGk&ab_channel=WarpedImagination
    ///     Create a POCO with a [InitializeOnLoad] attribute. In a static constructor, subscribe a GUI method to the OnGUIEvent of this ProjectSettingsProvider.
    ///     Properties in the POCO can be used to store user preferences, which can be accessed throughout the project.
    ///     Samples on how to use this can be found on the SOSXR SeaShark package: https://github.com/solo-fsw/sosxr-unity-seashark.git
    /// </summary>
    public class ProjectSettingsProvider : SettingsProvider
    {
        /// <summary>
        ///     UserScope = Preferences
        ///     ProjectScope = Project Settings
        /// </summary>
        /// <returns></returns>
        public ProjectSettingsProvider(string path, SettingsScope scope = SettingsScope.Project) : base(path, scope)
        {
        }


        private static readonly string _path = "Project/SOSXR/EditorSpice";


        public static Action OnGUIEvent;


        public override void OnGUI(string searchContext)
        {
            GUILayout.Space(20);

            OnGUIEvent?.Invoke();
        }


        /// <summary>
        ///     UserScope = Preferences
        ///     ProjectScope = Project Settings
        /// </summary>
        /// <returns></returns>
        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            return new ProjectSettingsProvider(_path);
        }
    }
}