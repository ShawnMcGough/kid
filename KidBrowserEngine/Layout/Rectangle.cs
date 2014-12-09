namespace KidBrowserEngine.Layout
{
    public class Rectangle
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public Rectangle ExpandedBy(EdgeSizes edgeSizes)
        {
            return new Rectangle
            {
                X = X - edgeSizes.Left,
                Y = Y - edgeSizes.Top,
                Width = Width + edgeSizes.Left + edgeSizes.Right,
                Height = Height + edgeSizes.Top + edgeSizes.Bottom
            };
        }
    }
}
