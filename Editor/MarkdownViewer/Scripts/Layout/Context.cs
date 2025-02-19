using UnityEngine;


namespace MG.MDV
{
    public class Context
    {
        private readonly StyleConverter mStyleConverter;
        private GUIStyle mStyleGUI;
        private readonly HandlerImages mImages;
        private readonly HandlerNavigate mNagivate;


        public Context(GUISkin skin, HandlerImages images, HandlerNavigate navigate)
        {
            mStyleConverter = new StyleConverter(skin);
            mImages = images;
            mNagivate = navigate;

            Apply(Style.Default);
        }


        public float LineHeight => mStyleGUI.lineHeight;
        public float MinWidth => LineHeight * 2.0f;
        public float IndentSize => LineHeight * 1.5f;


        public void SelectPage(string path)
        {
            mNagivate.SelectPage(path);
        }


        public Texture FetchImage(string url)
        {
            return mImages.FetchImage(url);
        }


        public void Reset()
        {
            Apply(Style.Default);
        }


        public GUIStyle Apply(Style style)
        {
            mStyleGUI = mStyleConverter.Apply(style);

            return mStyleGUI;
        }


        public Vector2 CalcSize(GUIContent content)
        {
            return mStyleGUI.CalcSize(content);
        }
    }
}