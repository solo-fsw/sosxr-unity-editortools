#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;


namespace SOSXR.EditorTools
{
    /// <summary>
    ///     This will try to validate all public fields of all MonoBehaviours before entering play mode.
    ///     It can also be invoked manually from the menu.
    /// </summary>
    // [InitializeOnLoad]
    public static class NullReferenceValidatorOnPlay
    {
        static NullReferenceValidatorOnPlay()
        {
         //   EditorApplication.playModeStateChanged += ValidateBeforePlay;
        }


        private static void ValidateBeforePlay(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingEditMode)
            {
                return;
            }

            ValidateAllReferences();
        }


        [MenuItem("SOSXR/Null Reference Validator")]
        public static void ValidateAllReferences()
        {
            var allComponents = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (var component in allComponents)
            {
                var fields = component.GetType().GetFields();

                foreach (var field in fields)
                {
                    if (!field.IsPublic || field.GetValue(component) != null)
                    {
                        continue;
                    }

                    Debug.LogError($"SOSXR: {field.Name} is null on {component.name}", component);
                }
            }
        }
    }
}
#endif