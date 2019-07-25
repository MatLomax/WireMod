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
			this.Width = 2;
			this.Height = 3;
			this.Origin = new Point16(1, 1);

			this.PinLayout = new List<PinDesign>
			{
				new PinDesign("In", 0, new Point16(0, 1), "bool", "Hostile Filter"),
				new PinDesign("In", 1, new Point16(1, 0), "bool", "Character Filter"),
				new PinDesign("Out", 0, new Point16(1, 2), "int"),
			};
		}

		public string Output() => this.GetOutput().ToString();

		private int GetOutput()
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

			return (int)npc.Select(n => (this.LocationRect.Location.ToWorldCoordinates() - n.position).Length()).OrderBy(n => n).FirstOrDefault();
		}
	}
}
