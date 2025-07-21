using UnityEditor;
using UnityEngine;


namespace SOSXR.EditorSpice
{
    /// <summary>
    ///     Based on Warped Imagination: https://www.youtube.com/watch?v=Q6TK-1ewnGk&ab_channel=WarpedImagination
    /// </summary>
    public class SOSXRTestSettings
    {
        /// <summary>
        ///     You get/set the value of the preference using a property like this.
        /// </summary>
        public static bool TestOption
        {
            get => EditorPrefs.GetBool(_testOption, true);
            set => EditorPrefs.SetBool(_testOption, value);
        }

        /// <summary>
        ///     This stores the value of the preference to disk, so that it persists between Unity sessions.
        /// </summary>
        private const string _testOption = "TestOption";


        /// <summary>
        ///     This is just a GUI element to display the test option in the Unity Editor, and then setting that value.
        /// </summary>
        public static void OnGUI()
        {
            GUILayout.Space(20);

            var isEnabled = TestOption;

            var value = EditorGUILayout.ToggleLeft(EditorExtensions.ConvertCamelCaseToSpace(_testOption), isEnabled, GUILayout.Width(200));

            if (isEnabled != value)
            {
                TestOption = value;
            }
        }
    }
}