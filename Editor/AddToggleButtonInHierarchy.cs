using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


/// <summary>
///     From Warped Imagination @ https://youtu.be/Rdg0PQS5OiU?si=sRTkgurRIXPfq_rv
/// </summary>
[InitializeOnLoad]
public static class AddToggleButtonInHierarchy
{
    static AddToggleButtonInHierarchy()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
    }


    private static void HandleHierarchyWindowItemOnGUI(int instanceid, Rect selectionrect)
    {
        var gameObject = EditorUtility.InstanceIDToObject(instanceid) as GameObject;

        if (gameObject == null)
        {
            return;
        }

        var rect = new Rect(selectionrect);
        rect.x -= 27f; // 27f is given by Warped Imagination
        rect.width = 13f; // 13f is given by Warped Imagination 

        var active = EditorGUI.Toggle(rect, gameObject.activeSelf);

        if (active != gameObject.activeSelf)
        {
            Undo.RecordObject(gameObject, "Active state change");
            gameObject.SetActive(active);
            EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
    }
}