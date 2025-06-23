using System.IO;
using UnityEditor;
using UnityEngine;


namespace SOSXR.EditorSpice
{
    public static class MenuItems
    {
        [MenuItem("SOSXR/Folders/" + nameof(PersistentDataPath), false, 100)]
        private static void PersistentDataPath()
        {
            var fullPath = Path.Combine(Application.persistentDataPath);

            OpenFolder(fullPath);
        }


        [MenuItem("SOSXR/Folders/" + nameof(AssetFolder), false, 100)]
        private static void AssetFolder()
        {
            var fullPath = Path.Combine(Application.dataPath);

            OpenFolder(fullPath);
        }

        public static void OpenFolder(string path)
        {
            if (Directory.Exists(path))
            {
                EditorUtility.RevealInFinder(path);
            }
            else
            {
                Debug.LogWarning($"Folder not found at path: {path}");
            }
        }
    }
}