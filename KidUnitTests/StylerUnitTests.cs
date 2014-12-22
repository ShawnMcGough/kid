using System.Collections.Generic;
using KidBrowserEngine.Css;
using KidBrowserEngine.Dom;
using KidBrowserEngine.Style;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KidUnitTests
{
    [TestClass]
    public class StylerParserUnitTests
    {
        [TestMethod]
        public void Styler_GetStyleTree_ReturnsValidTree()
        {
            // arrange
            const string css = @"
                                h1, h2, h3 { margin: auto; color: #cc0000; }
                                div.note { margin-bottom: 20px; padding: 10px; }
                                #answer { display: none; }";

            var root = new ElementNode(new ElementData("html", new Dictionary<string, string>()), new List<Node>());
            var stylesheet = new Stylesheet
            {
                Rules = new List<Rule>
                {
                    new Rule
                    {
                        Selectors = new List<Selector>
                        {
                            new SimpleSelector()
                            {
                                TagName = "html"
                            }
                        },
                        Declarations = new List<Declaration>
                        {
                            new Declaration
                            {
                                Name = "display", Value = new Value(){Keyword = "none"}
                            }
                        }
                    }
                }
            };

            // act
            var styletree = Styler.GetStyleTree(root, stylesheet);

            // assert
            Assert.IsTrue(styletree.SpecifiedValues.Count == 1);

        }
    }
}
