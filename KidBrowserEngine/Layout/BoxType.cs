using KidBrowserEngine.Style;

namespace KidBrowserEngine.Layout
{
	public abstract class BoxType
	{

		public abstract BoxTypes Type { get; }
		public StyledNode Node { get; set; }

		protected BoxType()
        {
			Node = new StyledNode();
        }
	}
}
