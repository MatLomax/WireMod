using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace WireMod.Devices
{
    internal class StringConstant : Device, IOutput
    {
        public StringConstant()
        {
            this.Name = "Constant String";

            this.ValueType = "string";

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("Out", 0, new Point16(0, 1), "string"),
            };
        }

        public string Output() => this.Value;

	    public override void OnRightClick(Pin pin = null)
	    {
			// TODO: Take user input

		    this.Value = Main.player[Main.myPlayer]?.name ?? "";

            if (Main.netMode == NetmodeID.MultiplayerClient)
		    {
			    WireMod.PacketHandler.SendChangeValue(256, Main.myPlayer, this.Location.X, this.Location.Y, this.Value);
		    }
	    }
	}
}
