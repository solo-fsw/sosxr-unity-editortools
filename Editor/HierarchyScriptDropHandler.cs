using System;
using UnityEditor;
using UnityEngine;

namespace SOSXR.EditorSpice.EditorScripts
{
    /// <summary>
    ///     From Warped Imagination: https://www.youtube.com/watch?v=FpOAcfULmTE
    /// </summary>
    [InitializeOnLoad]
    public class HierarchyScriptDropHandler
    {
        static HierarchyScriptDropHandler() => DragAndDrop.AddDropHandlerV2(OnScriptHierarchyDrop);

        private static DragAndDropVisualMode OnScriptHierarchyDrop(EntityId dropTargetEntityId, HierarchyDropFlags dropMode, Transform parentForDraggedObjects, bool perform)
        {
            var monoScript = GetScriptBeingDragged();

            if (monoScript != null)
            {
                if (perform)
                {
                    var gameObject = CreateAndRename(monoScript.name);
                    _ = gameObject.AddComponent(monoScript.GetClass());
                }

                return DragAndDropVisualMode.Copy;
            }

            return DragAndDropVisualMode.None;
        }

        public static GameObject CreateAndRename(string startingName)
        {
            GameObject gameObject = new(startingName);

            if (Selection.activeGameObject != null)
            {
                gameObject.transform.parent = Selection.activeGameObject.transform;
            }
            else
            {
                gameObject.transform.SetParent(null); // Or another default parent
            }

            gameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            Selection.activeGameObject = gameObject;

            Undo.RegisterCreatedObjectUndo(gameObject, "Created GameObject");

            EditorApplication.delayCall += static () =>
            {
                Type sceneHierarchyType = Type.GetType("UnityEditor.SceneHierarchyWindow,UnityEditor");
                _ = EditorWindow.GetWindow(sceneHierarchyType).SendEvent(EditorGUIUtility.CommandEvent("Rename"));
            };

            return gameObject;
        }

        private static MonoScript GetScriptBeingDragged()
        {
            foreach (var objectReference in DragAndDrop.objectReferences)
            {
                if (objectReference is MonoScript monoScript)
                {
                    var scriptType = monoScript.GetClass();

                    if (scriptType != null && scriptType.IsSubclassOf(typeof(MonoBehaviour)))
                    {
                        return monoScript;
                    }
                }
            }

            return null;
        }
    }
}
