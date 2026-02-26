using UnityEditor;
using UnityEditor.Build;
using UnityEngine;


namespace SOSXR.EditorSpice.EditorScripts
{
    // [InitializeOnLoad]
    public class DefineSymbolManager
    {
        static DefineSymbolManager()
        {
            AddDefineSymbol("SOSXR_EDITORTOOLS_INSTALLED");
        }


        public static void AddDefineSymbol(string defineSymbol)
        {
            var namedBuildTarget = GetCurrentNamedBuildTarget();

            var currentDefines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);

            if (!currentDefines.Contains(defineSymbol))
            {
                currentDefines = currentDefines + ";" + defineSymbol;
                PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, currentDefines);
                Debug.Log($"Added define symbol: {defineSymbol}");
            }
        }


        public static void RemoveDefineSymbol(string defineSymbol)
        {
            var namedBuildTarget = GetCurrentNamedBuildTarget();

            var currentDefines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);

            if (currentDefines.Contains(defineSymbol))
            {
                currentDefines = currentDefines.Replace(defineSymbol, "").Replace(";;", ";"); // Remove the symbol and clean up extra semicolons
                PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, currentDefines);
                Debug.Log($"Removed define symbol: {defineSymbol}");
            }
            else
            {
                Debug.LogWarning($"Define symbol {defineSymbol} not found.");
            }
        }


        private static NamedBuildTarget GetCurrentNamedBuildTarget()
        {
            var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

            return NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
        }
    }


    // This class is used to handle asset changes, such as when a script or package is removed
    public class AssetPostprocessorHandler : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var deletedAsset in deletedAssets)
            {
                // Debug.LogFormat("Deleted asset: {0}", deletedAsset);

                if (!deletedAsset.Contains("SOSXR_EditorSpice"))
                {
                    continue;
                }

                // If the package or script is deleted, remove the define symbol
                DefineSymbolManager.RemoveDefineSymbol("SOSXR_EDITORTOOLS_INSTALLED");

                Debug.Log("SOSXR Editor Tools package removed. Define symbol removed.");

                break;
            }
        }
    }
}