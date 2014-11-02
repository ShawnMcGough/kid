using System.Collections.Generic;
using System.Linq;
using KidBrowserEngine.Css;
using KidBrowserEngine.Dom;

namespace KidBrowserEngine.Style
{
    public class Styler
    {
        //public Dictionary<string, Value> PropertyMap = new Dictionary<string, Value>();

        public StyledNode GetStyleTree(Node root, Stylesheet stylesheet)
        {
            var styledNode = new StyledNode
            {
                Node = root,
                SpecifiedValues =
                    root.Type == NodeType.Text
                        ? new Dictionary<string, Value>()
                        : GetSpecifiedValues((ElementNode)root, stylesheet)
            };

            foreach (var child in root.Children)
                styledNode.Children.Add(GetStyleTree(child, stylesheet));

            return styledNode;
        }

        private Dictionary<string, Value> GetSpecifiedValues(ElementNode elem, Stylesheet stylesheet)
        {
            var rules = GetMatchingRules(elem, stylesheet);

            // Go through the rules from lowest to highest specificity.
            return
                rules.OrderBy(mr => mr.Specificity)
                    .SelectMany(mr => mr.Rule.Declarations)
                    .ToDictionary(declaration => declaration.Name, declaration => declaration.Value);
        }

        private IEnumerable<MatchedRule> GetMatchingRules(ElementNode elem, Stylesheet stylesheet)
        {
            return stylesheet.Rules.Select(rule => GetMatchRule(elem, rule)).ToList();
        }

        private MatchedRule GetMatchRule(ElementNode elem, Rule rule)
        {
            var match = rule.Selectors.FirstOrDefault(s => IsMatch(elem, s));
            return new MatchedRule
            {
                Specificity = match.Specificity(),
                Rule = rule
            };
        }

        private static bool IsMatch(ElementNode elem, Selector selector)
        {
            return IsMatchSimpleSelector(elem, selector);
        }

        private static bool IsMatchSimpleSelector(ElementNode elem, Selector selector)
        {
            // Check type selector
            if (selector.TagName != elem.Data.TagName)
                return false;

            // Check ID selector
            if (selector.Id != elem.Data.Id)
                return false;

            // Check class selectors
            if (!selector.Class.Any(c => elem.Data.Classes.Contains(c)))
                return false;

            // We didn't find any non-matching selector components.
            return true;

        }
    }
}
