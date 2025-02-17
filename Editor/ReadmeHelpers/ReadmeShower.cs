using System.IO;
using UnityEditor;
using UnityEngine;


[InitializeOnLoad]
public class ReadmeShower : Editor
{
    private static readonly string kShowedReadmeSessionStateName = "ReadmeEditor.showedReadme";


    static ReadmeShower()
    {
        EditorApplication.delayCall += SelectReadmeAutomatically; // Show on Editor Start
    }


    private static void SelectReadmeAutomatically()
    {
        if (SessionState.GetBool(kShowedReadmeSessionStateName, false))
        {
            return;
        }

        var readmeContent = SelectReadme();
        SessionState.SetBool(kShowedReadmeSessionStateName, true);
    }


    [MenuItem("SOSXR/Show README.md", priority = 100)]
    private static string SelectReadme()
    {
        var mdFilePaths = Directory.GetFiles(Application.dataPath, "README.md", SearchOption.AllDirectories);

        if (mdFilePaths.Length >= 1)
        {
            var mdFilePath = mdFilePaths[0]; // Select the first .md file found (you can change the logic here if needed)

            var readmeContent = File.ReadAllText(mdFilePath);

            // Convert absolute path to relative path
            var relativePath = "Assets" + mdFilePath.Replace(Application.dataPath, "").Replace('\\', '/');
            var asset = AssetDatabase.LoadAssetAtPath<Object>(relativePath);

            if (asset != null)
            {
                Selection.activeObject = asset;
            }
            else
            {
                Debug.LogWarning("The .md file was not found as an asset in the AssetDatabase. It might not be imported as an asset.");
            }

            return readmeContent;
        }

        Debug.Log("Couldn't find a readme file with .md extension");

        return null;
    }
}