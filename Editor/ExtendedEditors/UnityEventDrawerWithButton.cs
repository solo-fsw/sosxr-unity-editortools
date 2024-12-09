using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;


[CustomPropertyDrawer(typeof(UnityEvent), true)]
public class UnityEventDrawerWithButton : UnityEventDrawer
{
    private const string ButtonLabel = "SOSXR: Invoke";
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        base.OnGUI(position, property, label);
        
        float buttonWidth = 120;
        var buttonRect = new Rect(position.xMax - buttonWidth - 2, position.y + 1, buttonWidth, EditorGUIUtility.singleLineHeight);
        
        if (GUI.Button(buttonRect, ButtonLabel))
        {
            var targetObject = property.serializedObject.targetObject;
            var unityEvent = fieldInfo.GetValue(targetObject) as UnityEvent;
            unityEvent?.Invoke();
        }
    }
}