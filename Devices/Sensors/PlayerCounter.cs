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
				new PinDesign("In", 0, new Point16(1, 0), "area", "Area"),
				new PinDesign("In", 1, new Point16(0, 1), "teamColor", "Team Color Filter"),
				new PinDesign("Out", 0, new Point16(1, 2), "int", "Count"),
			};
		}

		public string Output(Pin pin = null) => this.GetOutput().ToString();

		private int GetOutput()
		{
			if (!this.Pins["In"][0].IsConnected()) return -1;
			if (!this.Pins["In"][0].IsConnected()) return -1;
			if (!Helpers.TryParseArea(this.Pins["In"][0].GetValue(), out var area) || !area.HasValue) return -2;

			return this.GetPlayers(area.Value).Count();
		}

		private IEnumerable<Player> GetPlayers((string AreaType, int Radius) area)
		{
			var players = Main.player.Select(p => p);
			
			if (this.Pins["In"][1].IsConnected())
			{
				var team = TeamColor.White;
				if (int.TryParse(this.Pins["In"][1].GetValue(), out var tc))
				{
					team = (TeamColor)tc;
				}
				players = players.Where(p => p.team == (int)team);
			}

			Vector2 pos;
			var connDev = ((PinIn)this.Pins["In"][0]).ConnectedPin.Device;
			if (connDev.Pins["In"][1].IsConnected() && Helpers.TryParsePoint(connDev.Pins["In"][1].GetValue(), out var point) && point.HasValue)
			{
				pos = point.Value.ToWorldCoordinates();
			}
			else
			{
				pos = this.LocationOriginWorld;
			}

			if (area.AreaType == "Square")
			{
				var rect = new Rectangle((int)pos.X - area.Radius, (int)pos.Y - area.Radius, area.Radius * 2, area.Radius * 2);
				return players.Where(p => rect.Contains(p.position.ToPoint()));
			}

			return players.Where(p => (pos - p.position).Length() < area.Radius && (pos - p.position).Length() > 1);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (this.LocationRect == default(Rectangle)) return;
			if (!this.LocationWorldRect.Intersects(WireMod.Instance.GetScreenRect())) return;

			if (!this.Pins["In"][0].IsConnected() || !Helpers.TryParseArea(this.Pins["In"][0].GetValue(), out var area) || !area.HasValue) return;
			
			//var deviceScreenRect = this.LocationScreenRect;
			var overlay = area.Value.AreaType == "Circle" ? Helpers.CreateCircle(area.Value.Radius * 2) : Helpers.CreateRectangle(area.Value.Radius * 2, area.Value.Radius * 2);

			Vector2 pos;
			var connDev = ((PinIn)this.Pins["In"][0]).ConnectedPin.Device;
			if (connDev.Pins["In"][1].IsConnected() && Helpers.TryParsePoint(connDev.Pins["In"][1].GetValue(), out var point) && point.HasValue)
			{
				pos = point.Value.ToWorldCoordinates() - Main.screenPosition;
			}
			else
			{
				pos = this.LocationOriginScreen;
			}

			pos -= overlay.Size() / 2;

			spriteBatch.Draw(overlay, pos, Color.LightGreen * 0.25f);
		}

		//public override List<(string Line, Color Color, float Size)> Debug(Pin pin = null)
		//{
		//	var debug = base.Debug(pin);

		//	if (!this.Pins["In"][0].IsConnected() || !Helpers.TryParseArea(this.Pins["In"][0].GetValue(), out var area) || !area.HasValue) return;
		//	if (pin == null && int.TryParse(this.Pins["In"][0].GetValue(), out var distance))
		//	{
		//		debug.Add(("----------------", Color.Black, WireMod.SmallText));

		//		debug.AddRange(this.GetPlayers(distance).Select(player => ($"Player: {player.name}", Color.Red, WireMod.SmallText)));
		//	}

		//	return debug;
		//}
	}
}
