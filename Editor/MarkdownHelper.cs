using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;


namespace SOSXR.EditorSpice
{
    public static class MarkdownHelper
    {
        [MenuItem("Assets/Create/Markdown file")]
        [MenuItem("SOSXR/Create Markdown file")]
        [MenuItem("Assets/Create/SOSXR/Create Markdown file")]
        private static void CreateMarkdown()
        {
            Create("NewMarkdown");
        }


        private static void Create(string fileName, string extension = ".md", string content = "")
        {
            var folderPath = "Assets";

            if (Selection.assetGUIDs.Length > 0)
            {
                var selectedPath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);

                if (Directory.Exists(selectedPath))
                {
                    folderPath = selectedPath;
                }
                else
                {
                    folderPath = Path.GetDirectoryName(selectedPath);
                }
            }

            var filePath = Path.Combine(folderPath, fileName + extension);

            var action = ScriptableObject.CreateInstance<DoCreateMarkdownFile>();
            action.DefaultContent = content;

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                action,
                filePath,
                EditorGUIUtility.IconContent("TextAsset Icon").image as Texture2D,
                null
            );
        }


        private class DoCreateMarkdownFile : EndNameEditAction
        {
            public string DefaultContent;


            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var fileName = Path.GetFileNameWithoutExtension(pathName);
                var content = string.IsNullOrEmpty(DefaultContent) ? "# " + fileName : DefaultContent;

                File.WriteAllText(pathName, content);
                AssetDatabase.Refresh();

                var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(pathName);
                ProjectWindowUtil.ShowCreatedAsset(asset);
            }
        }
    }
}