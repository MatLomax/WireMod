﻿using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;

namespace WireMod.Devices
{
	internal class TileCounter : Device, IOutput
	{
		public TileCounter()
		{
			this.Name = "Nearby Tile Counter Sensor";
			this.Width = 2;
			this.Height = 3;
			this.Origin = new Point16(1, 1);

			this.PinLayout = new List<PinDesign>
			{
				new PinDesign("In", 0, new Point16(1, 0), "area", "Area"),
				new PinDesign("In", 1, new Point16(0, 1), "int", "TileID"),
				new PinDesign("Out", 0, new Point16(1, 2), "int", "Count"),
			};
		}

		public string Output(Pin pin = null) => this.GetOutput().ToString();

		private int GetOutput()
		{
			if (!this.GetPin("Area").IsConnected()) return -1;
			if (!Helpers.TryParseArea(this.GetPin("Area").GetValue(), out var area)) return -2;
			if (!area.HasValue) return -1;
			if (area.Value.AreaType == "Circle") return -2;

			if (!this.GetPin("TileID").IsConnected()) return -1;
			if (!int.TryParse(this.GetPin("TileID").GetValue(), out var _)) return -2;

			return this.GetTiles(area.Value.Radius).Count();
		}

		private IEnumerable<Tile> GetTiles(int distance)
		{
			distance /= 16;

			var tiles = new List<Tile>();

			if (!this.GetPin("TileID").IsConnected() || !int.TryParse(this.GetPin("TileID").GetValue(), out var id)) return tiles;

			Point16 pos;

			var connDev = this.GetPinIn("Area").ConnectedPin.Device;
			if (connDev.GetPin("Point").IsConnected() && Helpers.TryParsePoint(connDev.GetPin("Point").GetValue(), out var point) && point.HasValue)
			{
				pos = point.Value;
			}
			else
			{
				pos = this.LocationOriginTile;
			}

			for (var y = pos.Y - distance; y <= pos.Y + distance; y++)
			{
				for (var x = pos.X - distance; x <= pos.X + distance; x++)
				{
					var tile = Main.tile[x, y];
					if (tile.type == id) tiles.Add(tile);
				}
			}

			return tiles;
		}
	}
}
