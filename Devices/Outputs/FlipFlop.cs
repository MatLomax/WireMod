using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace WireMod.Devices
{
    internal class FlipFlop : Device, IOutput
    {
        public FlipFlop()
        {
            this.Name = "Flip Flop";
            this.Width = 1;
            this.Height = 3;
            this.Origin = new Point16(0, 1);

            this.Value = "-1";
            this.ValueType = "int";

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("In", 0, new Point16(0, 0), "bool"),
                new PinDesign("Out", 0, new Point16(0, 2), "bool"),
            };
        }

        public string Output(Pin pin = null) => this.GetOutput().ToString();

        private int GetOutput()
        {
            if (!int.TryParse(this.Value, out var val)) return -1;
            if (!this.Pins["In"][0].IsConnected()) return -1;
            if (this.Pins["In"][0].DataType != "bool") return -1;
            if (!int.TryParse(this.Pins["In"][0].GetValue(), out var in0)) return -1;

            if ((in0 <= 0 && val > 0) || (in0 > 0 && val <= 0))
            {
                Wiring.blockPlayerTeleportationForOneIteration = true;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Wiring.TripWire(this.Pins["Out"][0].Location.X, this.Pins["Out"][0].Location.Y, 1, 1);
                }
                else
                {
                    Wiring.TripWire(this.Pins["Out"][0].Location.X, this.Pins["Out"][0].Location.Y, 1, 1);
                    WireMod.PacketHandler.SendTripWire(256, Main.myPlayer, this.Pins["Out"][0].Location.X, this.Pins["Out"][0].Location.Y);
                }
            }

            this.Value = in0.ToString();
            return in0;
        }

        public override List<(string Line, Color Color)> Debug(Pin pin = null)
        {
            var debug = base.Debug(pin);

            debug.Add(("------------------------", Color.Black));
            debug.Add(($"Stored Value: {this.Value}", Color.Purple));

            return debug;
        }
    }
}
