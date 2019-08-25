using System.Collections.Generic;
using System.Linq;
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

            this.Settings.Add("Value", "-1");
            this.Settings.Add("Trigger", Triggers.Last());

            this.RightClickHelp = $"Right Click to change Trigger type ({string.Join("/", Triggers)})";

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("In", 0, new Point16(0, 0), "bool"),
                new PinDesign("Out", 0, new Point16(0, 2), "bool"),
            };
        }

        public override void Update(GameTime gameTime)
        {
            if (!this.GetPinIn(0).IsConnected())
            {
                this.Settings["Value"] = "-1";
                return;
            };

            if (!int.TryParse(this.Settings["Value"], out var val) || !int.TryParse(this.GetPinIn(0).GetValue(), out var in0))
            {
                this.Settings["Value"] = "-2";
                return;
            }

            if ((in0 <= 0 && val > 0) || (in0 > 0 && val <= 0))
            {
                if (this.Settings["Trigger"] == Triggers[2] ||                   // Enter and Exit
                    (in0 == 1 && this.Settings["Trigger"] == Triggers[1]) ||     // Enter Only
                    (in0 == 0 && this.Settings["Trigger"] == Triggers[0]))       // Exit Only
                {
                    Wiring.blockPlayerTeleportationForOneIteration = true;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Wiring.TripWire(this.GetPinOut(0).Location.X, this.GetPinOut(0).Location.Y, 1, 1);
                    }
                    else
                    {
                        Wiring.TripWire(this.GetPinOut(0).Location.X, this.GetPinOut(0).Location.Y, 1, 1);
                        WireMod.PacketHandler.SendTripWire(256, Main.myPlayer, this.GetPinOut(0).Location.X, this.GetPinOut(0).Location.Y);
                    }
                }
            }

            this.Settings["Value"] = in0.ToString();
        }

        public string Output(Pin pin = null) => this.Settings["Value"];

        public override void OnRightClick(Pin pin = null)
        {
            this.Settings["Trigger"] = Triggers[(Triggers.IndexOf(this.Settings["Trigger"]) + 1) % Triggers.Count];
        }

        private static readonly List<string> Triggers = new List<string>
        {
            "Exit Only",
            "Enter Only",
            "Enter and Exit"
        };
    }
}
