﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace WireMod.Devices
{
	internal class PlayerCounter : Device, IOutput
	{
		public PlayerCounter()
		{
			this.Name = "Nearby Player Counter Sensor";
			this.Width = 2;
			this.Height = 3;
			this.Origin = new Point16(1, 1);

			this.PinLayout = new List<PinDesign>
			{
				new PinDesign("In", 0, new Point16(1, 0), "area", "Area"),
				new PinDesign("In", 1, new Point16(0, 1), "teamColor", "Team Color Filter"),
				new PinDesign("Out", 0, new Point16(1, 2), "int", "Count"),
			};
		}

		public string Output(Pin pin = null) => this.GetOutput().ToString();

		private int GetOutput()
		{
			if (!this.Pins["In"][0].IsConnected()) return -1;

			return this.GetPlayers().Count();
		}

		private IEnumerable<Player> GetPlayers()
		{
			var players = Main.player.Select(p => p);

			if (this.Pins["In"][1].IsConnected())
			{
				var team = TeamColor.White;
				if (int.TryParse(this.Pins["In"][1].GetValue(), out var tc))
				{
					team = (TeamColor)tc;
				}
				players = players.Where(p => p.team == (int)team);
			}

			if (!this.Pins["In"][0].IsConnected()) return new List<Player>();

			var input = ((PinIn)this.Pins["In"][0]).ConnectedPin.Device;
			if (!(input is AreaInput)) return new List<Player>();

			var area = ((AreaInput)input).GetArea(this);

			return players.Where(p => area.Contains(p.position));
		}

		public override List<(string Line, Color Color, float Size)> Debug(Pin pin = null)
		{
			var debug = base.Debug(pin);

			if (pin == null)
			{
				debug.Add(("----------------", Color.Black, WireMod.SmallText));
				debug.AddRange(this.GetPlayers().Select(player => ($"Player: {player.name}", Color.Red, WireMod.SmallText)));
			}

			return debug;
		}
	}
}
