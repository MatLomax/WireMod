﻿using System;
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
			DrawDevices(spriteBatch);
			DrawWires(spriteBatch);
		}

		private static void DrawDevices(SpriteBatch spriteBatch)
		{
            //Main.NewText($"Zoom Target: {Main.GameZoomTarget}");
            //Main.NewText($"UI Scale: {Main.UIScale}");
		    var scaled = 16 * Main.GameZoomTarget;

            var screenRect = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);

			foreach (var device in WireMod.Devices)
			{
				if (device.LocationRect == default(Rectangle)) continue;

				var deviceWorldRect = new Rectangle((int)(device.LocationRect.X * scaled), (int)(device.LocationRect.Y * scaled), (int)(device.Width * scaled), (int)(device.Height * scaled));
				if (!deviceWorldRect.Intersects(screenRect)) continue;

				var deviceScreenRect = new Rectangle(deviceWorldRect.X - screenRect.X, deviceWorldRect.Y - screenRect.Y, deviceWorldRect.Width, deviceWorldRect.Height);

				var texture = WireMod.Instance.GetTexture($"Images/{device.GetType().Name}");
				spriteBatch.Draw(texture, deviceScreenRect, device.GetSourceRect(), Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0f);
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

			    var pinScreenRect = new Rectangle(pinRect.X - screenRect.X, pinRect.Y - screenRect.Y, pinRect.Width, pinRect.Height);

                DrawLine(
					spriteBatch,
					pin.Location.ToWorldCoordinates() - screenRect.Location.ToVector2(),
					pin.ConnectedPin.Location.ToWorldCoordinates() - screenRect.Location.ToVector2(),
					GetWireColor(pin)
				);
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
			var zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

			var line = new Rectangle((int)(start.X + zero.X), (int)(start.Y + zero.Y), (int)edge.Length(), 3);
			spriteBatch.Draw(Main.magicPixel, line, null, color, angle, new Vector2(0, 0), SpriteEffects.None, 1f);
		}
	}
}