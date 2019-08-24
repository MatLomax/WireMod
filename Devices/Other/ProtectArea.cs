using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace WireMod.Devices
{
    internal class ProtectArea : Device
    {
        public ProtectArea()
        {
            this.Name = "Protect Area";
            this.Width = 3;
            this.Height = 2;
            this.Origin = new Point16(1, 1);

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("In", 0, new Point16(1, 0), "area", "Area"),
                new PinDesign("In", 1, new Point16(0, 1), "bool", "ProtectPlace"),
                new PinDesign("In", 2, new Point16(2, 1), "bool", "ProtectDestroy"),
            };
        }

        public Area GetProtectPlaceArea()
        {
            if (!this.GetPin("ProtectPlace").IsConnected() || !this.GetPin("ProtectPlace").GetBool()) return null;
            return ((AreaInput)this.GetPinIn("Area").ConnectedPin?.Device)?.GetTileArea();
        }

        public Area GetProtectDestroyArea()
        {
            if (!this.GetPin("ProtectDestroy").IsConnected() || !this.GetPin("ProtectDestroy").GetBool()) return null;
            return ((AreaInput)this.GetPinIn("Area").ConnectedPin?.Device)?.GetTileArea();
        }
    }

}

