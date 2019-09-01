using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

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
			if (!this.GetPin("Data").IsConnected() || !this.GetPin("Area").IsConnected()) return;

			var area = AreaFactory.Create(this.GetPin("Area").GetValue());
			if (!(area is RectArea rectArea)) return;

			var data = this.GetPin("Data").GetValue().Split(';');

			var srcTiles = data.Select(d => new TileInfo(d)).ToList();
			var srcRect = new Rectangle(srcTiles.Min(t => t.X), srcTiles.Min(t => t.Y), (srcTiles.Max(t => t.X) - srcTiles.Min(t => t.X)) + 1, (srcTiles.Max(t => t.Y) - srcTiles.Min(t => t.Y)) + 1);
			
			var destRect = rectArea.GetTileArea().GetRect();

			for (var y = 0; y < Math.Min(srcRect.Height, destRect.Height); y++)
			{
				for (var x = 0; x < Math.Min(srcRect.Width, destRect.Width); x++)
				{
					var srcTile = Main.tile[srcRect.X + x, srcRect.Y + y];
					var destTile = Main.tile[destRect.X + x, destRect.Y + y];

					if (Constants.CopyTileBlacklist.Contains(srcTile.type)) continue;

					var (wire, wire2, wire3, wire4) = (destTile.wire(), destTile.wire2(), destTile.wire3(), destTile.wire4());

					destTile.CopyFrom(srcTile);

					// Preserve tile wires
					destTile.wire(wire);
					destTile.wire2(wire2);
					destTile.wire3(wire3);
					destTile.wire4(wire4);

					WorldGen.SquareTileFrame(destRect.X + x, destRect.Y + y);
					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendTileSquare(-1, destRect.X + x, destRect.Y + y, 1);
					}
				}
			}
		}
	}
}