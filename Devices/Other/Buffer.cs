using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;

namespace WireMod.Devices
{
    internal class Buffer : Device
    {
        public Buffer()
        {
            this.Name = "Buffer";
            this.Width = 3;
            this.Height = 2;
            this.Origin = new Point16(1, 1);
            
            this.Settings.Add("TargetType", "Players");

            this.RightClickHelp = $"Right Click to change targetting type ({string.Join("/", TargetTypes)})";

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("In", 0, new Point16(1, 0), "int", "Buff ID"),
                new PinDesign("In", 1, new Point16(0, 1), "int", "Duration"),
                new PinDesign("In", 2, new Point16(2, 1), "int", "Distance"),
            };
        }

        public override void Update(GameTime gameTime)
        {
            if (!this.Pins["In"][0].IsConnected()) return;
            if (!int.TryParse(this.Pins["In"][0].GetValue(), out var buffId)) return;
            if (buffId == 0) return;

            if (!int.TryParse(this.Pins["In"][2].GetValue(), out var maxDistance)) return;

            if (!int.TryParse(this.Pins["In"][1].GetValue(), out var duration)) duration = 10;

            duration *= 100;

            var players = Main.player.Where(p => (p.position - this.LocationWorld).Length() < maxDistance).ToList();
            var npcs = Main.npc.Where(p => (p.position - this.LocationWorld).Length() < maxDistance).ToList();

            switch (this.Settings["TargetType"])
            {
                case "Players" when !players.Any():
                    return;
                case "NPCs" when !npcs.Any():
                    return;
                case "NPCs":
                    //Main.NewText($"NPC Buff: {buffId} ({duration} seconds)");
                    npcs.ForEach(n => n.AddBuff(buffId, duration));
                    break;
                case "Players":
                    //Main.NewText($"Player Buff: {buffId} ({duration} seconds)");
                    players.ForEach(p => p.AddBuff(buffId, duration));
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.LocationRect == default(Rectangle)) return;
            if (!this.Pins["In"][2].IsConnected() || !int.TryParse(this.Pins["In"][2].GetValue(), out var maxDistance)) return;
            if (maxDistance == 0) return;
            if (!int.TryParse(this.Pins["In"][0].GetValue(), out var buffId)) return;
            if (buffId == 0) return;

            var pixels = 16;
            var screenRect = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
            
            var deviceWorldRect = new Rectangle((int)this.LocationWorld.X, (int)this.LocationWorld.Y, (int)(this.Width * pixels), (int)(this.Height * pixels));
            if (!deviceWorldRect.Intersects(screenRect)) return;

            var deviceScreenRect = new Rectangle(deviceWorldRect.X - screenRect.X, deviceWorldRect.Y - screenRect.Y, deviceWorldRect.Width, deviceWorldRect.Height);

            var circle = Helpers.CreateCircle(maxDistance * 2);
            var pos = new Vector2(deviceScreenRect.X + (deviceScreenRect.Width / 2) - maxDistance, deviceScreenRect.Y + (deviceScreenRect.Height / 2) - maxDistance);

            spriteBatch.Draw(circle, pos, Color.Red * 0.25f);
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
