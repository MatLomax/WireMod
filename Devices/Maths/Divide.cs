using System.Collections.Generic;
using Terraria.DataStructures;

namespace WireMod.Devices
{
    internal class Divide : Device, IOutput
    {
        public Divide()
        {
            this.Name = "Divide";
            this.Width = 3;
            this.Height = 2;
            this.Origin = new Point16(1, 0);

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("In", 0, new Point16(0, 0), "int"),
                new PinDesign("In", 1, new Point16(2, 0), "int"),
                new PinDesign("Out", 0, new Point16(1, 1), "int"),
            };
        }

        public string Output(Pin pin = null) => this.GetOutput().ToString();

        private int GetOutput()
        {
            if (!this.Pins["In"][0].IsConnected() || !this.Pins["In"][1].IsConnected()) return -1;
            if (this.Pins["In"][0].DataType != "int" || this.Pins["In"][1].DataType != "int") return -1;
            if (!int.TryParse(this.Pins["In"][0].GetValue(), out var in0)) return -1;
            if (!int.TryParse(this.Pins["In"][1].GetValue(), out var in1)) return -1;
            if (in1 == 0) return 0; // Divide by zero protection
            return in0 / in1;
        }
    }
}
