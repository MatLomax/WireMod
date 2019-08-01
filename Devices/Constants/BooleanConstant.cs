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
			this.Value = "0";

			this.PinLayout = new List<PinDesign>
			{
				new PinDesign("Out", 0, new Point16(0, 1), "bool"),
			};
		}

		public string Output(Pin pin = null) => this.Value;

		public override void OnRightClick(Pin pin = null)
		{
			if (!int.TryParse(this.Value, out var value)) return;
			this.Value = (value == 0 ? 1 : 0).ToString();
			
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				WireMod.PacketHandler.SendChangeValue(256, Main.myPlayer, this.LocationTile.X, this.LocationTile.Y, this.Value);
			}
		}

		public override Rectangle GetSourceRect(int style = -1)
		{
			if (style == -1)
			{
				if (!int.TryParse(this.Value, out var value)) return base.GetSourceRect(style);
				style = value;
			}

			return new Rectangle(style * (this.Width * 16), 0, this.Width * 16, this.Height * 16);
		}

		public override List<(string Line, Color Color)> Debug(Pin pin = null)
		{
			var debug = base.Debug(pin);

			if (pin == null)
			{
				debug.Add(("----------------", Color.Black));
				debug.Add(("Right Click to toggle value", Color.Red));
			}
			
			return debug;
		}
	}
}