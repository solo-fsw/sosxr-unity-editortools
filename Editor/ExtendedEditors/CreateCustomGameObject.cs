using UnityEditor;
using UnityEngine;


namespace SOSXR.EditorTools
{
    public class CreateCustomGameObject : Editor
    {
        [MenuItem("GameObject/Create Empty _%Y", false, 0)]
        private static void CreateCustomGameObjectFunction()
        {
            var parent = new GameObject("CustomGameObject");
            // parent.icon = EditorGUIUtility.IconContent("GameObject Icon").image as Texture2D;
            var modelGameObject = new GameObject("Model");
            modelGameObject.transform.parent = parent.transform;
            var audioGameObject = new GameObject("Audio");
            audioGameObject.transform.parent = parent.transform;
            var logicGameObject = new GameObject("Logic");
            logicGameObject.transform.parent = parent.transform;
        }
    }
}