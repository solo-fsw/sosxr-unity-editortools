using UnityEditor;


namespace SOSXR.EditorSpice
{
    /// <summary>
    ///     Based on Warped Imagination: https://www.youtube.com/watch?v=Q6TK-1ewnGk&ab_channel=WarpedImagination
    /// </summary>
    public class SOSXRProjectSettings : SettingsProvider
    {
        /// <summary>
        ///     UserScope = Preferences
        ///     ProjectScope = Project Settings
        /// </summary>
        /// <param name="path"></param>
        /// <param name="scope"></param>
        public SOSXRProjectSettings(string path, SettingsScope scope = SettingsScope.Project) : base(path, scope)
        {
            // Needs nothing here
        }


        public override void OnGUI(string searchContext)
        {
            base.OnGUI(searchContext);

            SOSXRTestSettings.OnGUI();

            // Add more project settings GUI elements here as needed
        }


        /// <summary>
        ///     UserScope = Preferences
        ///     ProjectScope = Project Settings
        /// </summary>
        /// <returns></returns>
        [SettingsProvider]
        public static SettingsProvider CreateSOSXRSettingsProvider()
        {
            return new SOSXRPreferences("Project/SOSXR", SettingsScope.Project);
        }
    }
}