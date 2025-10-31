using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;


namespace MG.MDV
{
    public abstract class Content
    {
        public Rect Location;
        public Style Style;
        public GUIContent Payload;
        public string Link;


        public Content(GUIContent payload, Style style, string link)
        {
            Payload = payload;
            Style = style;
            Link = link;
        }


        public float Width => Location.width;
        public float Height => Location.height;
        public bool CanUpdate => false;


        public void CalcSize(Context context)
        {
            Location.size = context.CalcSize(Payload);
        }


        public void Draw(Context context)
        {
            if (string.IsNullOrEmpty(Link))
            {
                GUI.Label(Location, Payload, context.Apply(Style));

                return;
            }

            #if UNITY_EDITOR
            EditorGUIUtility.AddCursorRect(Location, MouseCursor.Link);
            #endif

            if (GUI.Button(Location, Payload, context.Apply(Style)))
            {
                if (Regex.IsMatch(Link, @"^\w+:", RegexOptions.Singleline))
                {
                    Application.OpenURL(Link);
                }
                else
                {
                    context.SelectPage(Link);
                }
            }
        }


        public virtual void Update(Context context)
        {
        }
    }
}