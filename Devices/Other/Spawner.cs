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
                new PinDesign("In", 1, new Point16(0, 1), "int", "NPC ID"),
            };
        }

        public void Trigger(Pin pin = null)
        {
            if (!int.TryParse(this.Pins["In"][1].GetValue(), out var id)) return;
            
            if (this._blacklist.Contains(id))
            {
                Main.NewText($"NPC ID {id} is blacklisted");
                return;
            }

            // TODO: Figure out what the difference AI options do
            var npcId = NPC.NewNPC((int)this.LocationWorld.X, (int)this.LocationWorld.Y, id, 0, WorldGen.genRand.Next(100), WorldGen.genRand.Next(100));
            var npc = Main.npc[npcId];
        }

        public override void OnRightClick(Pin pin = null)
        {
            this.Trigger();
        }

        public override List<(string Line, Color Color)> Debug(Pin pin = null)
        {
            var debug = base.Debug(pin);

            if (pin == null)
            {
                debug.Add(("----------------", Color.Black));
                debug.Add(("Right Click to spawn NPC", Color.Red));
            }

            return debug;
        }
    }
}
