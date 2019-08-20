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
				new PinDesign("In", 1, new Point16(0, 1), "bool", "Hostile Filter"),
				new PinDesign("In", 2, new Point16(2, 1), "bool", "TownNPC Filter"),
				new PinDesign("Out", 0, new Point16(1, 2), "int", "Count"),
			};
		}

		public string Output(Pin pin = null) => this.GetOutput().ToString();

		private int GetOutput()
		{
			if (!this.Pins["In"][0].IsConnected()) return -1;

			return this.GetNPCs().Count();
		}

		private IEnumerable<NPC> GetNPCs()
		{
			var npc = Main.npc.Select(n => n).Where(n => n.life > 0 && !n.dontCountMe);
			var pos = this.LocationWorld + new Vector2(8, 8);

			if (this.Pins["In"][1].IsConnected() && int.TryParse(this.Pins["In"][1].GetValue(), out var hostile))
			{
				npc = npc.Where(n => n.friendly == (hostile == 0));
			}

			if (this.Pins["In"][2].IsConnected() && int.TryParse(this.Pins["In"][2].GetValue(), out var character))
			{
				npc = npc.Where(n => n.townNPC == (character == 1));
			}

			if (!this.Pins["In"][0].IsConnected()) return new List<NPC>();

			var input = ((PinIn)this.Pins["In"][0]).ConnectedPin.Device;
			if (!(input is AreaInput)) return new List<NPC>();

			var area = ((AreaInput)input).GetArea(this);

			return npc.Where(p => area.Contains(p.position));
		}

		public override List<(string Line, Color Color, float Size)> Debug(Pin pin = null)
		{
			var debug = base.Debug(pin);

			if (pin == null)
			{
				debug.Add(("----------------", Color.Black, WireMod.SmallText));
				debug.AddRange(this.GetNPCs().Select(npc => ($"NPC: {npc.FullName}", Color.Red, WireMod.SmallText)));
			}

			return debug;
		}
	}
}
