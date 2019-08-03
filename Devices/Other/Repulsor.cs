﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;

namespace WireMod.Devices
{
    internal class Repulsor : Device, IOutput
    {
        public Repulsor()
        {
            this.Name = "Repulsor";
            this.Width = 3;
            this.Height = 3;
            this.Origin = new Point16(1, 1);
            
            this.Settings.Add("TargetType", "Players");

            this.RightClickHelp = $"Right Click to change targetting type ({string.Join("/", TargetTypes)})";

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("In", 0, new Point16(1, 0), "bool", "Armed"),
                new PinDesign("In", 1, new Point16(0, 1), "int", "Velocity"),
                new PinDesign("In", 2, new Point16(2, 1), "int", "Distance"),
                new PinDesign("Out", 0, new Point16(1, 2), "bool", "IsActive"),
            };
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.LocationRect == default(Rectangle)) return;
            if (!this.Pins["In"][2].IsConnected() || !int.TryParse(this.Pins["In"][2].GetValue(), out var maxDistance)) return;
            if (maxDistance == 0) return;
            if (!int.TryParse(this.Pins["In"][0].GetValue(), out var armed)) return;
            if (armed == 0) return;

            var pixels = 16;
            var screenRect = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
            
            var deviceWorldRect = new Rectangle((int)(this.LocationRect.X * pixels), (int)(this.LocationRect.Y * pixels), (int)(this.Width * pixels), (int)(this.Height * pixels));
            if (!deviceWorldRect.Intersects(screenRect)) return;

            var deviceScreenRect = new Rectangle(deviceWorldRect.X - screenRect.X, deviceWorldRect.Y - screenRect.Y, deviceWorldRect.Width, deviceWorldRect.Height);

            var circle = Helpers.CreateCircle(maxDistance * 2);
            var pos = new Vector2(deviceScreenRect.X + (deviceScreenRect.Width / 2) - maxDistance, deviceScreenRect.Y + (deviceScreenRect.Height / 2) - maxDistance);

            spriteBatch.Draw(circle, pos, Color.LightBlue * 0.25f);
        }
        
        public string Output(Pin pin = null) => this.GetOutput().ToString();

        private int GetOutput()
        {
            if (!this.Pins["In"][0].IsConnected()) return -1;
            if (!int.TryParse(this.Pins["In"][0].GetValue(), out var armed)) return -2;

            if (armed == 0) return 0;

            var player = Main.player.OrderBy(p => (p.position - this.LocationRect.Location.ToWorldCoordinates()).Length()).FirstOrDefault();
            var npc = Main.npc.OrderBy(p => (p.position - this.LocationRect.Location.ToWorldCoordinates()).Length()).FirstOrDefault();

            switch (this.Settings["TargetType"])
            {
                case "Players" when player == null:
                    return 0;
                case "NPCs" when npc == null:
                    return 0;
            }

            Vector2 direction;
            bool pLeft;
            bool pUp;
            switch (this.Settings["TargetType"])
            {
                case "NPCs":
                    direction = npc.position - this.LocationRect.Location.ToWorldCoordinates();
                    pLeft = npc.velocity.X < 0;
                    pUp = npc.velocity.Y < 0;
                    break;
                case "Players":
                default:
                    direction = player.position - this.LocationRect.Location.ToWorldCoordinates();
                    pLeft = player.velocity.X < 0;
                    pUp = player.velocity.Y < 0;
                    break;
            }

            var distance = direction.Length();

            if (!this.Pins["In"][1].IsConnected() || !int.TryParse(this.Pins["In"][1].GetValue(), out var maxVelocity)) return -2;
            if (!this.Pins["In"][2].IsConnected() || !int.TryParse(this.Pins["In"][2].GetValue(), out var maxDistance)) return -2;

            if (distance > maxDistance) return 0;

            var power = Math.Max(1, maxDistance / Math.Abs(distance));

            var left = direction.X < 0;
            var up = direction.Y < 0;

            // Only trigger if target is moving towards the repulsor (prevents multiple triggers)
            if (left && pLeft && up && pUp
                || left && pLeft && !up && !pUp
                || !left && !pLeft && up && pUp
                || !left && !pLeft && !up && !pUp) return 0;

            var x = left
                ? Math.Max(maxVelocity * -1, direction.X * power)
                : Math.Min(maxVelocity, direction.X * power);

            var y = up
                ? Math.Max(maxVelocity * -1, direction.Y * power)
                : Math.Min(maxVelocity, direction.Y * power);

            switch (this.Settings["TargetType"])
            {
                case "NPCs":
                    npc.velocity.X += x;
                    npc.velocity.Y += y;
                    break;
                case "Players":
                default:
                    player.velocity.X += x;
                    player.velocity.Y += y;
                    break;
            }
            

            return 1;
        }

        public override void OnRightClick(Pin pin = null)
        {
            this.Settings["TargetType"] = TargetTypes[(TargetTypes.IndexOf(this.Settings["TargetType"]) + 1) % TargetTypes.Count];
        }

        private static readonly List<string> TargetTypes = new List<string>
        {
            "Players",
            "NPCs",
        };
    }
}
