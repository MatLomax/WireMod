
using System.Collections.Generic;
using Terraria.DataStructures;

namespace WireMod.Devices
{
    internal class Equals : Device, IOutput
    {
        public Equals()
        {
            this.Name = "Equals";
            this.Width = 3;
            this.Height = 2;
            this.Origin = new Point16(1, 0);

            this.AutoTypes.AddRange(new [] {"int", "bool", "string"});
            
            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("In", 0, new Point16(0, 0), "int", "", true),
                new PinDesign("In", 1, new Point16(2, 0), "int", "", true),
                new PinDesign("Out", 0, new Point16(1, 1), "bool"),
            };
        }

        public string Output(Pin pin = null) => this.GetOutput().ToString();

        private int GetOutput()
        {
            if (!this.GetPinIn(0).IsConnected() || !this.GetPinIn(1).IsConnected()) return -1;
            if (this.GetPinIn(0).DataType != this.GetPinIn(1).DataType) return -1;

            switch (this.GetPinIn(0).DataType)
            {
                case "int":
                    if (!int.TryParse(this.GetPinIn(0).GetValue(), out var in0)) return -2;
                    if (!int.TryParse(this.GetPinIn(1).GetValue(), out var in1)) return -2;
                    return in0 == in1 ? 1 : 0;
                case "string":
                default:
                    return GetPinIn(0).GetValue() == this.GetPinIn(1).GetValue() ? 1 : 0;
            }
        }
    }
}
