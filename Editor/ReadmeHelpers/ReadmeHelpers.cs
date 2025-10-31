using System.IO;
using UnityEditor;
using UnityEngine;


public static class ReadmeHelpers
{
    /// <summary>
    ///     Creates a README.md file in the Assets folder.
    ///     It grabs a file called README_template.md from a Resources folder.
    /// </summary>
    [MenuItem("Assets/Create/SOSXR/Create README.md file in Assets folder", priority = 100)]
    [MenuItem("SOSXR/Setup/Create README.md file in Assets folder")]
    private static void CreateReadmeAtRoot()
    {
        var folderDirectory = Path.GetFullPath(Application.dataPath + "/Assets/..");
        var fullPath = folderDirectory + "/" + "README.md";

        if (File.Exists(fullPath))
        {
            Debug.LogWarning("README.md already exists at " + fullPath + ". Will not create another one.");

            return;
        }

        var template = Resources.Load<TextAsset>("README_template"); // From Resources folder

        if (template != null)
        {
            var templateContent = template.text;

            using var sw = File.CreateText(fullPath);

            sw.WriteLine("# " + GetProjectName());
            sw.WriteLine(templateContent);
        }
        else
        {
            Debug.LogError("Template file not found in Resources.");
        }

        AssetDatabase.Refresh();
    }


    public static string GetProjectName()
    {
        var split = Application.dataPath.Split('/');

        var projectName = split[^2];

        return projectName;
    }
}