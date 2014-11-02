using System.Collections.Generic;
using System.Linq;
using KidBrowserEngine.Css;
using KidBrowserEngine.Dom;

namespace KidBrowserEngine.Style
{
    public class StyledNode
    {
        public Node Node { get; set; }
        public Dictionary<string, Value> SpecifiedValues { get; set; }
        public List<StyledNode> Children { get; set; }

        public Value GetValue(string name)
        {
            return SpecifiedValues.Any(sv => sv.Key == name) ? SpecifiedValues.First(sv => sv.Key == name).Value : null;
        }

        public Value LookupValue(string name, string fallbackName, Value defaultValue)
        {
            return (GetValue(name) ?? GetValue(fallbackName)) ?? defaultValue;
        }

        public Display GetDisplay()
        {
            switch (GetValue("display").Keyword)
            {
                case "block":
                    return Display.Block;
                case "none":
                    return Display.None;
                default:
                    return Display.Inline;
            }
        }

        
    }
}
