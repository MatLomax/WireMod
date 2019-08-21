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
				new PinDesign("In", 0, new Point16(0, 1), "bool", "Hostile Filter"),
				new PinDesign("In", 1, new Point16(1, 0), "bool", "TownNPC Filter"),
				new PinDesign("Out", 0, new Point16(1, 2), "int", "Distance"),
				new PinDesign("Out", 1, new Point16(2, 1), "string", "Name"),
			};
		}

		public string Output(Pin pin = null)
		{
			var npc = this.GetNearestNPC();
			return pin == this.Pins["Out"][0] ? ((int)(this.LocationRect.Location.ToWorldCoordinates() - npc.position).Length()).ToString() : npc.FullName;
		}

		private NPC GetNearestNPC()
		{
			var npc = Main.npc.Select(n => n);

			if (this.Pins["In"][0].IsConnected() && int.TryParse(this.Pins["In"][0].GetValue(), out var hostile))
			{
				npc = npc.Where(n => n.friendly == (hostile == 0));
			}

			if (this.Pins["In"][1].IsConnected() && int.TryParse(this.Pins["In"][1].GetValue(), out var character))
			{
				npc = npc.Where(n => n.townNPC == (character == 1));
			}

			return npc.OrderBy(n => (this.LocationRect.Location.ToWorldCoordinates() - n.position).Length()).FirstOrDefault();
		}
	}
}
