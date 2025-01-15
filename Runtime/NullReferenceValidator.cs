using System.Reflection;
using UnityEngine;


namespace SOSXR.EditorTools
{
    /// <summary>
    ///     This runs in conjunction with the NullCheckBootstrap.cs script to check for null references in the editor.
    /// </summary>
    public static class NullReferenceValidator
    {
        public static void ValidateObject(MonoBehaviour component)
        {
            var fields = component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var componentName = component.GetType().Name;

            foreach (var field in fields)
            {
                if (!field.IsPublic && field.GetCustomAttribute<SerializeField>() == null)
                {
                    continue;
                }

                var value = field.GetValue(component);

                if (value != null)
                {
                    continue;
                }

                Debug.LogErrorFormat(component, $"SOSXR: Field '{field.Name}' is null in script '{componentName}' on '{component.gameObject.name}'");
            }
        }


        public static void ValidateObjects(MonoBehaviour[] componentsToTest)
        {
            foreach (var comp in componentsToTest)
            {
                ValidateObject(comp);
            }
        }
    }
}