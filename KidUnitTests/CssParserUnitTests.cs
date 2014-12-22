using System.Linq;
using KidBrowserEngine.Css;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KidUnitTests
{
    [TestClass]
    public class CssParserUnitTests
    {
        [TestMethod]
        public void Parser_ParseValidCss_ReturnsValidRuleCount()
        {
            // arrange
            const string css = @"
                                h1, h2, h3 { margin: auto; color: #cc0000; }
                                div.note { margin-bottom: 20px; padding: 10px; }
                                #answer { display: none; }";

            // act
            var stylesheet = Parser.Parse(css);

            // assert
            Assert.IsTrue(stylesheet.Rules.Count() == 3 );
            
        }
    }
}
