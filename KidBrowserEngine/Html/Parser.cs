using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using KidBrowserEngine.Dom;

namespace KidBrowserEngine.Html
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
        public static Node Parse(string html)
        {
            var nodes = new Parser(html).ParseNodes();

            return nodes.Count == 1
                ? nodes[0]
                : new ElementNode(new ElementData("html", new Dictionary<string, string>()), nodes);
        }

        private Node ParseNode()
        {
            var isElement = NextChar().Equals('<');
            return isElement ? ParseElement() : ParseText();
        }

        private Node ParseText()
        {
            return new TextNode(ConsumeWhile(c => !c.Equals('<')));
        }

        private Node ParseElement()
        {
            // open tag
            Contract.Assert(ConsumeChar() == '<');
            var tagName = ParseTagName();
            var attributes = ParseAttributes();

            Contract.Assert(ConsumeChar() == '>');

            // content
            var children = ParseNodes();

            // close tag
            Contract.Assert(ConsumeChar() == '<');
            Contract.Assert(ConsumeChar() == '/');
            Contract.Assert(ParseTagName() == tagName);
            Contract.Assert(ConsumeChar() == '>');

            return new ElementNode(new ElementData(tagName, attributes), children);
        }

        private List<Node> ParseNodes()
        {
            var nodes = new List<Node>();
            while (true)
            {
                ConsumeWhiteSpace();
                if (EndOfInput() || StartsWith("</"))
                    break;

                nodes.Add(ParseNode());
            }
            return nodes;
        }

        private Dictionary<string, string> ParseAttributes()
        {
            var attributes = new Dictionary<string, string>();
            while (true)
            {
                ConsumeWhiteSpace();
                if (NextChar() == '>')
                    break;

                var attribute = ParseSingleAttribute();
                attributes.Add(attribute.Key, attribute.Value);
            }
            return attributes;
        }

        private KeyValuePair<string, string> ParseSingleAttribute()
        {
            var name = ParseTagName();
            Contract.Assert(ConsumeChar() == '=');
            var value = ParseAttributeValue();

            return new KeyValuePair<string, string>(name, value);
        }

        private string ParseAttributeValue()
        {
            var openQuote = ConsumeChar();
            Contract.Assert(openQuote == '"' || openQuote == '\'');
            var value = ConsumeWhile(c => !c.Equals(openQuote));
            Contract.Assert(ConsumeChar() == openQuote);
            return value;
        }

        private string ParseTagName()
        {
            return ConsumeWhile(c => char.IsLetterOrDigit(c));
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
