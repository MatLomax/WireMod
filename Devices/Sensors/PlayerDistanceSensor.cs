using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;

namespace WireMod.Devices
{
	internal class PlayerDistanceSensor : Device, IOutput
	{
		public PlayerDistanceSensor()
		{
			this.Name = "Nearest Player Distance Sensor";
			this.Width = 2;
			this.Height = 3;
			this.Origin = new Point16(0, 1);

			this.PinLayout = new List<PinDesign>
			{
				new PinDesign("In", 0, new Point16(0, 0), "teamColor", "TeamColor Filter"),
				new PinDesign("Out", 0, new Point16(0, 2), "int", "Distance"),
				new PinDesign("Out", 1, new Point16(1, 1), "string", "Name"),
			};
		}

		public string Output(Pin pin = null) => pin != null && pin.DataType == "string" ? this.GetOutputName(pin) : this.GetOutput(pin).ToString();

		private int GetOutput(Pin pin)
		{
			if (pin == null) return -1;
			if (Main.PlayerList.Count == 0) return -1;

			var nearest = this.GetNearestPlayer();

			if (nearest == null) return -1;

			return (int)(this.LocationRect.Location.ToWorldCoordinates() - nearest.position).Length();
		}

		private string GetOutputName(Pin pin)
		{
			if (pin == null) return "";
			if (Main.PlayerList.Count == 0) return "";

			var nearest = this.GetNearestPlayer();

			if (nearest == null) return "";

			return nearest.name;
		}

		private Player GetNearestPlayer()
		{
			var player = Main.player.OrderBy(p => (this.LocationRect.Location.ToWorldCoordinates() - p.position).Length());
			Player nearest;

			if (this.Pins["In"][0].IsConnected())
			{
				var team = TeamColor.White;
				if (int.TryParse(this.Pins["In"][0].GetValue(), out var tc))
				{
					team = (TeamColor)tc;
				}

				nearest = player.FirstOrDefault(p => p.team == (int)team);
			}
			else
			{
				nearest = player.FirstOrDefault();
			}

			return nearest;
		}
	}
}
