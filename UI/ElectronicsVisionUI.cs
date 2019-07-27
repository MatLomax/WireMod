using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using WireMod.Devices;

namespace WireMod.UI
{
	public class ElectronicsVisionUI : UIState
	{
		public static bool Visible { get; set; }

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			this.DrawDevices(spriteBatch);
			this.DrawWires(spriteBatch);
		}

		private void DrawDevices(SpriteBatch spriteBatch)
		{
		    var pixels = 16/* * Main.UIScale*/;

            var screenRect = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);

			foreach (var device in WireMod.Devices)
			{
				if (device.LocationRect == default(Rectangle)) continue;

				var deviceWorldRect = new Rectangle((int)(device.LocationRect.X * pixels), (int)(device.LocationRect.Y * pixels), (int)(device.Width * pixels), (int)(device.Height * pixels));
				if (!deviceWorldRect.Intersects(screenRect)) continue;

				var deviceScreenRect = new Rectangle(deviceWorldRect.X - screenRect.X, deviceWorldRect.Y - screenRect.Y, deviceWorldRect.Width, deviceWorldRect.Height);

				var texture = WireMod.Instance.GetTexture($"Images/{device.GetType().Name}");
				spriteBatch.Draw(texture, new Vector2(deviceScreenRect.X, deviceScreenRect.Y), device.GetSourceRect(), Color.White, 0f, new Vector2(0, 0), Main.UIScale, SpriteEffects.None, 0f);
				device.Draw(spriteBatch);
			}
		}

		private void DrawWires(SpriteBatch spriteBatch)
		{
			var screenRect = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);

			foreach (var pin in WireMod.Pins)
			{
				if (pin.Type != "Out") continue;
				if (!pin.IsConnected()) continue;

				var pinRect = new Rectangle((int)pin.Location.ToWorldCoordinates(0, 0).X, (int)pin.Location.ToWorldCoordinates(0, 0).Y, 16, 16);
				if (!screenRect.Intersects(pinRect)) continue;

				foreach (var p in ((PinOut) pin).ConnectedPins)
				{
					DrawLine(
						spriteBatch,
						pin.Location.ToWorldCoordinates() - screenRect.Location.ToVector2(),
						p.Location.ToWorldCoordinates() - screenRect.Location.ToVector2(),
						GetWireColor(pin)
					);
				}
			}
		}

		private static Color GetWireColor(Pin pin)
		{
			switch (pin.DataType)
			{
				case "bool":
					if (pin.GetValue() == "0") return Color.Red;
					if (pin.GetValue() == "1") return Color.Green;
					break;
				case "int":
					return Color.Yellow;
				case "string":
					return Color.Blue;
			}

			return Color.White;
		}

		private static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
		{
			var edge = end - start;
			var angle = (float)Math.Atan2(edge.Y, edge.X);
			var zero = /*Main.drawToScreen ?*/ Vector2.Zero /*: new Vector2(Main.offScreenRange)*/;

			var line = new Rectangle((int)(start.X + zero.X), (int)(start.Y + zero.Y), (int)edge.Length(), 3);
			spriteBatch.Draw(Main.magicPixel, line, null, color, angle, new Vector2(0, 0), SpriteEffects.None, 1f);
		}
	}
}
