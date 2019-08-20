using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace WireMod.Devices
{
    internal class Variable : Device, IOutput
    {
        public Variable()
        {
            this.Name = "Variable";

            this.Width = 2;
            this.Height = 3;
            this.Origin = new Point16(1, 0);
            
            this.Settings.Add("Value", "0");

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("In", 0, new Point16(1, 0), "int"),
                new PinDesign("In", 1, new Point16(0, 1), "bool", "Write"),
                new PinDesign("Out", 0, new Point16(1, 2), "int"),
            };
        }

        public override void Update(GameTime gameTime)
        {
            if (!this.Pins["In"][1].IsConnected() || !int.TryParse(this.Pins["In"][1].GetValue(), out var write)) write = 0;

            if (this.Pins["In"][0].IsConnected() && write == 1)
            {
                this.Settings["Value"] = this.Pins["In"][0].GetValue();
            }
        }

        public string Output(Pin pin = null) => this.Settings["Value"];
    }
}