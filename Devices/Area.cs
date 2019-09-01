using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace WireMod.Devices
{
	public static class AreaFactory
	{
		public static Area Create(string input)
		{
			if (string.IsNullOrEmpty(input) || !input.Contains(":")) return null;

			var split = input.Split(':');

			if (!int.TryParse(split[1], out var distance)) return null;

			var pos = split[2].Split(',');
			if (!int.TryParse(pos[0], out var x) || !int.TryParse(pos[1], out var y)) return null;

			return Create(split[0], distance, new Vector2(x, y));
		}

		public static Area Create(string shape, int distance, Vector2 pos)
		{
			switch (shape)
			{
				case "Circle":
					return new CircArea
					{
						Center = pos,
						Radius = distance
					};
				case "Square":
				default:
					return new RectArea
					{
						Center = pos,
						Radius = distance
					};
			}
		}
	}

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
			if (!Helpers.GetScreenRect().Intersects(this.GetRect())) return;

			var size = this.GetRect().Width;
			var rect = Helpers.CreateRectangle(size, size);
			spriteBatch.Draw(rect, this.Center - Main.screenPosition - new Vector2(size / 2, size / 2) - Helpers.Drift, color * 0.25f);
		}

		public TileArea GetTileArea()
		{
			return new TileArea
			{
				Center = this.Center / 16,
				Radius = this.Radius / 16,
			};
		}
	}

	public class TileArea : Area
	{
		public Rectangle GetRect() => new Rectangle((int)this.Center.X - this.Radius, (int)this.Center.Y - this.Radius, this.Radius * 2 + 1, this.Radius * 2 + 1);

		public override bool Contains(Vector2 point) => this.GetRect().Contains(point.ToPoint());

		public override void Draw(SpriteBatch spriteBatch, Color color)
		{
			var rect = this.GetRect();
			var worldRect = new Rectangle(rect.X * 16, rect.Y * 16, rect.Width * 16, rect.Height * 16);
			if (!Helpers.GetScreenRect().Intersects(worldRect)) return;

			spriteBatch.Draw(Helpers.CreateRectangle(worldRect.Width, worldRect.Height), this.Center - Main.screenPosition - new Vector2(worldRect.Width / 2, worldRect.Height / 2) - Helpers.Drift, color * 0.25f);
		}
	}

	public class CircArea : Area
	{
		public override bool Contains(Vector2 point) => (this.Center - point).Length() < this.Radius;

		public override void Draw(SpriteBatch spriteBatch, Color color)
		{
			var circ = Helpers.CreateCircle(this.Radius * 2);
			spriteBatch.Draw(circ, this.Center - Main.screenPosition - new Vector2(this.Radius, this.Radius) - Helpers.Drift, color * 0.25f);
		}
	}
}