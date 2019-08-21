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
				new PinDesign("In", 0, new Point16(0, 0), "teamColor", "TeamColorFilter"),
				new PinDesign("Out", 0, new Point16(0, 2), "int", "Distance"),
				new PinDesign("Out", 1, new Point16(1, 1), "string", "Name"),
			};
		}

		public string Output(Pin pin = null)
		{
			switch (pin?.Name)
			{
				case "Distance": return this.GetOutputDistance(pin).ToString();
				case "Name": return this.GetOutputName(pin);
				default: return "";
			}
		}

		private int GetOutputDistance(Pin pin)
		{
			if (pin == null) return -2;
			if (Main.PlayerList.Count == 0) return -1;

			var nearest = this.GetNearestPlayer();

			if (nearest == null) return -1;

			return (int)(this.LocationOriginWorld - nearest.position).Length();
		}

		private string GetOutputName(Pin pin)
		{
			if (pin == null) return "";
			if (Main.PlayerList.Count == 0) return "";

			var nearest = this.GetNearestPlayer();

			return nearest?.name ?? "";
		}

		private Player GetNearestPlayer()
		{
			var player = Main.player.OrderBy(p => (this.LocationOriginWorld - p.position).Length());

			if (!this.GetPin("TeamColorFilter").IsConnected()) return player.FirstOrDefault();

			var team = TeamColor.White;
			if (int.TryParse(this.GetPin("TeamColorFilter").GetValue(), out var tc))
			{
				team = (TeamColor)tc;
			}

			return player.FirstOrDefault(p => p.team == (int)team);
		}
	}
}
