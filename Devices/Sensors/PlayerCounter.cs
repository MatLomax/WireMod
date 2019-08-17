using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
				new PinDesign("In", 0, new Point16(1, 0), "int", "Distance"),
				new PinDesign("In", 1, new Point16(0, 1), "teamColor", "Team Color Filter"),
				new PinDesign("Out", 0, new Point16(1, 2), "int", "Count"),
			};
		}

		public string Output(Pin pin = null) => this.GetOutput().ToString();

		private int GetOutput()
		{
			if (!int.TryParse(this.Pins["In"][0].GetValue(), out var distance)) return -2;

			return this.GetPlayers(distance).Count();
		}

		private IEnumerable<Player> GetPlayers(int distance)
		{
			var players = Main.player.Select(p => p);
			var pos = this.LocationWorld + new Vector2(8, 8);

			if (this.Pins["In"][1].IsConnected())
			{
				var team = TeamColor.White;
				if (int.TryParse(this.Pins["In"][1].GetValue(), out var tc))
				{
					team = (TeamColor)tc;
				}
				players = players.Where(p => p.team == (int)team);
			}
			
			return players.Where(p => (pos - p.position).Length() < distance && (pos - p.position).Length() > 1);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (this.LocationRect == default(Rectangle)) return;
			if (!this.Pins["In"][0].IsConnected() || !int.TryParse(this.Pins["In"][0].GetValue(), out var distance)) return;
			if (distance < 1) return;

			if (!this.LocationWorldRect.Intersects(WireMod.Instance.GetScreenRect())) return;

			var deviceScreenRect = this.LocationScreenRect;
			var circle = Helpers.CreateCircle(distance * 2);
			var pos = new Vector2(deviceScreenRect.X + (deviceScreenRect.Width / 2) - distance, deviceScreenRect.Y + (deviceScreenRect.Height / 2) - distance);

			spriteBatch.Draw(circle, pos, Color.LightGreen * 0.25f);
		}

		public override List<(string Line, Color Color, float Size)> Debug(Pin pin = null)
		{
			var debug = base.Debug(pin);

			if (pin == null && int.TryParse(this.Pins["In"][0].GetValue(), out var distance))
			{
				debug.Add(("----------------", Color.Black, WireMod.SmallText));

				debug.AddRange(this.GetPlayers(distance).Select(player => ($"Player: {player.name}", Color.Red, WireMod.SmallText)));
			}

			return debug;
		}
	}
}
