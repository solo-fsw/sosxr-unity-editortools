using UnityEditor;
using UnityEngine;


namespace SOSXR.EditorTools
{
    public class CreateCustomGameObject : Editor
    {
        [MenuItem("GameObject/Create Empty Better _%Y", false, 0)]
        private static void CreateCustomGameObjectFunction()
        {
            var parent = CreateGameObject("CustomGameObject", "SOSXR_Logo");

            CreateGameObject("Model", "Model_icon", parent);

            CreateGameObject("Audio", "Audio_icon", parent);

            CreateGameObject("Logic", "Logic_icon", parent);
        }


        private static GameObject CreateGameObject(string goName, string iconName, GameObject parentTo = null)
        {
            var go = new GameObject(goName);
            go.transform.parent = parentTo?.transform;
            go.transform.position = Vector3.zero;
            go.transform.rotation = Quaternion.identity;
            
            var icon = Resources.Load<Texture2D>(iconName);
            EditorGUIUtility.SetIconForObject(go, icon);

            return go;
        }
    }
}