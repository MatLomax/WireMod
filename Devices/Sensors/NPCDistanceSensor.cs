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
			this.Width = 1;
			this.Height = 2;

			this.PinLayout = new List<PinDesign>
			{
				new PinDesign("Out", 0, new Point16(0, 1), "int"),
			};
		}

		public string Output() => this.GetOutput().ToString();

		private int GetOutput()
		{
			var npc = Main.npc.Select(n => (this.LocationRect.Location.ToWorldCoordinates() - n.position).Length()).OrderBy(n => n).FirstOrDefault();

			return (int)npc;
		}
	}
}
