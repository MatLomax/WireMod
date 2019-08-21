using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace WireMod.Devices
{
    internal class OutputLamp : Device
    {
        public OutputLamp()
        {
            this.Name = "Output Lamp";
            this.Width = 1;
            this.Height = 2;
            this.Origin = new Point16(0, 1);

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("In", 0, new Point16(0, 0), "bool"),
            };
        }

        public override Rectangle GetSourceRect(int style = -1)
        {
            if (style == -1)
            {
                if (!int.TryParse(this.GetPinIn(0).GetValue(), out var input)) return base.GetSourceRect(style);
                style = input + 1;
            }

            return new Rectangle(style * (this.Width * 16), 0, this.Width * 16, this.Height * 16);
        }
    }
}
