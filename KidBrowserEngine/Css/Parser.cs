using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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

                throw new FormatException(string.Format("Unexpected character '{0}' in selector list.", c));

            }

            // Return selectors with highest specificity first, for use in matching.
            return selectors.OrderByDescending(s => s.Specificity()).ToList();
        }

        // Parse one simple selector type#id.class1.class2.class3
        private SimpleSelector ParseSimpleSelector()
        {
            var selector = new SimpleSelector();

            while (!EndOfInput())
            {
                var c = NextChar();

                if (c == '#')
                {
                    ConsumeChar();
                    selector.Id = ParseIdentifier();
                }
                else if (c == '.')
                {
                    ConsumeChar();
                    selector.Class.Add(ParseIdentifier());
                }
                else if (c == '*')
                    ConsumeChar();

                else if (IsValidIdentifierChar(c))
                    selector.TagName = ParseIdentifier();
                else
                    break;

            }

            return selector;
        }

        private string ParseIdentifier()
        {
            return ConsumeWhile(c => IsValidIdentifierChar(c));
        }

        private static bool IsValidIdentifierChar(char c)
        {
            if (char.IsLetterOrDigit(c))
                return true;
            return c == '-' || c == '_';
        }

        private List<Declaration> ParseDeclarations()
        {
            var declarations = new List<Declaration>();
            Contract.Assert(ConsumeChar() == '{');
            while (true)
            {
                ConsumeWhiteSpace();
                if (NextChar() == '}')
                {
                    ConsumeChar();
                    break;
                }
                declarations.Add(ParseDeclaration());

            }
            return declarations;

        }

        // Parse one <property>: <value>; declaration.
        private Declaration ParseDeclaration()
        {
            var declaration = new Declaration { Name = ParseIdentifier() };
            ConsumeWhiteSpace();
            Contract.Assert(ConsumeChar() == ':');
            ConsumeWhiteSpace();
            declaration.Value = ParseValue();
            ConsumeWhiteSpace();
            Contract.Assert(ConsumeChar() == ';');

            return declaration;
        }

        private Value ParseValue()
        {
            var c = NextChar();

            var value = new Value();
            if (Char.IsDigit(c))
                value.Length = ParseLength();
            else if (c == '#')
                value.Color = ParseColor();
            else
                value.Keyword = ParseIdentifier();

            return value;
        }

        private byte[] ParseColor()
        {
            Contract.Assert(ConsumeChar() == '#');
            var hex = ConsumeWhile(c => Char.IsLetterOrDigit(c));
            return Enumerable.Range(0, hex.Length)
                 .Where(x => x % 2 == 0)
                 .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                 .ToArray();
        }

        private KeyValuePair<float, Unit> ParseLength()
        {
            var len = float.Parse(ConsumeWhile(c => Char.IsDigit(c) || c == '.'));
            
            var unit = ParseIdentifier().ToLowerInvariant();
            if (unit != "px")
                throw new FormatException("Unrecognized unit.");
            
            return new KeyValuePair<float, Unit>(len, Unit.Px);
            
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
