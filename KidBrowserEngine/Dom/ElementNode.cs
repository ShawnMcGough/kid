using System.Collections.Generic;

namespace KidBrowserEngine.Dom
{
    public class ElementNode : Node
    {
        public override NodeType Type { get { return NodeType.Element; } }
        public ElementData Data { get; set; }

        public ElementNode(ElementData data, List<Node> children)
        {
            Children = children;
            Data = data;
        }
    }
    
    public class ElementData
    {
        public string TagName { get; set; }
        public Dictionary<string, string> Attributes { get; set; }

        public ElementData(string name, Dictionary<string, string> attributes)
        {
            TagName = name;
            Attributes = attributes;
        }
    }
}
