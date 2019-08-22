using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace WireMod.Devices
{
	public class TileCopier : Device, IOutput
	{
		public TileCopier()
		{
			this.Name = "Tile Copier";
			this.Width = 2;
			this.Height = 3;
			this.Origin = new Point16(1, 1);

			this.Settings.Add("Value", "");
			
			this.PinLayout = new List<PinDesign>
			{
				new PinDesign("In", 0, new Point16(1, 0), "area", "Area"),
				new PinDesign("In", 1, new Point16(0, 1), "bool", "Copy"),
				new PinDesign("Out", 0, new Point16(1, 2), "tile[]"),
			};
		}

		public override void Update(GameTime gameTime)
		{
			if (!this.GetPin("Copy").IsConnected() || this.GetPin("Copy").GetValue() != "1") return;

			if (!this.GetPin("Area").IsConnected())
			{
				this.Settings["Value"] = "";
				return;
			}

			if (!Helpers.TryParseArea(this.GetPin("Area").GetValue(), out var area) || !area.HasValue) return;
			if (area.Value.AreaType == "Circle") return;

			this.Settings["Value"] = string.Join(";", this.GetTiles().Select(t => t.ToString()));
		}

		public string Output(Pin pin = null) => this.Settings["Value"];

		private IEnumerable<TileInfo> GetTiles()
		{
			var tiles = new List<TileInfo>();

			var input = this.GetPinIn("Area").ConnectedPin.Device;
			if (!(input is AreaInput areaInput)) return tiles;

			var areaRect = areaInput.GetTileArea().GetTileRect();

			for (var y = areaRect.Y; y < areaRect.Y + areaRect.Height; y++)
			{
				for (var x = areaRect.X; x < areaRect.X + areaRect.Width; x++)
				{
					tiles.Add(new TileInfo(x, y));
				}
			}

			return tiles;
		}
	}
}