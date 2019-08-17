using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
				new PinDesign("In", 0, new Point16(1, 0), "int", "Distance"),
				new PinDesign("In", 1, new Point16(0, 1), "int", "Tile ID"),
				new PinDesign("Out", 0, new Point16(1, 2), "int", "Count"),
			};
		}

		public string Output(Pin pin = null) => this.GetOutput().ToString();

		private int GetOutput()
		{
			if (!this.Pins["In"][0].IsConnected()) return -1;
			if (!int.TryParse(this.Pins["In"][0].GetValue(), out var distance)) return -2;
			if (!this.Pins["In"][1].IsConnected()) return -1;
			if (!int.TryParse(this.Pins["In"][1].GetValue(), out var _)) return -2;

			return this.GetTiles(distance).Count();
		}

		private IEnumerable<Tile> GetTiles(int distance)
		{
			var tiles = new List<Tile>();

			if (!this.Pins["In"][1].IsConnected() || !int.TryParse(this.Pins["In"][1].GetValue(), out var id)) return tiles;
			
			var pos = this.LocationTile + this.Origin;

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

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (this.LocationRect == default(Rectangle)) return;
			if (!this.Pins["In"][0].IsConnected() || !int.TryParse(this.Pins["In"][0].GetValue(), out var distance)) return;
			if (distance < 1) return;

			if (!this.LocationWorldRect.Intersects(WireMod.Instance.GetScreenRect())) return;

			var size = ((distance * 2) + 1) * 16;
			var rect = Helpers.CreateRectangle(size, size);

			spriteBatch.Draw(rect, this.LocationOriginScreen - (rect.Size() / 2), Color.LightGreen * 0.25f);
		}
	}
}
