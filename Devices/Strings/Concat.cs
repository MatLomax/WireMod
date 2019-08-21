using System.Collections.Generic;
using Terraria.DataStructures;

namespace WireMod.Devices
{
    internal class Concat : Device, IOutput
    {
        public Concat()
        {
            this.Name = "Concat";
            this.Width = 3;
            this.Height = 2;
            this.Origin = new Point16(1, 0);

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("In", 0, new Point16(0, 0), "string"),
                new PinDesign("In", 1, new Point16(2, 0), "string"),
                new PinDesign("Out", 0, new Point16(1, 1), "string"),
            };
        }

        public string Output(Pin pin = null) => this.GetPinIn(0).GetValue() + this.GetPinIn(1).GetValue();
    }
}
