using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace WireMod.Devices
{
	internal class NPCCounter : Device, IOutput
	{
		public NPCCounter()
		{
			this.Name = "Nearby NPC Counter Sensor";
			this.Width = 3;
			this.Height = 3;
			this.Origin = new Point16(1, 1);

			this.PinLayout = new List<PinDesign>
			{
				new PinDesign("In", 0, new Point16(1, 0), "area", "Area"),
				new PinDesign("In", 1, new Point16(0, 1), "bool", "IsHostile"),
				new PinDesign("In", 2, new Point16(2, 1), "bool", "IsTownNPC"),
				new PinDesign("Out", 0, new Point16(1, 2), "int", "Count"),
			};
		}

		public string Output(Pin pin = null) => this.GetOutput().ToString();

		private int GetOutput()
		{
			if (!this.GetPin("Area").IsConnected()) return -1;

			return this.GetNPCs().Count();
		}

		private IEnumerable<NPC> GetNPCs()
		{
			if (!this.GetPin("Area").IsConnected()) return new List<NPC>();

			var input = this.GetPinIn("Area").ConnectedPin.Device;
			if (!(input is AreaInput areaInput)) return new List<NPC>();

			var npc = Main.npc.Select(n => n).Where(n => !n.dontCountMe);

			if (this.GetPin("IsHostile").IsConnected() && int.TryParse(this.GetPin("IsHostile").GetValue(), out var hostile))
			{
				npc = npc.Where(n => n.friendly == (hostile == 0));
			}

			if (this.GetPin("IsTownNPC").IsConnected() && int.TryParse(this.GetPin("IsTownNPC").GetValue(), out var townNPC))
			{
				npc = npc.Where(n => n.townNPC == (townNPC == 1));
			}
			
			var area = areaInput.GetArea(this);

			return npc.Where(p => area.Contains(p.position));
		}

		public override List<(string Line, Color Color, float Size)> Debug(Pin pin = null)
		{
			var debug = base.Debug(pin);

			debug.Add(("----------------", Color.Black, WireMod.SmallText));
			debug.AddRange(this.GetNPCs().Select(npc => ($"NPC: {npc.FullName}", Color.Red, WireMod.SmallText)));

			return debug;
		}
	}
}
