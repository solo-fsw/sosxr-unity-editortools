using System;
using System.Linq;
using UnityEngine;


namespace SOSXR.EditorTools
{
    /// <summary>
    ///     This class is used to validate all objects in the scene for null references.
    /// </summary>
    public class NullReferenceValidatorBootstrap : MonoBehaviour
    {
        private static readonly Type[] excludedTypes =
        {
            // typeof(NetworkObject)
        }; // Add types to exclude here


        private void Awake()
        {
            DontDestroyOnLoad(this);
        }


        private void Start()
        {
            AfterSceneLoad();
        }


        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AfterSceneLoad()
        {
            #if DEVELOPMENT_BUILD || UNITY_EDITOR
            var allMonoBehaviours = FindObjectsOfType<MonoBehaviour>();

            // Exclude specified types
            var objectsToValidate = allMonoBehaviours
                                    .Where(obj => !IsExcluded(obj))
                                    .ToArray();
            
            NullReferenceValidator.ValidateObjects(objectsToValidate);
            Debug.Log("SOSXR NullReferenceValidatorBootstrap: Null check complete");
            #endif
        }


        private static bool IsExcluded(MonoBehaviour obj)
        {
            var objType = obj.GetType();

            // Check if the object type matches or is a subclass of any excluded types
            return excludedTypes.Contains(objType);
        }
    }
}