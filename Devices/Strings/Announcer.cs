using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;

namespace WireMod.Devices
{
    internal class Announcer : Device, ITriggered
    {
        private readonly Color _textColor = new Color(255, 255, 20);

        public Announcer()
        {
            this.Name = "Announcer";
            this.Width = 3;
            this.Height = 2;
            this.Origin = new Point16(1, 1);

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("In", 0, new Point16(1, 0), "string"),
                new PinDesign("In", 1, new Point16(0, 1), "bool", "Trigger"),
                new PinDesign("In", 2, new Point16(2, 1), "teamColor", "TeamColorFilter"),
            };
        }

        public void Trigger(Pin pin = null)
        {
            if (!this.GetPinIn(0).IsConnected()) return;

            var message = this.GetPinIn(0).GetValue();

            switch (Main.netMode)
            {
                case NetmodeID.SinglePlayer:
                    Main.NewText(message, this._textColor);
                    break;
                case NetmodeID.Server:
                    NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(message), this._textColor);
                    break;
            }
        }
    }
}
