using System;
using System.Linq;
using SOSXR.EditorTools.Helpers;
using UnityEditor;
using UnityEngine;


namespace SOSXR.EditorTools
{
    /// <summary>
    ///     This needs to be in an Editor folder
    ///     From Warped Imagination: https://www.youtube.com/watch?v=EFh7tniBqkk&t=33s
    /// </summary>
    [InitializeOnLoad] // This will call the constructor of the class when Unity starts
    public static class HierarchyIconDisplay
    {
        static HierarchyIconDisplay()
        {
            //if (PackageIsInstalled.PackageInstalled("com.browar.editor-toolbox"))
            //{
            //    return;
            //}

            // Only add the toggle button if the EditorPackage is not installed
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
            EditorApplication.update += OnEditorUpdate;
        }
        

        private static readonly bool IncludeScripts = true;
        private static readonly bool KeepIconsForPrefabs = false;

        private static bool _hierarchyHasFocus = false;
        private static EditorWindow _hierarchyEditorWindow;


        /// <summary>
        ///     Runs every time the editor updates
        /// </summary>
        private static void OnEditorUpdate()
        {
            if (_hierarchyEditorWindow == null)
            {
                _hierarchyEditorWindow = EditorWindow.GetWindow(Type.GetType("UnityEditor.SceneHierarchyWindow,UnityEditor"));
            }

            _hierarchyHasFocus = EditorWindow.focusedWindow != null && EditorWindow.focusedWindow == _hierarchyEditorWindow;
        }


        private static void OnHierarchyWindowItemOnGUI(int instanceid, Rect selectionrect)
        {
            var gameObject = EditorUtility.InstanceIDToObject(instanceid) as GameObject;

            if (gameObject == null)
            {
                return;
            }

            if (PrefabUtility.GetCorrespondingObjectFromSource(gameObject) != null && KeepIconsForPrefabs) // Keep icons for prefabs
            {
                return;
            }

            var components = gameObject.GetComponents<Component>();

            if (components == null || components.Length == 0)
            {
                return;
            }

            var component = components.Length > 1 ? components[1] : components[0]; // components[0] is the Transform. This does mean that the 'most interesting / important' component should be the first one that is not the Transform component

            if (component == null)
            {
                return;
            }

            var type = component.GetType();

            GUIContent content;

            if (!IncludeScripts)
            {
                content = EditorGUIUtility.ObjectContent(null, type); // Gimme the icon for the particular content. 
            }
            else
            {
                content = EditorGUIUtility.ObjectContent(component, type); // Gimme the icon for the particular content.
            }

            content.text = null;
            content.tooltip = type.Name; // Set the tooltip to the name of the type

            if (content.image == null)
            {
                // maybe log me some stuff
                return;
            }

            var isSelected = Selection.instanceIDs.Contains(instanceid);
            var isHovering = selectionrect.Contains(Event.current.mousePosition);


            var color = UnityEditorBackgroundColor.Get(isSelected, isHovering, _hierarchyHasFocus);
            var backgroundRect = selectionrect;
            backgroundRect.width = 18.5f; // 18.5f is given by Warped Imagination
            EditorGUI.DrawRect(backgroundRect, color);

            EditorGUI.LabelField(selectionrect, content);
        }
    }
}