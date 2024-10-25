using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;


namespace SOSXR.EditorTools
{
    [ExecuteAlways]
    public class MissingMonoBehaviourDetector : MonoBehaviour
    {
        #if UNITY_EDITOR
        [DidReloadScripts]
        #endif
        [MenuItem("SOSXR/Find GameObjects with Missing Scripts")] // This is not strictly necessary, but it's a nice touch. This script will reload anyway once the scripts are recompiled.
        [ContextMenu(nameof(FindGameObjectsWithMissingScripts))]
        private static void FindGameObjectsWithMissingScripts()
        {
            var allObjectsInScene = FindObjectsOfType<GameObject>();
            var count = 0;

            foreach (var gameObject in allObjectsInScene)
            {
                var allMonoBehavioursInScene = gameObject.GetComponents<MonoBehaviour>();

                foreach (var monoBehaviour in allMonoBehavioursInScene)
                {
                    if (monoBehaviour != null)
                    {
                        continue;
                    }

                    Debug.LogError(nameof(MissingMonoBehaviourDetector) + "Missing MonoBehaviour found on child of " + gameObject.transform.root.name); // Somehow the direct GO of the missing MB didn't want to print their name
                    count++;
                }
            }

            if (count > 0)
            {
                Debug.LogError(nameof(MissingMonoBehaviourDetector) + " Found " + count + " GameObjects with missing MonoBehaviours.");
            }
            // Debug.Log(nameof(MissingMonoBehaviourDetector) + " No GameObjects with missing MonoBehaviours found.");
        }
    }
}