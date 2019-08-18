using System.Collections.Generic;
using System.Linq;
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
                new PinDesign("In", 0, new Point16(1, 0), "int", "Radius"),
                new PinDesign("In", 1, new Point16(0, 1), "point", "Point"),
                new PinDesign("Out", 0, new Point16(1, 2), "area", "Area"),
            };
        }

        public string Output(Pin pin = null)
        {
            if (!this.Pins["In"][0].IsConnected() || !int.TryParse(this.Pins["In"][0].GetValue(), out var radius)) return "";

            return $"{this.Settings["AreaType"]}:{radius}";
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
