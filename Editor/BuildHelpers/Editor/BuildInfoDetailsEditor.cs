using System.IO;
using UnityEditor;
using UnityEngine;


namespace SOSXR.BuildHelpers
{
    [CustomEditor(typeof(BuildInfoDetails), true)]
    public class BuildInfoDetailsEditor : Editor
    {
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