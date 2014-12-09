using System;
using KidBrowserEngine.Style;

namespace KidBrowserEngine.Layout
{
    public class Layouter
    {

        /// Transform a style tree into a layout tree.
        public LayoutBox LayoutTree(StyledNode styledNode, Dimensions containingBlock)
        {
            // The layout algorithm expects the container height to start at 0.
            // TODO: Save the initial containing block height, for calculating percent heights.
            containingBlock.Content.Height = 0f;

            var rootBox = BuildLayoutTree(styledNode);
            rootBox.Layout(containingBlock);
            return rootBox;
        }

        /// Build the tree of LayoutBoxes, but don't perform any layout calculations yet.
        public LayoutBox BuildLayoutTree(StyledNode styledNode)
        {
            // Create the root box.
            var root = new LayoutBox {StyledNode = styledNode};
            switch (styledNode.GetDisplay())
            {
                case Display.Inline:
                    root.BoxType = BoxTypes.Inline;
                    break;
                case Display.Block:
                    root.BoxType = BoxTypes.Block;
                    break;
                case Display.None:
                    throw new Exception("Root node has display: none.");
                default:
                    throw new ArgumentOutOfRangeException();
            }

            

            // Create the descendant boxes.
            foreach (var child in styledNode.Children)
            {
                switch (child.GetDisplay())
                {
                    case Display.Inline:
                        root.GetInlineContainer().Children.Add(BuildLayoutTree(child));
                        break;
                    case Display.Block:
                        root.Children.Add(BuildLayoutTree(child));
                        break;
                    case Display.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        
            return root;
        }

    }
}
