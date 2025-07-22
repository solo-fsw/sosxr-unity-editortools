using System.IO;
using SOSXR.EditorSpice;
using UnityEditor;
using UnityEngine;


namespace SOSXR.BuildHelpers
{
    public static class OpenBuildsFolder
    {
        private static readonly string _folderName = "_Builds";


        [MenuItem("SOSXR/Folders/" + nameof(Builds), false, 100)]
        public static void Builds()
        {
            var fullPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, _folderName);

            MenuItems.OpenFolder(fullPath);
        }
    }
}