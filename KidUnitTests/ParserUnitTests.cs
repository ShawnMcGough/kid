using KidBrowserEngine.Html;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KidUnitTests
{
    [TestClass]
    public class ParserUnitTests
    {
        [TestMethod]
        public void Parser_ParseValidHtml_ReturnsCorrectNodeCount()
        {
            // arrange
            const string html = @"
                                <html>
                                    <body>
                                        <h1>Title</h1>
                                        <div id='main' class='test'>
                                            <p>Hello <em>world</em>!</p>
                                        </div>
                                    </body>
                                </html>";
            // act
            var nodes = Parser.Parse(html);

            // assert
            Assert.IsTrue(nodes.Children.Count == 1);
            Assert.IsTrue(nodes.Children[0].Children.Count == 2);
            Assert.IsTrue(nodes.Children[0].Children[0].Children.Count == 1);

        }
    }
}
