using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using WireMod.UI;

namespace WireMod.Devices
{
    internal class StringConstant : Device, IOutput
    {
        public StringConstant()
        {
            this.Name = "Constant String";

            this.Settings.Add("Value", "");

	        this.RightClickHelp = "Right Click to change value";

			this.PinLayout = new List<PinDesign>
            {
                new PinDesign("Out", 0, new Point16(0, 1), "string"),
            };
        }

        public string Output(Pin pin = null) => this.Settings["Value"];

	    public override void OnRightClick(Pin pin = null)
	    {
		    var input = new UserInputUI(this.Settings["Value"]);
		    input.OnSave += (s, e) =>
		    {
			    this.Settings["Value"] = input.Value;

			    if (Main.netMode == NetmodeID.MultiplayerClient)
			    {
				    WireMod.PacketHandler.SendChangeSetting(256, Main.myPlayer, this.LocationTile.X, this.LocationTile.Y, "Value", this.Settings["Value"]);
			    }
			};

			WireMod.Instance.UserInputUserInterface.SetState(input);
			UserInputUI.Visible = true;
	    }
	}
}
