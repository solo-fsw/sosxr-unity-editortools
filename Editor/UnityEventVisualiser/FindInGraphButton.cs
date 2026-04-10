using UnityEditor;

namespace EventVisualizer.Base
{
    public static class FindInGraphButton
    {
        [MenuItem("GameObject/EventGraph/Find in current graph", false, 0)]
        private static void FindEvents()
        {
            var window = EditorWindow.GetWindow<EventsGraphWindow>();

            window?.OverrideSelection(Selection.activeEntityId);
        }

        [MenuItem("GameObject/EventGraph/Graph just this", false, 0)]
        private static void GraphSelection()
        {
            var window = EditorWindow.GetWindow<EventsGraphWindow>();
            window.RebuildGraph(new[] { Selection.activeGameObject }, false);
        }

        [MenuItem("GameObject/EventGraph/Graph this hierarchy", false, 0)]
        private static void GraphSelectionHierarchy()
        {
            var window = EditorWindow.GetWindow<EventsGraphWindow>();
            window.RebuildGraph(new[] { Selection.activeGameObject }, true);
        }
    }
}
