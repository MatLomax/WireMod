using System.Collections.Generic;
using Terraria.DataStructures;

namespace WireMod.Devices
{
    internal class FalseConstant : Device, IOutput
    {
        public FalseConstant()
        {
            this.Name = "False Constant";

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("Out", 0, new Point16(0, 1), "bool"),
            };
        }

        public string Output() => "0";
    }
}
