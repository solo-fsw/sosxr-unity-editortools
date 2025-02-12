#if UNITY_EDITOR

using System;
using System.IO;
using UnityEditor;
using UnityEngine;


public class AddValidateInterfaceToMonoBehaviours : EditorWindow
{
    private readonly string _additionalCode = @"
    


    ";

    private string selectedFolderPath;


    [MenuItem("SOSXR/Add IValidate Interface to Scripts..")]
    public static void ShowWindow()
    {
        GetWindow<AddValidateInterfaceToMonoBehaviours>("Add IValidate Interface");
    }


    private void OnGUI()
    {
        if (GUILayout.Button("Select Directory"))
        {
            selectedFolderPath = EditorUtility.OpenFolderPanel("Select Folder", Application.dataPath, "");
        }

        GUILayout.Label("Selected Directory: " + (string.IsNullOrEmpty(selectedFolderPath) ? "None" : selectedFolderPath));

        if (!string.IsNullOrEmpty(selectedFolderPath))
        {
            if (GUILayout.Button("Add Interface to MonoBehaviours"))
            {
                AddInterfaceToScripts(selectedFolderPath);
            }
        }
        else
        {
            GUILayout.Label("Please select a directory.");
        }
    }


    private void AddInterfaceToScripts(string directoryPath)
    {
        if (!directoryPath.StartsWith(Application.dataPath))
        {
            Debug.LogError("The selected folder must be within the Assets directory.");

            return;
        }

        var scriptPaths = Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories);

        foreach (var scriptPath in scriptPaths)
        {
            // Skip files in any "Editor" subdirectory
            if (scriptPath.Contains("/Editor/") || scriptPath.Contains(@"\Editor\"))
            {
                continue;
            }

            var fileName = Path.GetFileName(scriptPath);
            var scriptText = File.ReadAllText(scriptPath);

            // Check if the file contains a MonoBehaviour class and doesn't already implement the interface
            if (!scriptText.Contains(" : MonoBehaviour") || scriptText.Contains("IValidate"))
            {
                continue;
            }

            // Find the index of "MonoBehaviour" and insert the interface
            var index = scriptText.IndexOf(" : MonoBehaviour", StringComparison.Ordinal);
            scriptText = scriptText.Insert(index + " : MonoBehaviour".Length, ", IValidate");

            // Insert the IsValid property and OnValidate method at the correct position
            var lastNamespaceIndex = scriptText.LastIndexOf("namespace", StringComparison.Ordinal);
            var lastClassIndex = scriptText.LastIndexOf("class", StringComparison.Ordinal);
            var lastBraceIndex = scriptText.LastIndexOf("}", StringComparison.Ordinal);

            if (lastNamespaceIndex != -1 && lastClassIndex > lastNamespaceIndex)
            {
                // Ensure we're inserting before the last closing brace of the class, not the namespace
                var classSection = scriptText.Substring(lastClassIndex, lastBraceIndex - lastClassIndex);
                var lastClassBraceIndex = classSection.LastIndexOf("}", StringComparison.Ordinal);

                if (lastClassBraceIndex != -1)
                {
                    var insertIndex = lastClassIndex + lastClassBraceIndex;

                    scriptText = scriptText.Insert(insertIndex, _additionalCode);
                }
            }
            else if (lastBraceIndex != -1)
            {
                scriptText = scriptText.Insert(lastBraceIndex - 1, _additionalCode);
            }

            File.WriteAllText(scriptPath, scriptText);

            Debug.Log($"Modified {fileName}");
        }

        AssetDatabase.Refresh();
    }
}


#endif