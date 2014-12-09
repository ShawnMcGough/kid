using System;
using System.Collections.Generic;
using System.Linq;
using KidBrowserEngine.Css;
using KidBrowserEngine.Style;

namespace KidBrowserEngine.Layout
{
	public class LayoutBox
	{
		public Dimensions Dimensions { get; set; }
		public BoxTypes BoxType { get; set; }
		public StyledNode StyledNode { get; set; }
		public List<LayoutBox> Children { get; set; }


		public LayoutBox()
		{
			BoxType = BoxTypes.AnonymousBlock;
			Dimensions = new Dimensions();
			Children = new List<LayoutBox>();
		}

		private StyledNode GetStyleNode()
		{
			return new StyledNode();
			//switch (BoxType.Type)
			//{
			//    case BoxTypes.AnonymousBlock:
			//        throw new ArgumentOutOfRangeException("BoxType.Type", "Anonymous block box has no style node.");
			//    case BoxTypes.Block:
			//        return new BoxType {StyledNode = new StyledNode(), Type = BoxTypes.Block};
			//    case BoxTypes.Inline:
			//        return new StyledNode();
			//    default:
			//        throw new ArgumentOutOfRangeException();
			//}
		}

		public void Layout(Dimensions containingBlock)
		{
			switch (BoxType)
			{
				case BoxTypes.AnonymousBlock:
					break;
				case BoxTypes.Block:
					LayoutBlock(containingBlock);
					break;
				case BoxTypes.Inline:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}


		private void LayoutBlock(Dimensions containingBlock)
		{
			// Child width can depend on parent width, so we need to calculate this box's width before
			// laying out its children.
			CalculateBlockWidth(containingBlock);

			// Determine where the box is located within its container.
			CalculateBlockPosition(containingBlock);

			// Recursively lay out the children of this box.
			LayoutBlockChildren();

			// Parent height can depend on child height, so `calculate_height` must be called after the
			// children are laid out.
			CalculateBlockHeight();
		}

		/// Calculate the width of a block-level non-replaced element in normal flow.
		///
		/// http://www.w3.org/TR/CSS2/visudet.html#blockwidth
		///
		/// Sets the horizontal margin/padding/border dimensions, and the `width`.
		private void CalculateBlockWidth(Dimensions containingBlock)
		{
			var style = GetStyleNode();

			// `width` has initial value `auto`.
			var auto = new Value { Keyword = "auto" };
			var width = style.GetValue("width") ?? auto;

			// margin, border, and padding have initial value 0.
			var zero = new Value { Length = new KeyValuePair<float, Unit>(0f, Unit.Px) };
			var marginLeft = style.LookupValue("margin-left", "margin", zero);
			var marginRight = style.LookupValue("margin-right", "margin", zero);
			var borderLeft = style.LookupValue("border-left-width", "border-width", zero);
			var borderRight = style.LookupValue("border-right-width", "border-width", zero);
			var paddingLeft = style.LookupValue("padding-left", "padding", zero);
			var paddingRight = style.LookupValue("padding-right", "padding", zero);

			var total = (new List<Value>
            {
                marginLeft,
                marginRight,
                borderLeft,
                borderRight,
                paddingLeft,
                paddingRight,
                width
            }).Select(v => v.ToPx()).Sum();

			// If width is not auto and the total is wider than the container, treat auto margins as 0.
			if (width != auto && total > containingBlock.Content.Width)
			{
				if (marginLeft == auto)
				{
					marginLeft = zero;
				}
				if (marginRight == auto)
				{
					marginRight = zero;
				}
			}
			// Adjust used values so that the above sum equals `containing_block.width`.
			// Each arm of the `match` should increase the total width by exactly `underflow`,
			// and afterward all values should be absolute lengths in px.
			var underflow = containingBlock.Content.Width - total;

			var autoSizedWidth = false;
			var autoSizedLeft = false;
			var autoSizedRight = false;
			if (width == auto)
				autoSizedWidth = true;
			if (marginLeft == auto)
				autoSizedLeft = true;
			if (marginRight == auto)
				autoSizedRight = true;

			if (!autoSizedWidth && !autoSizedLeft && !autoSizedRight)
				marginRight = new Value { Length = new KeyValuePair<float, Unit>(marginRight.ToPx() + underflow, Unit.Px) };

			if (!autoSizedWidth && !autoSizedLeft && autoSizedRight)
				marginRight = new Value { Length = new KeyValuePair<float, Unit>(underflow, Unit.Px) };

			if (!autoSizedWidth && autoSizedLeft && !autoSizedRight)
				marginLeft = new Value { Length = new KeyValuePair<float, Unit>(underflow, Unit.Px) };

			if (autoSizedWidth)
			{
				if (autoSizedLeft)
					marginLeft = new Value { Length = new KeyValuePair<float, Unit>(0, Unit.Px) };
				if (autoSizedRight)
					marginRight = new Value { Length = new KeyValuePair<float, Unit>(0, Unit.Px) };
				if (underflow >= 0.0)
				{
					// Expand width to fill the underflow.
					width = new Value { Length = new KeyValuePair<float, Unit>(underflow, Unit.Px) };
				}
				else
				{
					// Width can't be negative. Adjust the right margin instead.
					width = new Value { Length = new KeyValuePair<float, Unit>(0, Unit.Px) };
					marginRight = new Value { Length = new KeyValuePair<float, Unit>(marginRight.ToPx() + underflow, Unit.Px) };
				}

			}

			// If margin-left and margin-right are both auto, their used values are equal.
			if (!autoSizedWidth && autoSizedLeft && autoSizedRight)
			{
				marginLeft = new Value { Length = new KeyValuePair<float, Unit>(underflow / 2, Unit.Px) };
				marginRight = marginLeft;

			}
			var d = Dimensions;
			d.Content.Width = width.ToPx();
			d.Padding.Left = paddingLeft.ToPx();
			d.Padding.Right = paddingRight.ToPx();
			d.Border.Left = borderLeft.ToPx();
			d.Border.Right = borderRight.ToPx();
			d.Margin.Left = marginLeft.ToPx();
			d.Margin.Right = marginRight.ToPx();
		}

		/// Finish calculating the block's edge sizes, and position it within its containing block.
		///
		/// http://www.w3.org/TR/CSS2/visudet.html#normal-block
		///
		/// Sets the vertical margin/padding/border dimensions, and the `x`, `y` values.
		private void CalculateBlockPosition(Dimensions containingBlock)
		{
			var style = GetStyleNode();
			var d = Dimensions;

			// margin, border, and padding have initial value 0.
			var zero = new Value() { Length = new KeyValuePair<float, Unit>(0, Unit.Px) };

			// If margin-top or margin-bottom is `auto`, the used value is zero.
			d.Margin.Top = style.LookupValue("margin-top", "margin", zero).ToPx();
			d.Margin.Bottom = style.LookupValue("margin-bottom", "margin", zero).ToPx();
			d.Border.Top = style.LookupValue("border-top-width", "border-width", zero).ToPx();
			d.Border.Bottom = style.LookupValue("border-bottom-width", "border-width", zero).ToPx();
			d.Padding.Top = style.LookupValue("padding-top", "padding", zero).ToPx();
			d.Padding.Bottom = style.LookupValue("padding-bottom", "padding", zero).ToPx();

			// Position the box below all the previous boxes in the container.
			d.Content.X = containingBlock.Content.X +
			d.Margin.Left + d.Border.Left + d.Padding.Left;
			d.Content.Y = containingBlock.Content.Y + containingBlock.Content.Height +
			d.Margin.Top + d.Border.Top + d.Padding.Top;
		}

		/// Lay out the block's children within its content area.
		///
		/// Sets `self.dimensions.height` to the total content height.
		private void LayoutBlockChildren()
		{
			var d = Dimensions;
			foreach (var child in Children)
			{
				child.Layout(d);
				// Increment the height so each child is laid out below the previous one.
				d.Content.Height = d.Content.Height + child.Dimensions.GetMarginBox().Height;
			}
		}

		/// Height of a block-level non-replaced element in normal flow with overflow visible.
		private void CalculateBlockHeight()
		{
			// If the height is set to an explicit length, use that exact length.
			// Otherwise, just keep the value set by `layout_block_children`.
			var explicitHeight = GetStyleNode().GetValue("height");
			if (explicitHeight != null)
				Dimensions.Content.Height = explicitHeight.Length.Key;
		}

		public LayoutBox GetInlineContainer()
		{
			switch (BoxType)
			{
				case BoxTypes.Inline:
				case BoxTypes.AnonymousBlock:
					return this;

				case BoxTypes.Block:

					var last = Children.LastOrDefault();
					if (last != null && last.BoxType == BoxTypes.AnonymousBlock)
						return last;

					var anonymousBlock = new LayoutBox();
					Children.Add(anonymousBlock);
					return anonymousBlock;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
