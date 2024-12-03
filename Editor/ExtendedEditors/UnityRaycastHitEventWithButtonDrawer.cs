using UnityEditor;
using UnityEngine;
using UnityEngine.Events;


namespace SOSXR.EditorTools
{
    [CustomPropertyDrawer(typeof(UnityEvent<RaycastHit>), true)]
    public class UnityRaycastHitEventWithButtonDrawer : UnityEventButtonDrawerBase
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);
            DrawButtonWithField<object>(position, property, label, $"SOSXR: {property.displayName}", null, value => { }, () => null, v => { });
        }
    }
}