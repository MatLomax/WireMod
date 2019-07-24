using System.Collections.Generic;
using Terraria.DataStructures;

namespace WireMod.Devices
{
    internal class AndGate : Device, IOutput
    {
        public AndGate()
        {
            this.Name = "AND Gate";
            this.Width = 3;
            this.Height = 2;
            this.Origin = new Point16(1, 0);

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("In", 0, new Point16(0, 0), "bool"),
                new PinDesign("In", 1, new Point16(2, 0), "bool"),
                new PinDesign("Out", 0, new Point16(1, 1), "bool"),
            };
        }

        public string Output() => this.GetOutput().ToString();

        private int GetOutput()
        {
            if (!this.Pins["In"][0].IsConnected() || !this.Pins["In"][1].IsConnected()) return -1;
            if (this.Pins["In"][0].DataType != "bool" || this.Pins["In"][1].DataType != "bool") return -1;
            if (!int.TryParse(this.Pins["In"][0].GetValue(), out var in0)) return -1;
            if (!int.TryParse(this.Pins["In"][1].GetValue(), out var in1)) return -1;
            return in0 == 1 && in1 == 1 ? 1 : 0;
        }
    }
}
