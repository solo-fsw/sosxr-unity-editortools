using UnityEditor;
using UnityEngine;


namespace SOSXR.EditorTools.Attributes
{
    [CustomPropertyDrawer(typeof(BeginReadOnlyGroupAttribute))]
    public class BeginReadOnlyGroupDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            return 0;
        }


        public override void OnGUI(Rect position)
        {
            EditorGUI.BeginDisabledGroup(true);
        }
    }
}