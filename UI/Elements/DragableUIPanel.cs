using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace WireMod.UI.Elements
{
	public class DragableUIPanel : UIPanel
	{
		private Vector2 _offset;
		public bool Dragging;

		public override void MouseDown(UIMouseEvent evt)
		{
			base.MouseDown(evt);
			this.DragStart(evt);
		}

		public override void MouseUp(UIMouseEvent evt)
		{
			base.MouseUp(evt);
			this.DragEnd(evt);
		}

		private void DragStart(UIMouseEvent evt)
		{
			this._offset = new Vector2(evt.MousePosition.X - this.Left.Pixels, evt.MousePosition.Y - this.Top.Pixels);
			this.Dragging = true;
		}

		private void DragEnd(UIMouseEvent evt)
		{
			var end = evt.MousePosition;
			this.Dragging = false;

			this.Left.Set(end.X - this._offset.X, 0f);
			this.Top.Set(end.Y - this._offset.Y, 0f);

			Recalculate();
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (this.ContainsPoint(Main.MouseScreen))
			{
				Main.LocalPlayer.mouseInterface = true;
			}

			if (this.Dragging)
			{
				Left.Set(Main.mouseX - this._offset.X, 0f);
				Top.Set(Main.mouseY - this._offset.Y, 0f);
				Recalculate();
			}

			var parentSpace = this.Parent.GetDimensions().ToRectangle();
			if (this.GetDimensions().ToRectangle().Intersects(parentSpace)) return;

			this.Left.Pixels = Utils.Clamp(this.Left.Pixels, 0, parentSpace.Right - this.Width.Pixels);
			this.Top.Pixels = Utils.Clamp(this.Top.Pixels, 0, parentSpace.Bottom - this.Height.Pixels);

			Recalculate();
		}
	}
}