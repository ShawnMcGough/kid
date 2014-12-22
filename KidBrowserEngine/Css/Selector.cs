using System.Collections.Generic;

namespace KidBrowserEngine.Css
{
    public abstract class Selector
    {
        public string TagName;
        public string Id;
        public List<string> Class;
        public abstract SelectorType Type { get; }

        protected Selector()
        {
            Class = new List<string>();
        }

        public int Specificity()
        {
            return
                (
                    (string.IsNullOrWhiteSpace(Id) ? 0 : 100) +
                    (Class.Count * 10) +
                    (string.IsNullOrWhiteSpace(TagName) ? 0 : 1)
                );

        }
    }
}
