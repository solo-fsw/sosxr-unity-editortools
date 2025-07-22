////////////////////////////////////////////////////////////////////////////////

using System.IO;
using UnityEditor;
using UnityEngine;


namespace MG.MDV
{
    public class Menus
    {
        private static string GetFilePath(string filename)
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (string.IsNullOrEmpty(path))
            {
                path = "Assets";
            }
            else if (AssetDatabase.IsValidFolder(path) == false)
            {
                path = Path.GetDirectoryName(path);
            }

            return AssetDatabase.GenerateUniqueAssetPath(path + "/" + filename);
        }
    }
}