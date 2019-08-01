using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using WireMod.UI;

namespace WireMod.Devices
{
    internal class NumberConstant : Device, IOutput
    {
        public NumberConstant()
        {
            this.Name = "Constant Number";

            this.ValueType = "int";
            this.Value = "0";

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("Out", 0, new Point16(0, 1), "int"),
            };
        }

        public string Output(Pin pin = null) => this.Value;

        public override void OnRightClick(Pin pin = null)
        {
            // TODO: Take user input
            var input = new UserInputUI(this.Value);
            input.OnSave += (s, e) =>
            {
                if (!int.TryParse(input.Value, out var value))
                {
                    Main.NewText($"Could not parse '{input.Value}' as an integer");
                    return;
                }

                this.Value = input.Value;

                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    WireMod.PacketHandler.SendChangeValue(256, Main.myPlayer, this.LocationTile.X, this.LocationTile.Y, this.Value);
                }
            };

            WireMod.Instance.UserInputUserInterface.SetState(input);
            UserInputUI.Visible = true;
        }

        public override List<(string Line, Color Color)> Debug(Pin pin = null)
        {
            var debug = base.Debug(pin);

            if (pin == null)
            {
                debug.Add(("----------------", Color.Black));
                debug.Add(("Right Click to change value", Color.Red));
            }

            return debug;
        }
    }
}
