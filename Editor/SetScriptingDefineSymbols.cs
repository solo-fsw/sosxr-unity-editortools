using UnityEditor;
using UnityEditor.Build;


namespace SOSXR.EditorSpice.EditorScripts
{
    // [InitializeOnLoad] // This will call the constructor of the class when Unity starts
    public class SetScriptingDefineSymbols
    {
        static SetScriptingDefineSymbols()
        {
            var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            var currentBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);

            if (PlayerSettings.GetScriptingDefineSymbols(currentBuildTarget).Contains(_defineSymbol))
            {
                // Debug.Log($"Scripting define symbols already set for {_defineSymbol}.");

                return;
            }

            PlayerSettings.SetScriptingDefineSymbols(currentBuildTarget, _defineSymbol);

            Debug.Log($"Scripting define symbols set for {_defineSymbol}.");
        }


        private const string _defineSymbol = "EDITORSPICE";
    }
}