using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace WireMod.Devices
{
    internal class NumberConstant : Device, IOutput
    {
        public NumberConstant()
        {
            this.Name = "Constant Number";

            this.ValueType = "int";
            this.Value = WorldGen.genRand.Next(1, 100).ToString();

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("Out", 0, new Point16(0, 1), "int"),
            };
        }

        public string Output() => this.Value;

        public override void OnRightClick(Pin pin = null)
        {
            // TODO: Take user input
            this.Value = WorldGen.genRand.Next(1, 100).ToString();

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
		        WireMod.PacketHandler.SendChangeValue(256, Main.myPlayer, this.Location.X, this.Location.Y, this.Value);
            }
        }
    }
}
