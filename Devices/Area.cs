using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace WireMod.Devices
{
	public abstract class Area
	{
		public Vector2 Center { get; set; }
		public int Radius { get; set; }

		public abstract bool Contains(Vector2 point);

		public virtual void Draw(SpriteBatch spriteBatch, Color color) { }
	}

	public class RectArea : Area
	{
		public Rectangle GetRect() => new Rectangle((int)this.Center.X - this.Radius - 8, (int)this.Center.Y - this.Radius - 8, this.Radius * 2 + 16, this.Radius * 2 + 16);

		public override bool Contains(Vector2 point) => this.GetRect().Contains(point.ToPoint());

		public override void Draw(SpriteBatch spriteBatch, Color color)
		{
			if (!WireMod.Instance.GetScreenRect().Intersects(this.GetRect())) return;

			var size = this.GetRect().Width;
			var rect = Helpers.CreateRectangle(size, size);
			spriteBatch.Draw(rect, this.Center - Main.screenPosition - new Vector2(size / 2, size / 2), color * 0.25f);
		}
	}

	public class CircArea : Area
	{
		public override bool Contains(Vector2 point) => (this.Center - point).Length() < this.Radius;

		public override void Draw(SpriteBatch spriteBatch, Color color)
		{
			var circ = Helpers.CreateCircle(this.Radius * 2);
			spriteBatch.Draw(circ, this.Center - Main.screenPosition - new Vector2(this.Radius, this.Radius), color * 0.25f);
		}
	}
}