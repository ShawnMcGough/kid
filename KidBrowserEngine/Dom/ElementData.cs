using System;
using System.Collections.Generic;

namespace KidBrowserEngine.Dom
{
    public class ElementData
    {
        public string TagName { get; set; }
        public Dictionary<string, string> Attributes { get; set; }

        public ElementData(string name, Dictionary<string, string> attributes)
        {
            TagName = name;
            Attributes = attributes;
        }

        public string Id
        {
            get { return GetAttribute("id"); }
        }
        public HashSet<string> Classes
        {
            get
            {
                var classes = GetAttribute("class");
                return classes == null
                    ? new HashSet<string>()
                    : new HashSet<string>(classes.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries));
            }
        }
        public string GetAttribute(string key)
        {
            return Attributes.ContainsKey(key) ? Attributes[key] : null;
        }
    }
}
