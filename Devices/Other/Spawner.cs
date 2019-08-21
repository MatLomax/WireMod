using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace WireMod.Devices
{
    internal class Spawner : Device, ITriggered
    {
        /// <summary>
        /// List of disallowed NPCs
        /// </summary>
        private readonly List<int> _blacklist = new List<int>
        {
            142, // Santa Claus
        };

        public Spawner()
        {
            this.Name = "Spawner";
            this.Width = 2;
            this.Height = 2;
            this.Origin = new Point16(1, 1);

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("In", 0, new Point16(1, 0), "bool", "Trigger"),
                new PinDesign("In", 1, new Point16(0, 1), "int", "NPCID"),
            };
        }

        public void Trigger(Pin pin = null)
        {
            if (!int.TryParse(this.GetPin("NPCID").GetValue(), out var id)) return;
            
            if (this._blacklist.Contains(id))
            {
                Main.NewText($"NPC ID {id} is blacklisted");
                return;
            }

            // TODO: Figure out what the 'Start' argument actually does
            var npcId = NPC.NewNPC((int)this.LocationOriginWorld.X, (int)this.LocationOriginWorld.Y, id, 1);
            var npc = Main.npc[npcId];

            // Do something with NPC?
        }

        public override void OnRightClick(Pin pin = null)
        {
            this.Trigger();
        }

        public override List<(string Line, Color Color, float Size)> Debug(Pin pin = null)
        {
            var debug = base.Debug(pin);

            if (pin == null)
            {
                debug.Add(("----------------", Color.Black, WireMod.SmallText));
                debug.Add(("Right Click to spawn NPC", Color.Red, WireMod.SmallText));
            }

            return debug;
        }
    }
}
