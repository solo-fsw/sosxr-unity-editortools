using UnityEditor;
using UnityEditor.Compilation;

/// <summary>
///     From: https://github.com/adammyhre/Unity-Utils
/// </summary>
public static class CompileProject
{
    [MenuItem("File/Compile _F5")]
    private static void Compile()
    {
        CompilationPipeline.RequestScriptCompilation(
            RequestScriptCompilationOptions.CleanBuildCache
        );
    }

    /// <summary>
    ///     Refresh the asset database — equivalent to switching focus back to Unity with Auto Refresh disabled.
    ///     Detects changed scripts on disk and recompiles only what changed.
    /// </summary>
    [MenuItem("File/Refresh _F6")]
    private static void Refresh()
    {
        AssetDatabase.Refresh();
    }
}

