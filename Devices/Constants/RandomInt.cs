using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using WireMod.UI;

namespace WireMod.Devices
{
    internal class RandomInt : Device, IOutput, ITriggered
    {
        public RandomInt()
        {
            this.Name = "Random Integer";
            this.Width = 3;
            this.Height = 3;
            this.Origin = new Point16(1, 1);

            this.Settings.Add("Value", "0");

            this.RightClickHelp = "Right Click to generate new value";

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("In", 0, new Point16(1, 0), "bool", "Trigger"),
                new PinDesign("In", 1, new Point16(0, 1), "int", "Min"),
                new PinDesign("In", 2, new Point16(2, 1), "int", "Max"),
                new PinDesign("Out", 0, new Point16(1, 2), "int"),
            };
        }

        public string Output(Pin pin = null) => this.Settings["Value"];

        public override void OnRightClick(Pin pin = null) => this.Trigger();

        public void Trigger(Pin pin = null)
        {
            if (!int.TryParse(this.Pins["In"][1].GetValue(), out var min)) min = 0;
            if (!int.TryParse(this.Pins["In"][2].GetValue(), out var max)) max = 100;

            this.Settings["Value"] = WorldGen.genRand.Next(min, max).ToString();
        }
    }
}
