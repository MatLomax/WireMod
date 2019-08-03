using System.Collections.Generic;
using System.Linq;
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

        public string Output(Pin pin = null) => this.GetOutput().ToString();

        private int GetOutput()
        {
            if (!int.TryParse(this.Settings["Value"], out var val)) return -1;
            if (!this.Pins["In"][0].IsConnected()) return -1;
            if (this.Pins["In"][0].DataType != "bool") return -1;
            if (!int.TryParse(this.Pins["In"][0].GetValue(), out var in0)) return -1;

            if ((in0 <= 0 && val > 0) || (in0 > 0 && val <= 0))
            {
                if (this.Settings["Trigger"] == Triggers[2] ||                   // Enter and Exit
                    (in0 == 1 && this.Settings["Trigger"] == Triggers[1]) ||     // Enter
                    (in0 == 0 && this.Settings["Trigger"] == Triggers[0]))       // Exit
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
            }

            this.Settings["Value"] = in0.ToString();
            return in0;
        }

        public override void OnRightClick(Pin pin = null)
        {
            this.Settings["Trigger"] = Triggers[(Triggers.IndexOf(this.Settings["Trigger"]) + 1) % Triggers.Count];
        }

        private static readonly List<string> Triggers = new List<string>
        {
            "Exit",
            "Enter",
            "Enter and Exit"
        };
    }
}
