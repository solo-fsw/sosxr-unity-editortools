////////////////////////////////////////////////////////////////////////////////

namespace MG.MDV
{
    public class Layout
    {
        private readonly Context mContext;
        private readonly BlockContainer mDocument;


        public Layout(Context context, BlockContainer doc)
        {
            mContext = context;
            mDocument = doc;
        }


        public float Height => mDocument.Rect.height;


        public Block Find(string id)
        {
            return mDocument.Find(id);
        }


        public void Arrange(float maxWidth)
        {
            mContext.Reset();
            mDocument.Arrange(mContext, MarkdownViewer.Margin, maxWidth);
        }


        public void Draw()
        {
            mContext.Reset();
            mDocument.Draw(mContext);
        }
    }
}