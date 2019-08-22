using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;

namespace WireMod.Devices
{
    internal class AreaInput : Device, IOutput
    {
        public AreaInput()
        {
            this.Name = "Area Input";
            this.Width = 2;
            this.Height = 3;
            this.Origin = new Point16(1, 1);

            this.Settings.Add("AreaType", AreaTypes.First());

            this.RightClickHelp = "Right Click to change area type";

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("In", 0, new Point16(1, 0), "int", "Distance"),
                new PinDesign("In", 1, new Point16(0, 1), "point", "Point"),
                new PinDesign("Out", 0, new Point16(1, 2), "area", "Area"),
            };
        }

        public string Output(Pin pin = null)
        {
            if (!this.GetPin("Distance").IsConnected() || !int.TryParse(this.GetPin("Distance").GetValue(), out var distance)) return "";

            return $"{this.Settings["AreaType"]}:{distance}";
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!this.GetPin("Distance").IsConnected()) return;

            this.GetArea().Draw(spriteBatch, Color.LightGreen);
        }

        public Area GetArea()
        {
            var radius = 0;
            if (this.GetPin("Distance").IsConnected() && int.TryParse(this.GetPin("Distance").GetValue(), out var dist))
            {
                radius = dist;
            }

            var pos = this.LocationOriginWorld;
            if (this.GetPin("Point").IsConnected() && Helpers.TryParsePoint(this.GetPin("Point").GetValue(), out var point) && point.HasValue)
            {
                pos = point.Value.ToWorldCoordinates();
            }
            
            if (this.Settings["AreaType"] == "Circle")
            {
                return new CircArea
                {
                    Center = pos,
                    Radius = radius
                };
            }

            return new RectArea
            {
                Center = pos,
                Radius = radius
            };
        }

        public RectArea GetTileArea()
        {
            var area = this.GetArea();

            return new RectArea
            {
                Center = area.Center / 16,
                Radius = area.Radius / 16,
            };
        }

        public override void OnRightClick(Pin pin = null)
        {
            this.Settings["AreaType"] = AreaTypes[(AreaTypes.IndexOf(this.Settings["AreaType"]) + 1) % AreaTypes.Count];
        }

        private static readonly List<string> AreaTypes = new List<string>
        {
            "Square",
            "Circle",
        };
    }
}
