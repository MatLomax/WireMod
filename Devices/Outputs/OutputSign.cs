using System.Collections.Generic;
using Terraria.DataStructures;

namespace WireMod.Devices
{
    internal class OutputSign : Device
    {
        public OutputSign()
        {
            this.Name = "Output Sign";
            this.Width = 1;
            this.Height = 2;
            this.Origin = new Point16(0, 1);

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("In", 0, new Point16(0, 0), "string"),
            };
        }
    }
}
