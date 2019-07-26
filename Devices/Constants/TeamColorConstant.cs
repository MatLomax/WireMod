using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace WireMod.Devices
{
    internal class TeamColorConstant : Device, IOutput
    {
        public TeamColorConstant()
        {
            this.Name = "Team Color Constant";
            this.Value = ((int)TeamColor.White).ToString();
            this.ValueType = "TeamColor";

			this.PinLayout = new List<PinDesign>
            {
                new PinDesign("Out", 0, new Point16(0, 1), "teamColor"),
            };
        }

        public string Output(Pin pin = null) => this.Value;

        public override void OnRightClick(Pin pin = null)
        {
            if (!int.TryParse(this.Value, out var tc)) return;

            this.Value = ((tc + 1) % 6).ToString();

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                WireMod.PacketHandler.SendChangeValue(256, Main.myPlayer, this.Location.X, this.Location.Y, this.Value);
            }
        }

        public override Rectangle GetSourceRect(int style = -1)
        {
            if (!int.TryParse(this.Value, out var tc)) return default(Rectangle);

            var output = style == -1 ? tc : style + 1;

            return new Rectangle(output * (this.Width * 16), 0, this.Width * 16, this.Height * 16);
        }

        public override List<(string Line, Color Color)> Debug()
        {
            var debug = base.Debug();
            debug.Add(("----------------", Color.Black));
            debug.Add(("Right Click to toggle value", Color.Red));
            return debug;
        }
    }
}
