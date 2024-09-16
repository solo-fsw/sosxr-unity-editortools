using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Callbacks;
#endif


[ExecuteAlways]
public class MissingMonoBehaviourDetector : MonoBehaviour
{
    #if UNITY_EDITOR
    [DidReloadScripts]
    #endif
    [MenuItem("SOSXR/Find GameObjects with Missing Scripts")]
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
        else
        {
            Debug.Log(nameof(MissingMonoBehaviourDetector) + " No GameObjects with missing MonoBehaviours found.");
        }
    }
}