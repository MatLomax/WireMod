using System.Collections.Generic;
using Terraria.DataStructures;

namespace WireMod.Devices
{
    internal class NotGate : Device, IOutput
    {
        public NotGate()
        {
            this.Name = "NOT Gate";
            this.Width = 1;
            this.Height = 3;
            this.Origin = new Point16(0, 1);

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("In", 0, new Point16(0, 0), "bool"),
                new PinDesign("Out", 0, new Point16(0, 2), "bool"),
            };
        }

        public string Output(Pin pin = null) => this.GetOutput().ToString();
        
        private int GetOutput()
        {
            if (!this.Pins["In"][0].IsConnected()) return -1;
            if (!int.TryParse(this.Pins["In"][0].GetValue(), out var in0)) return -1;
            return in0 == 1 ? 0 : 1;
        }
    }
}
