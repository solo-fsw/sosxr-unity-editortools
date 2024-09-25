using System.Reflection;
using SOSXR.EditorTools;
using UnityEditor;
using UnityEngine;


/// <summary>
///     From Warped Imagination: https://www.youtube.com/watch?v=M3tLr3EYIiE&t=0s
///     and
///     https://www.youtube.com/watch?v=DztxQiMr4EU&t=784s
/// </summary>
[CustomEditor(typeof(MonoBehaviour), true, isFallback = false)]
public class BaseEditor : Editor
{
    private readonly string _logoTextureName = "SOSXR_Logo";
    private static Texture2D _Logo = null;
    
    private readonly string[] _desiredNamespace = {"SOSXR"};
    private bool isDesiredNamespace;
    
    private DescriptionAttribute _descriptionAttribute = null;


    protected virtual void OnEnable()
    {
        var targetNamespace = target.GetType().Namespace;

        if (targetNamespace != null)
        {
            foreach (var nm in _desiredNamespace)
            {
                if (targetNamespace.Contains(nm))
                {
                    isDesiredNamespace = true;

                    break;
                }
            }
        }

        _descriptionAttribute ??= GetComponentAttribute(target);
    }


    public override void OnInspectorGUI()
    {
        DescriptionGUI(_descriptionAttribute);
        
        base.OnInspectorGUI();

        if (isDesiredNamespace)
        {
            LogoGUI();
        }
    }


    private void LogoGUI()
    {
        GetLogo();

        if (_Logo == null)
        {
            return;
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label(_Logo, GUILayout.Width(25), GUILayout.Height(25));
        EditorGUILayout.EndHorizontal();
    }


    private void GetLogo()
    {
        if (_Logo != null)
        {
            return;
        }

        _Logo = Resources.Load<Texture2D>(_logoTextureName);
    }


    private static void DescriptionGUI(DescriptionAttribute descriptionAttribute)
    {
        if (descriptionAttribute != null && !string.IsNullOrEmpty(descriptionAttribute.Description))
        {
            EditorGUILayout.Space(5);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            var inspectorWidth = Screen.width;
            GUILayout.Box(descriptionAttribute.Description, GUILayout.Width(inspectorWidth * 0.9f));

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);
            
            var lineHeight = 1;
            var lineRect = EditorGUILayout.GetControlRect(false, lineHeight);
            lineRect.height = 1;
            EditorGUI.DrawRect(lineRect, Color.grey);
            
            EditorGUILayout.Space(5);
        }
    }


    public static DescriptionAttribute GetComponentAttribute(Object obj)
    {
        //return obj.GetType().GetCustomAttribute<ComponentAttribute>() ?? new ComponentAttribute(obj.GetType().Name);
        return obj.GetType().GetCustomAttribute<DescriptionAttribute>();
    }
}