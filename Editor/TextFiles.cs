using System.IO;
using UnityEditor;
using UnityEngine;


/// <summary>
///     Based on spiney199 on https://discussions.unity.com/t/how-can-i-create-text-files/915171
/// </summary>
public static class TextFiles
{
    [MenuItem("Assets/Create/SOSXR/Create Markdown file here", priority = 50)]
    [MenuItem("SOSXR/Create Markdown file here", priority = 100)]
    private static void CreateMarkdown()
    {
        Create("NewTextFile");
    }
    

    private static void Create(string fileName, string extension = ".md", string content = "", bool atRoot = false)
    {
        var folderDirectory = "";

        if (atRoot)
        {
            folderDirectory = Path.GetFullPath(Application.dataPath + "/..");
        }
        else
        {
            var folderGUID = Selection.assetGUIDs[0];
            var projectFolderPath = AssetDatabase.GUIDToAssetPath(folderGUID);
            folderDirectory = Path.GetFullPath(projectFolderPath);
        }

        if (string.IsNullOrEmpty(content))
        {
            content = "# " + fileName;
        }

        using (var sw = File.CreateText(folderDirectory + "/" + fileName + extension))
        {
            sw.WriteLine(content);
        }

        AssetDatabase.Refresh();
    }
}