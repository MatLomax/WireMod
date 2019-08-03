using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
				new PinDesign("In", 0, new Point16(0, 1), "bool", "Hostile Filter"),
				new PinDesign("In", 1, new Point16(1, 0), "int", "Distance"),
				new PinDesign("In", 2, new Point16(2, 1), "bool", "Character Filter"),
				new PinDesign("Out", 0, new Point16(1, 2), "int", "Count"),
			};
		}

		public string Output(Pin pin = null) => this.GetOutput().ToString();

		private int GetOutput()
		{
			if (!int.TryParse(this.Pins["In"][1].GetValue(), out var distance)) return -2;

			return this.GetNPCs(distance).Count();
		}

		private IEnumerable<NPC> GetNPCs(int distance)
		{
			var npc = Main.npc.Select(n => n);
			var pos = this.LocationRect.Location.ToWorldCoordinates();

			if (this.Pins["In"][0].IsConnected() && int.TryParse(this.Pins["In"][0].GetValue(), out var hostile))
			{
				npc = npc.Where(n => n.friendly == (hostile == 0));
			}

			if (this.Pins["In"][2].IsConnected() && int.TryParse(this.Pins["In"][2].GetValue(), out var character))
			{
				npc = npc.Where(n => n.townNPC == (character == 1));
			}

			return npc.Where(n => (pos - n.position).Length() < distance && (pos - n.position).Length() > 1);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (this.LocationRect == default(Rectangle)) return;
			if (!this.Pins["In"][1].IsConnected() || !int.TryParse(this.Pins["In"][1].GetValue(), out var distance)) return;
			if (distance == 0) return;

			var pixels = 16;
			var screenRect = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);

			var deviceWorldRect = new Rectangle((int)(this.LocationRect.X * pixels), (int)(this.LocationRect.Y * pixels), (int)(this.Width * pixels), (int)(this.Height * pixels));
			if (!deviceWorldRect.Intersects(screenRect)) return;

			var deviceScreenRect = new Rectangle(deviceWorldRect.X - screenRect.X, deviceWorldRect.Y - screenRect.Y, deviceWorldRect.Width, deviceWorldRect.Height);

			var circle = Helpers.CreateCircle(distance * 2);
			var pos = new Vector2(deviceScreenRect.X + (deviceScreenRect.Width / 2) - distance, deviceScreenRect.Y + (deviceScreenRect.Height / 2) - distance);

			spriteBatch.Draw(circle, pos, Color.LightGreen * 0.25f);
		}

		public override List<(string Line, Color Color)> Debug(Pin pin = null)
		{
			var debug = base.Debug(pin);

			if (pin == null)
			{
				if (int.TryParse(this.Pins["In"][1].GetValue(), out var distance))
				{
					debug.Add(("----------------", Color.Black));

					debug.AddRange(this.GetNPCs(distance).Select(npc => ($"NPC: {npc.FullName}", Color.Red)));
				}
			}

			return debug;
		}
	}
}
