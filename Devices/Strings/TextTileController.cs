using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using WireMod.Tiles;

namespace WireMod.Devices
{
    internal class TextTileController : Device
    {
        public TextTileController()
        {
            this.Name = "TextTileController";
            this.Width = 2;
            this.Height = 2;
            this.Origin = new Point16(1, 1);

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("In", 0, new Point16(1, 0), "string"),
                new PinDesign("In", 1, new Point16(0, 1), "bool", "Activated"),
            };
        }

        public override void Update(GameTime gameTime)
        {
            var tiles = this.GetTiles();

            var input = this.GetPinIn(0).IsConnected() ? this.GetPinIn(0).GetValue() : "";
            if (this.GetPin("Activated").IsConnected() && (!int.TryParse(this.GetPin("Activated").GetValue(), out var active) || active == 0)) input = "";
            
            input = input.PadRight(tiles.Count, ' ');
            
            for (var i = 0; i < Math.Min(input.Length, tiles.Count); i++)
            {
                var style = GetStyle(input.Substring(i, 1));
                tiles[i].frameX = (short)((style % 16) * 18);
                tiles[i].frameY = (short)((style / 16) * 18);
            }
        }

        private List<Tile> GetTiles()
        {
            var tiles = new List<Tile>();

            var pos = this.LocationOriginTile;
            var x = pos.X;

            for (var i = 0; i < 255; i++)
            {
                var tile = Main.tile[++x, pos.Y];
                if (tile == null) break;
                if (tile.type != WireMod.Instance.TileType<TextTile>()) break;

                tiles.Add(tile);
            }

            return tiles;
        }
        
        public override List<(string Line, Color Color, float Size)> Debug(Pin pin = null)
        {
            var debug = base.Debug(pin);

            debug.Add(("----------------", Color.Black, WireMod.SmallText));
            debug.Add(($"Found {this.GetTiles().Count} tiles", Color.Red, WireMod.SmallText));

            return debug;
        }

        private static int GetStyle(string inputChar)
        {
            var ccc = Encoding.GetEncoding("ISO-8859-1");
            var by = ccc.GetBytes(inputChar);
            return string.Equals(inputChar, ccc.GetString(by)) ? by[0] : 0;
        }
    }
}
