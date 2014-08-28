namespace KidBrowserEngine.Dom
{
    public class TextNode : Node
    {
        public override NodeType Type { get { return NodeType.Text; } }
        public string Text { get; set; }
        public TextNode(string data)
        {
            Text = data;
        }
    }
}
