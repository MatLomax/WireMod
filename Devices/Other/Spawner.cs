using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace WireMod.Devices
{
    internal class Spawner : Device, IOutput
    {
        /// <summary>
        /// List of disallowed NPCs
        /// </summary>
        private readonly List<int> _blacklist = new List<int>
        {

        };

        public Spawner()
        {
            this.Name = "Spawner";
            this.Width = 2;
            this.Height = 3;
            this.Origin = new Point16(1, 1);

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("In", 0, new Point16(1, 0), "bool", "Trigger"),
                new PinDesign("In", 1, new Point16(0, 1), "int", "NPC ID"),
                new PinDesign("Out", 0, new Point16(1, 2), "bool", "IsActive"),
            };
        }
		
        public string Output(Pin pin = null) => this.GetOutput().ToString();

        private int GetOutput()
        {
            if (!this.Pins["In"][0].IsConnected()) return -1;
            if (!int.TryParse(this.Pins["In"][0].GetValue(), out var trigger)) return -2;
            if (!int.TryParse(this.Pins["In"][1].GetValue(), out var id)) return -2;

            if (trigger == 0) return 0;
            
            this.Trigger(id);

            return 1;
        }

        private void Trigger(int id)
        {
            if (this._blacklist.Contains(id))
            {
                Main.NewText($"NPC ID {id} is blacklisted");
                return;
            }

            var npcId = NPC.NewNPC(this.LocationWorld.X, this.LocationWorld.Y, id, 1, WorldGen.genRand.Next(10), WorldGen.genRand.Next(10));
            var npc = Main.npc[npcId];
        }

        public override void OnRightClick(Pin pin = null)
        {
            if (!int.TryParse(this.Pins["In"][1].GetValue(), out var id)) return;

			this.Trigger(id);
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
