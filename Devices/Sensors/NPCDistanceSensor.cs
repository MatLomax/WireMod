using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;

namespace WireMod.Devices
{
	internal class NPCDistanceSensor : Device, IOutput
	{
		public NPCDistanceSensor()
		{
			this.Name = "Nearest NPC Distance Sensor";
			this.Width = 3;
			this.Height = 3;
			this.Origin = new Point16(1, 1);

			this.PinLayout = new List<PinDesign>
			{
				new PinDesign("In", 0, new Point16(0, 1), "bool", "IsHostile"),
				new PinDesign("In", 1, new Point16(1, 0), "bool", "IsTownNPC"),
				new PinDesign("Out", 0, new Point16(1, 2), "int", "Distance"),
				new PinDesign("Out", 1, new Point16(2, 1), "string", "Name"),
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
			if (Main.npc.Length == 0) return -1;

			var nearest = this.GetNearestNPC();

			if (nearest == null) return -1;

			return (int)(this.LocationOriginWorld - nearest.position).Length();
		}

		private string GetOutputName(Pin pin)
		{
			if (pin == null) return "";
			if (Main.npc.Length == 0) return "";

			var nearest = this.GetNearestNPC();

			return nearest?.FullName ?? "";
		}

		private NPC GetNearestNPC()
		{
			var npc = Main.npc.Select(n => n);

			if (this.GetPin("IsHostile").IsConnected() && int.TryParse(this.GetPin("IsHostile").GetValue(), out var hostile))
			{
				npc = npc.Where(n => n.friendly == (hostile == 0));
			}

			if (this.GetPin("IsTownNPC").IsConnected() && int.TryParse(this.GetPin("IsTownNPC").GetValue(), out var townNPC))
			{
				npc = npc.Where(n => n.townNPC == (townNPC == 1));
			}

			return npc.OrderBy(n => (this.LocationOriginWorld - n.position).Length()).FirstOrDefault();
		}
	}
}
