using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;

namespace WireMod.Devices
{
    internal class Repulsor : Device
    {
        public Repulsor()
        {
            this.Name = "Repulsor";
            this.Width = 3;
            this.Height = 2;
            this.Origin = new Point16(1, 1);

            this.Settings.Add("TargetType", TargetTypes.First());

            this.RightClickHelp = $"Right Click to change targetting type ({string.Join("/", TargetTypes)})";

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("In", 0, new Point16(1, 0), "bool", "Armed"),
                new PinDesign("In", 1, new Point16(0, 1), "int", "Velocity"),
                new PinDesign("In", 2, new Point16(2, 1), "int", "Distance"),
            };
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.LocationRect == default(Rectangle)) return;
            if (!this.Pins["In"][2].IsConnected() || !int.TryParse(this.Pins["In"][2].GetValue(), out var maxDistance)) return;
            if (maxDistance == 0) return;
            if (!int.TryParse(this.Pins["In"][0].GetValue(), out var armed)) return;
            if (armed == 0) return;

            var deviceScreenRect = this.LocationScreenRect;
            if (deviceScreenRect == default(Rectangle)) return;

            var circle = Helpers.CreateCircle(maxDistance * 2);
            var pos = new Vector2(deviceScreenRect.X + (deviceScreenRect.Width / 2) - maxDistance, deviceScreenRect.Y + (deviceScreenRect.Height / 2) - maxDistance);

            spriteBatch.Draw(circle, pos, Color.LightBlue * 0.25f);
        }

        public override void Update(GameTime gameTime)
        {
            if (!this.Pins["In"][0].IsConnected()) return;
            if (!int.TryParse(this.Pins["In"][0].GetValue(), out var armed)) return;

            if (armed == 0) return;

            if (!this.Pins["In"][1].IsConnected() || !int.TryParse(this.Pins["In"][1].GetValue(), out var maxVelocity)) return;
            if (!this.Pins["In"][2].IsConnected() || !int.TryParse(this.Pins["In"][2].GetValue(), out var maxDistance)) return;
            
            var entities = new List<Entity>();

            if (this.Settings["TargetType"] == "All" || this.Settings["TargetType"] == "Players")
            {
                entities.AddRange(Main.player.Select(p => new { player = p, distance = (p.position - this.LocationRect.Location.ToWorldCoordinates()).Length() }).OrderBy(p => p.distance).Select(p => p.player));
            }

            if (this.Settings["TargetType"] == "All" || this.Settings["TargetType"] == "NPCs")
            {
                entities.AddRange(Main.npc.Select(p => new { player = p, distance = (p.position - this.LocationRect.Location.ToWorldCoordinates()).Length() }).OrderBy(p => p.distance).Select(p => p.player));
            }

            if (!entities.Any()) return;

            foreach (var entity in entities)
            {
                var direction = entity.position - (this.LocationWorld + this.Origin.ToWorldCoordinates());
                var pLeft = entity.velocity.X < 0;
                var pUp = entity.velocity.Y < 0;

                var distance = direction.Length();

                if (distance > maxDistance) return;

                var power = Math.Max(1, maxDistance / Math.Abs(distance));

                var left = direction.X < 0;
                var up = direction.Y < 0;

                var x = left
                    ? Math.Max(maxVelocity * -1, direction.X * power)
                    : Math.Min(maxVelocity, direction.X * power);

                var y = up
                    ? Math.Max(maxVelocity * -1, direction.Y * power)
                    : Math.Min(maxVelocity, direction.Y * power);

                entity.velocity.X = Math.Max(Math.Min(entity.velocity.X + x, maxVelocity), maxVelocity * -1);
                entity.velocity.Y = Math.Max(Math.Min(entity.velocity.Y + y, maxVelocity), maxVelocity * -1);
            }
        }

        public override void OnRightClick(Pin pin = null)
        {
            this.Settings["TargetType"] = TargetTypes[(TargetTypes.IndexOf(this.Settings["TargetType"]) + 1) % TargetTypes.Count];
        }

        private static readonly List<string> TargetTypes = new List<string>
        {
            "All",
            "Players",
            "NPCs",
        };
    }
}
