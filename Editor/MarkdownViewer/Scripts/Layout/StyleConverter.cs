////////////////////////////////////////////////////////////////////////////////

using UnityEngine;


namespace MG.MDV
{
    public class StyleConverter
    {
        private Style mCurrentStyle = Style.Default;
        private readonly GUIStyle[] mWorking;
        private readonly GUIStyle[] mReference;

        private readonly Color linkColor = new(0.41f, 0.71f, 1.0f, 1.0f);


        public StyleConverter(GUISkin skin)
        {
            mReference = new GUIStyle[CustomStyles.Length];
            mWorking = new GUIStyle[CustomStyles.Length];

            for (var i = 0; i < CustomStyles.Length; i++)
            {
                mReference[i] = skin.GetStyle(CustomStyles[i]);
                mWorking[i] = new GUIStyle(mReference[i]);
            }
        }


        private const int FixedBlock = 7;
        private const int Variable = 8;
        private const int FixedInline = 12;

        private static readonly string[] CustomStyles =
        {
            "variable",
            "h1",
            "h2",
            "h3",
            "h4",
            "h5",
            "h6",
            "fixed_block",
            "variable",
            "variable_bold",
            "variable_italic",
            "variable_bolditalic",
            "fixed_inline",
            "fixed_inline_bold",
            "fixed_inline_italic",
            "fixed_inline_bolditalic"
        };


        //------------------------------------------------------------------------------


        public GUIStyle Apply(Style src)
        {
            if (src.Block)
            {
                return mWorking[FixedBlock];
            }

            var style = mWorking[src.Size];

            if (mCurrentStyle != src)
            {
                var font = (src.Fixed ? FixedInline : Variable) + (src.Bold ? 1 : 0) + (src.Italic ? 2 : 0);

                style.font = mReference[font].font;
                style.fontStyle = mReference[font].fontStyle;
                style.normal.textColor = src.Link ? linkColor : mReference[font].normal.textColor;

                mCurrentStyle = src;
            }

            return style;
        }
    }
}