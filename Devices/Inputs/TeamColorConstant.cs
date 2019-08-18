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

            this.Settings.Add("Value", ((int)TeamColor.White).ToString());

            this.RightClickHelp = "Right Click to toggle value";

			this.PinLayout = new List<PinDesign>
            {
                new PinDesign("Out", 0, new Point16(0, 1), "teamColor"),
            };
        }

        public string Output(Pin pin = null) => this.Settings["Value"];

        public override void OnRightClick(Pin pin = null)
        {
            if (!int.TryParse(this.Settings["Value"], out var tc)) return;

            this.Settings["Value"] = ((tc + 1) % 6).ToString();

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                WireMod.PacketHandler.SendChangeSetting(256, Main.myPlayer, this.LocationTile.X, this.LocationTile.Y, "Value", this.Settings["Value"]);
            }
        }

        public override Rectangle GetSourceRect(int style = -1)
        {
            if (!int.TryParse(this.Settings["Value"], out var teamcolor)) return default(Rectangle);

            var output = style == -1 ? teamcolor : style + 1;

            return new Rectangle(output * this.Width * 16, 0, this.Width * 16, this.Height * 16);
        }
    }
}
