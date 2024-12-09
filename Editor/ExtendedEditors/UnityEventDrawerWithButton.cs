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

        float buttonWidth = 100;

        var buttonRect = new Rect(position.xMax - buttonWidth - 2, position.y + 1, buttonWidth, EditorGUIUtility.singleLineHeight);

        if (GUI.Button(buttonRect, ButtonLabel))
        {
            var targetObject = property.serializedObject.targetObject;
            var unityEvent = fieldInfo.GetValue(targetObject) as UnityEvent;
            unityEvent?.Invoke();
        }
    }
}


[CustomPropertyDrawer(typeof(UnityEvent<string>), true)]
public class UnityEventDrawerStringWithButton : UnityEventDrawer
{
    private const string ButtonLabel = "SOSXR: Invoke";


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        base.OnGUI(position, property, label);

        float buttonWidth = 100;
        float fieldWidth = 120;

        var fieldRect = new Rect(position.xMax - buttonWidth - fieldWidth - 2, position.y + 1, fieldWidth, EditorGUIUtility.singleLineHeight);
        var buttonRect = new Rect(position.xMax - buttonWidth - 2, position.y + 1, buttonWidth, EditorGUIUtility.singleLineHeight);

        var invokeValue = EditorGUI.TextField(fieldRect, "SOSXR");

        if (GUI.Button(buttonRect, ButtonLabel))
        {
            var targetObject = property.serializedObject.targetObject;
            var unityEvent = fieldInfo.GetValue(targetObject) as UnityEvent<string>;
            unityEvent?.Invoke(invokeValue);
        }
    }
}


[CustomPropertyDrawer(typeof(UnityEvent<int>), true)]
public class UnityEventDrawerIntWithButton : UnityEventDrawer
{
    private const string ButtonLabel = "SOSXR: Invoke";


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        base.OnGUI(position, property, label);

        float buttonWidth = 100;
        float fieldWidth = 120;

        var fieldRect = new Rect(position.xMax - buttonWidth - fieldWidth - 2, position.y + 1, fieldWidth, EditorGUIUtility.singleLineHeight);
        var buttonRect = new Rect(position.xMax - buttonWidth - 2, position.y + 1, buttonWidth, EditorGUIUtility.singleLineHeight);

        var invokeValue = EditorGUI.IntField(fieldRect, 42);

        if (GUI.Button(buttonRect, ButtonLabel))
        {
            var targetObject = property.serializedObject.targetObject;
            var unityEvent = fieldInfo.GetValue(targetObject) as UnityEvent<int>;
            unityEvent?.Invoke(invokeValue);
        }
    }
}


[CustomPropertyDrawer(typeof(UnityEvent<float>), true)]
public class UnityEventDrawerFloatWithButton : UnityEventDrawer
{
    private const string ButtonLabel = "SOSXR: Invoke";


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        base.OnGUI(position, property, label);

        float buttonWidth = 100;
        float fieldWidth = 120;

        var fieldRect = new Rect(position.xMax - buttonWidth - fieldWidth - 2, position.y + 1, fieldWidth, EditorGUIUtility.singleLineHeight);
        var buttonRect = new Rect(position.xMax - buttonWidth - 2, position.y + 1, buttonWidth, EditorGUIUtility.singleLineHeight);

        var invokeValue = EditorGUI.FloatField(fieldRect, 3.14f);

        if (GUI.Button(buttonRect, ButtonLabel))
        {
            var targetObject = property.serializedObject.targetObject;
            var unityEvent = fieldInfo.GetValue(targetObject) as UnityEvent<float>;
            unityEvent?.Invoke(invokeValue);
        }
    }
}