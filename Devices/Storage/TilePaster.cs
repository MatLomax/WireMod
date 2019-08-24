using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace WireMod.Devices
{
	public class TilePaster : Device, ITriggered
	{
		public TilePaster()
		{
			this.Name = "Tile Paster";
			this.Width = 2;
			this.Height = 3;
			this.Origin = new Point16(1, 1);

			this.PinLayout = new List<PinDesign>
			{
				new PinDesign("In", 0, new Point16(1, 0), "tile[]", "Data"),
				new PinDesign("In", 1, new Point16(0, 1), "bool", "Trigger"),
				new PinDesign("In", 2, new Point16(1, 2), "area", "Area")
			};
		}
		
		public void Trigger(Pin pin = null)
		{
			if (!this.GetPin("Data").IsConnected()) return;
			//if (!this.GetPin("Paste").IsConnected() || this.GetPin("Paste").GetValue() != "1") return;
			if (!this.GetPin("Area").IsConnected() || !Helpers.TryParseArea(this.GetPin("Area").GetValue(), out var area) || !area.HasValue) return;
			if (area.Value.AreaType == "Circle") return;

			var data = this.GetPin("Data").GetValue().Split(';');

			var srcTiles = data.Select(d => new TileInfo(d)).ToList();
			var srcRect = new Rectangle(srcTiles.Min(t => t.X), srcTiles.Min(t => t.Y), (srcTiles.Max(t => t.X) - srcTiles.Min(t => t.X)) + 1, (srcTiles.Max(t => t.Y) - srcTiles.Min(t => t.Y)) + 1);

			var input = this.GetPinIn("Area").ConnectedPin.Device;
			if (!(input is AreaInput areaInput)) return;

			var destRect = areaInput.GetTileArea().GetRect();

			for (var y = 0; y < Math.Min(srcRect.Height, destRect.Height); y++)
			{
				for (var x = 0; x < Math.Min(srcRect.Width, destRect.Width); x++)
				{
					var destTile = Main.tile[destRect.X + x, destRect.Y + y];
					var srcTile = Main.tile[srcRect.X + x, srcRect.Y + y];

					if (Constants.CopyTileBlacklist.Contains(srcTile.type)) continue;

					destTile.CopyFrom(srcTile);
				}
			}
		}
	}
}