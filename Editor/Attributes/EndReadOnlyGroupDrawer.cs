using UnityEditor;
using UnityEngine;


namespace SOSXR.EditorTools.Attributes
{
    [CustomPropertyDrawer(typeof(EndReadOnlyGroupAttribute))]
    public class EndReadOnlyGroupDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            return 0;
        }


        public override void OnGUI(Rect position)
        {
            EditorGUI.EndDisabledGroup();
        }
    }
}