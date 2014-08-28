using System.Collections.Generic;

namespace KidBrowserEngine.Dom
{
    public abstract class Node
    {
        public List<Node> Children { get; set; }
        public abstract NodeType Type { get; }

        protected Node()
        {
            Children = new List<Node>();
        }
    }
}
