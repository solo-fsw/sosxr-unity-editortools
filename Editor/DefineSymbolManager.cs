using UnityEditor;
using UnityEngine;


namespace SOSXR.EditorTools
{
    [InitializeOnLoad]
    public class DefineSymbolManager
    {
        static DefineSymbolManager()
        {
            // Add the define symbol when the script is loaded
            AddDefineSymbol("SOSXR_EDITORTOOLS_INSTALLED");
        }


        // Add the define symbol to the current build target group
        public static void AddDefineSymbol(string defineSymbol)
        {
            var currentBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

            var currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(currentBuildTargetGroup);

            if (!currentDefines.Contains(defineSymbol))
            {
                currentDefines = currentDefines + ";" + defineSymbol;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(currentBuildTargetGroup, currentDefines);
                Debug.Log($"Added define symbol: {defineSymbol}");
            }
            else
            {
                Debug.Log($"Define symbol {defineSymbol} is already present.");
            }
        }


        // Remove the define symbol from the current build target group
        public static void RemoveDefineSymbol(string defineSymbol)
        {
            var currentBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

            var currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(currentBuildTargetGroup);

            if (currentDefines.Contains(defineSymbol))
            {
                currentDefines = currentDefines.Replace(defineSymbol, "").Replace(";;", ";"); // Remove the symbol and clean up extra semicolons
                PlayerSettings.SetScriptingDefineSymbolsForGroup(currentBuildTargetGroup, currentDefines);
                Debug.Log($"Removed define symbol: {defineSymbol}");
            }
            else
            {
                Debug.Log($"Define symbol {defineSymbol} not found.");
            }
        }
    }


    // This class is used to handle asset changes, such as when a script or package is removed
    public class AssetPostprocessorHandler : AssetPostprocessor
    {
        // Called when assets are imported, deleted, or moved
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            // Check if any assets related to the package (or script) have been deleted
            foreach (var deletedAsset in deletedAssets)
            {
                Debug.LogFormat("Deleted asset: {0}", deletedAsset);

                if (deletedAsset.Contains("SOSXR_EditorTools"))
                {
                    // If the package or script is deleted, remove the define symbol
                    DefineSymbolManager.RemoveDefineSymbol("SOSXR_EDITORTOOLS_INSTALLED");
                    Debug.Log("SOSXR Editor Tools package removed. Define symbol removed.");

                    break;
                }
            }
        }
    }
}