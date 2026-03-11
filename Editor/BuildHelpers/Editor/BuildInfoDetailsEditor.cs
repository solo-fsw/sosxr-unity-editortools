using System.IO;
using UnityEditor;
using UnityEngine;


namespace SOSXR.BuildHelpers
{
    /// <summary>
    /// Custom editor for BuildInfoDetails assets. Adds quick reveal functionality to locate the associated asset in the Project window.
    /// </summary>
    [CustomEditor(typeof(BuildInfoDetails), true)]
    public class BuildInfoDetailsEditor : Editor
    {
        /// <summary>
        /// Draws the default inspector and adds a helper button to reveal the corresponding asset in the Project view.
        /// </summary>
        public override void OnInspectorGUI()
        {
            var buildInfoDetails = (BuildInfoDetails) target;

            base.OnInspectorGUI();

            if (GUILayout.Button("Reveal in Project View"))
            {
                var assetPath = GetAssetPath(buildInfoDetails.FilePath);

                if (!string.IsNullOrEmpty(assetPath))
                {
                    var obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

                    if (obj != null)
                    {
                        Selection.activeObject = obj;
                        EditorGUIUtility.PingObject(obj);
                    }
                }
            }
        }


        private string GetAssetPath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }

            var relativePath = filePath.Replace(Application.dataPath, "Assets");

            return File.Exists(relativePath) ? relativePath : null;
        }
    }
}
