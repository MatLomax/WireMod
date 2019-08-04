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

		public static float DeviceVisibility { get; set; } = 1f;
		public static float WireVisibility { get; set; } = 0.75f;

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			DrawDevices(spriteBatch);
			DrawWires(spriteBatch);
		}

		private static void DrawDevices(SpriteBatch spriteBatch)
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
				spriteBatch.Draw(texture, new Vector2(deviceScreenRect.X, deviceScreenRect.Y), device.GetSourceRect(), Color.White * DeviceVisibility, 0f, new Vector2(0, 0), Main.UIScale, SpriteEffects.None, 0f);
				device.Draw(spriteBatch);
			}
		}

		private static void DrawWires(SpriteBatch spriteBatch)
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

					DrawWireDot(spriteBatch, p.Location.ToWorldCoordinates() - screenRect.Location.ToVector2());
				}

				DrawWireDot(spriteBatch, pin.Location.ToWorldCoordinates() - screenRect.Location.ToVector2());
			}

			// Draw connecting wire
			var modPlayer = Main.LocalPlayer.GetModPlayer<WireModPlayer>();
			if (modPlayer.ConnectingPin == null) return;

			DrawLine(
				spriteBatch,
				modPlayer.ConnectingPin.Location.ToWorldCoordinates() - screenRect.Location.ToVector2(),
				Main.MouseScreen,
				GetWireColor(modPlayer.ConnectingPin)
			);

			DrawWireDot(spriteBatch, modPlayer.ConnectingPin.Location.ToWorldCoordinates() - screenRect.Location.ToVector2());
		}

		private static void DrawWireDot(SpriteBatch spriteBatch, Vector2 position)
		{
			spriteBatch.Draw(Helpers.CreateCircle(10), position - new Vector2(5, 5), Color.Black * 0.25f * WireVisibility);
			spriteBatch.Draw(Helpers.CreateCircle(5), position - new Vector2(3, 3), Color.White * 0.5f * WireVisibility);
		}

		private static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
		{
			var edge = end - start;
			var angle = (float)Math.Atan2(edge.Y, edge.X);

			var line = new Rectangle((int)start.X + 1, (int)start.Y, (int)edge.Length(), 3);
			spriteBatch.Draw(Main.magicPixel, line, null, color * WireVisibility, angle, new Vector2(0, 0), SpriteEffects.None, 1f);
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
	}
}
