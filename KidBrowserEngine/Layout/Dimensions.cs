namespace KidBrowserEngine.Layout
{
    public class Dimensions
    {
        public Rectangle Content { get; set; }
        public EdgeSizes Padding { get; set; }
        public EdgeSizes Border { get; set; }
        public EdgeSizes Margin { get; set; }

        public Dimensions()
        {
            Content = new Rectangle();
            Padding = new EdgeSizes();
            Border = new EdgeSizes();
            Margin = new EdgeSizes();
        }
        public Rectangle GetPaddingBox()
        {
            return Content.ExpandedBy(Padding);
        }
        public Rectangle GetBorderBox()
        {
            return GetPaddingBox().ExpandedBy(Border);
        }
        public Rectangle GetMarginBox()
        {
            return GetBorderBox().ExpandedBy(Border);
        }
    }
}
