using System.Collections.Generic;

namespace KidBrowserEngine.Css
{
    internal class Value
    {
        public string Keyword;
        public byte[] Color = { 0, 0, 0, 0 };
        public KeyValuePair<float, Unit> Length;
    }
}
