using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KidBrowserEngine.Css
{
    public class Parser
    {
        private int Position { get; set; }
        private string Input { get; set; }

        public Parser(string html, int position = 0)
        {
            Position = position;
            Input = html;
        }

        // Parse a list of rule sets, separated by optional whitespace.
        private List<Rule> ParseRules()
        {
            var rules = new List<Rule>();
            while (true)
            {
                ConsumeWhiteSpace();
                if (EndOfInput())
                    break;

                rules.Add(ParseRule());
            }
            return rules;
        }

        // Parse a rule set
        private Rule ParseRule()
        {
            return new Rule
            {
                Selectors = ParseSelectors(),
                Declarations = ParseDeclarations()
            };
        }

        // Parse a comma-separated list of selectors.
        private List<Selector> ParseSelectors()
        {
            var selectors = new List<Selector>();
            while (true)
            {
                selectors.Add(new SimpleSelector());
                ConsumeWhiteSpace();
                var c = NextChar();

                if (c == ',')
                {
                    ConsumeChar();
                    ConsumeWhiteSpace();
                    break;
                }

                if (c == '{')
                    break;

                throw new Exception(string.Format("Unexpected character '{0}' in selector list.", c));

            }

            // Return selectors with highest specificity first, for use in matching.
            return selectors.OrderByDescending(s => s.Specificity()).ToList();
        }

        // Parse one simple selector, e.g.: `type#id.class1.class2.class3`
        private SimpleSelector ParseSimpleSelector()
        {
            var selector = new SimpleSelector();

            while (!EndOfInput())
            {


            }

            return selector;
        }

        private void ConsumeWhiteSpace()
        {
            ConsumeWhile(c => char.IsWhiteSpace(c));
        }
        private string ConsumeWhile(Expression<Func<char, bool>> predicate)
        {
            var test = predicate.Compile();
            var result = new StringBuilder();
            while (!EndOfInput() && (test(NextChar())))
                result.Append(ConsumeChar());

            return result.ToString();
        }
        private char ConsumeChar()
        {
            return Input.ToCharArray().Skip(Position++).Take(1).First();
        }
        private char NextChar()
        {
            return Input.ToCharArray().Skip(Position).Take(1).First();
        }
        private bool StartsWith(string value)
        {
            return Input.Substring(Position).StartsWith(value);
        }
        private bool EndOfInput()
        {
            return Position >= Input.Length;
        }
    }
}
