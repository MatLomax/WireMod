using System;
using System.Collections.Generic;
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

            var input = this.Pins["In"][0].IsConnected() ? this.Pins["In"][0].GetValue() : "";
            if (this.Pins["In"][1].IsConnected() && (!int.TryParse(this.Pins["In"][1].GetValue(), out var active) || active == 0)) input = "";
            
            input = input.PadRight(tiles.Count, ' ');
            
            for (var i = 0; i < Math.Min(input.Length, tiles.Count); i++)
            {
                var style = GetStyle(input.Substring(i, 1));
                tiles[i].frameX = (short)(style * 18);
            }
        }

        private List<Tile> GetTiles()
        {
            var tiles = new List<Tile>();

            var pos = this.LocationTile + this.Origin;
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

        private static readonly List<string> InputChars = new List<string>
        {
            " ",
            "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z",
            "0","1","2","3","4","5","6","7","8","9"
        };

        private static int GetStyle(string inputChar) => InputChars.Contains(inputChar.ToUpper()) ? InputChars.IndexOf(inputChar.ToUpper()) : 0;

        public override List<(string Line, Color Color, float Size)> Debug(Pin pin = null)
        {
            var debug = base.Debug(pin);

            if (pin == null)
            {
                debug.Add(("----------------", Color.Black, WireMod.SmallText));
                debug.Add(($"Found {this.GetTiles().Count} tiles", Color.Red, WireMod.SmallText));
            }

            return debug;
        }
    }
}
