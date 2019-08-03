using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace WireMod.Devices
{
	internal class BooleanConstant : Device, IOutput
	{
		public BooleanConstant()
		{
			this.Name = "Boolean Constant";

            this.Settings.Add("Value", "0");

			this.RightClickHelp = "Right Click to toggle value";

			this.PinLayout = new List<PinDesign>
			{
				new PinDesign("Out", 0, new Point16(0, 1), "bool"),
			};
		}

		public string Output(Pin pin = null) => this.Settings["Value"];

		public override void OnRightClick(Pin pin = null)
		{
			this.Settings["Value"] = (this.Settings["Value"] == "0" ? "1" : "0");
			
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				WireMod.PacketHandler.SendChangeSetting(256, Main.myPlayer, this.LocationTile.X, this.LocationTile.Y, "Value", this.Settings["Value"]);
			}
		}

		public override Rectangle GetSourceRect(int style = -1)
		{
			if (style == -1)
			{
				if (!int.TryParse(this.Settings["Value"], out var value)) return base.GetSourceRect(style);
				style = value;
			}

			return new Rectangle(style * (this.Width * 16), 0, this.Width * 16, this.Height * 16);
		}
	}
}