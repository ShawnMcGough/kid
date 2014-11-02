using System.Collections.Generic;
using System.Linq;

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
    

}
