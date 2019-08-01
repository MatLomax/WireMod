﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
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

            this.ValueType = "string";

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("Out", 0, new Point16(0, 1), "string"),
            };
        }

        public string Output(Pin pin = null) => this.Value;

	    public override void OnRightClick(Pin pin = null)
	    {
		    var input = new UserInputUI(this.Value);
		    input.OnSave += (s, e) =>
		    {
			    this.Value = input.Value;

			    if (Main.netMode == NetmodeID.MultiplayerClient)
			    {
				    WireMod.PacketHandler.SendChangeValue(256, Main.myPlayer, this.LocationTile.X, this.LocationTile.Y, this.Value);
			    }
			};

			WireMod.Instance.UserInputUserInterface.SetState(input);
			UserInputUI.Visible = true;
	    }

	    public override List<(string Line, Color Color)> Debug(Pin pin = null)
	    {
		    var debug = base.Debug(pin);

		    if (pin == null)
		    {
			    debug.Add(("----------------", Color.Black));
			    debug.Add(("Right Click to change value", Color.Red));
		    }

		    return debug;
	    }
	}
}
