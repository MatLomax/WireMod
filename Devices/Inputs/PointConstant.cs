using System.Collections.Generic;
using Terraria.DataStructures;

namespace WireMod.Devices
{
    internal class PointConstant : Device, IOutput
    {
        public PointConstant()
        {
            this.Name = "Point Input";
            this.Width = 1;
            this.Height = 2;
            this.Origin = new Point16(0, 0);

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("Out", 0, new Point16(0, 1), "point"),
            };
        }

        public string Output(Pin pin = null)
        {
            var pos = this.LocationTile + this.Origin;
            return $"{pos.X},{pos.Y}";
        }
    }
}
