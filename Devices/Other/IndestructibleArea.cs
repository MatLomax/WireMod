using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace WireMod.Devices
{
    internal class IndestructibleArea : Device
    {
        public IndestructibleArea()
        {
            this.Name = "IndestructibleArea";
            this.Width = 2;
            this.Height = 2;
            this.Origin = new Point16(1, 1);

            

            this.RightClickHelp = "[RIGHT CLICK : UNUSED]";


            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("In", 0, new Point16(1, 0), "bool", "Activator [UNUSED]"),
                new PinDesign("In", 1, new Point16(0, 1), "area", "Area"),
            };

        }

        public override void Update(GameTime gameTime)
        {
            //UNUSED
        }

        public Area GetTileArea()
        {
            return ((this.Pins["In"][1].Wires[0].EndPin.Device as AreaInput) == null) ? null : (this.Pins["In"][1].Wires[0].EndPin.Device as AreaInput).GetTileArea();
        }
    }

}

