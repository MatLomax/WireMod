using System.Collections.Generic;
using Terraria.DataStructures;

namespace WireMod.Devices
{
    internal class NotGate : Device, IOutput
    {
        public NotGate()
        {
            this.Name = "NOT Gate";
            this.Width = 2;
            this.Height = 2;
            this.Origin = new Point16(1, 0);

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("In", 0, new Point16(0, 0), "bool"),
                new PinDesign("Out", 0, new Point16(1, 1), "bool"),
            };
        }

        public string Output() => this.GetOutput().ToString();
        
        private int GetOutput()
        {
            if (!this.Pins["In"][0].IsConnected()) return -1;
            if (this.Pins["In"][0].DataType != "bool") return -1;
            if (!int.TryParse(this.Pins["In"][0].GetValue(), out var in0)) return -1;
            return in0 == 1 ? 0 : 1;
        }
    }
}
