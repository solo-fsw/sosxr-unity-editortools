using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


namespace SOSXR.EditorSpice.EditorScripts
{
    public static class RightClickOptions
    {
        /// <summary>
        ///     Copy the current component to a new child gameObject and remove the original component.
        ///     From Warped Imagination: https://www.youtube.com/watch?v=qDoevls1wmI&t=467s
        /// </summary>
        /// <param name="command"></param>
        [MenuItem("CONTEXT/Component/Extract Component to Child", priority = 555)]
        public static void ExtractMenuOption(MenuCommand command)
        {
            var source = command.context as Component;

            var undoGroupIndex = Undo.GetCurrentGroup();
            Undo.IncrementCurrentGroup();

            var child = new GameObject(source.GetType().Name);
            child.transform.parent = source.transform;
            child.transform.localPosition = source.transform.localPosition;
            child.transform.localRotation = source.transform.localRotation;
            child.transform.localScale = source.transform.localScale;

            Undo.RegisterCreatedObjectUndo(child, $"Created child object: {child.name}");

            if (!ComponentUtility.CopyComponent(source) || !ComponentUtility.PasteComponentAsNew(child))
            {
                Debug.LogErrorFormat(source.gameObject, "SOSXR: Failed to extract component");

                Undo.CollapseUndoOperations(undoGroupIndex);
                Undo.PerformUndo();

                return;
            }

            Undo.DestroyObjectImmediate(source);
            Undo.CollapseUndoOperations(undoGroupIndex);
        }
    }
}