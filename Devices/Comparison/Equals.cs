
using System.Collections.Generic;
using System.Linq;
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

            this.Settings.Add("CompareType", CompareTypes.First());
            this.RightClickHelp = $"Right Click to change comparison type ({string.Join("/", CompareTypes)})";

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("In", 0, new Point16(0, 0), this.Settings["CompareType"]),
                new PinDesign("In", 1, new Point16(2, 0), this.Settings["CompareType"]),
                new PinDesign("Out", 0, new Point16(1, 1), "bool"),
            };
        }

        public string Output(Pin pin = null) => this.GetOutput().ToString();

        private int GetOutput()
        {
            if (!this.Pins["In"][0].IsConnected() || !this.Pins["In"][1].IsConnected()) return -1;
            if (this.Pins["In"][0].DataType != this.Pins["In"][1].DataType) return -1;

            if (this.Settings["CompareType"] == "int")
            {
                if (!int.TryParse(this.Pins["In"][0].GetValue(), out var in0)) return -1;
                if (!int.TryParse(this.Pins["In"][1].GetValue(), out var in1)) return -1;
                return in0 == in1 ? 1 : 0;
            }

            return this.Pins["In"][0].GetValue() == this.Pins["In"][1].GetValue() ? 1 : 0;
        }

        public override void OnRightClick(Pin pin = null)
        {
            this.Settings["CompareType"] = CompareTypes[(CompareTypes.IndexOf(this.Settings["CompareType"]) + 1) % CompareTypes.Count];
            foreach (var p in this.Pins["In"].Values) p.DataType = this.Settings["CompareType"];
        }

        private static readonly List<string> CompareTypes = new List<string>
        {
            "int",
            "string",
        };
    }
}
