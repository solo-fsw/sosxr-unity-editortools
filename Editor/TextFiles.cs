using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;


namespace SOSXR.EditorTools
{
    public static class TextFiles
    {
        [MenuItem("SOSXR/Create Markdown file here")]
        [MenuItem("Assets/Create/SOSXR/Create Markdown file here")]
        private static void CreateMarkdown()
        {
            Create("NewTextFile");
        }


        private static void Create(string fileName, string extension = ".md", string content = "")
        {
            if (Selection.assetGUIDs.Length == 0)
            {
                Debug.LogWarning("No asset selected. Please select a folder or asset to create the Markdown file.");

                return;
            }

            var folderGUID = Selection.assetGUIDs[0];
            var projectFolderPath = AssetDatabase.GUIDToAssetPath(folderGUID);
            var relativeFilePath = projectFolderPath + "/" + fileName + extension;

            // Start renaming process without creating the file yet
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                ScriptableObject.CreateInstance<DoCreateMarkdownFile>(),
                relativeFilePath,
                EditorGUIUtility.IconContent("TextAsset Icon").image as Texture2D,
                content
            );
        }


        private class DoCreateMarkdownFile : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                // Extract the file name (without extension) for use as the first line in the file
                var fileName = Path.GetFileNameWithoutExtension(pathName);

                // Create content using the chosen filename as the first line with a "#"
                var content = "# " + fileName;

                // If the file does not exist, create it with the desired content
                if (!File.Exists(pathName))
                {
                    File.WriteAllText(pathName, content);
                    AssetDatabase.Refresh();
                }

                // Load the created asset and select it in the Project view
                var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(pathName);
                Selection.activeObject = asset;
            }
        }
    }
}