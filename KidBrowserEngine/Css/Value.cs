using System.Collections.Generic;

namespace KidBrowserEngine.Css
{
    public class Value
    {
        public string Keyword { get; set; }
        public byte[] Color = { 0, 0, 0, 0 };
        public KeyValuePair<float, Unit> Length { get; set; }

        public float ToPx()
        {
            return Length.Value == Unit.Px ? Length.Key : 0;
        }
    }
}
