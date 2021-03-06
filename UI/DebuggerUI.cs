﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.UI;
using Terraria.UI.Chat;
using WireMod.Devices;

namespace WireMod.UI
{
    internal class DebuggerUI : UIState
    {
		public static bool Visible { get; set; }
        
        public readonly Device Device;
        public readonly Pin Pin;

        public DebuggerUI(Device device, Pin pin = null)
        {
            this.Device = device;
            this.Pin = pin;
        }
        
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            var lines = this.Device.Debug(this.Pin);
            if (lines == null) return;

            const int padding = 10;
            const int border = 2;

            var width = (int)(lines.Max(t => (int)ChatManager.GetStringSize(Main.fontMouseText, t.Line, Vector2.One).X * t.Size) + (padding * 2));
            var height = (int)lines.ToList().Sum(t => Main.fontMouseText.MeasureString(t.Line).Y) + padding * 2;

            var x = 100;
            var y = Main.screenHeight - height - 30;

            spriteBatch.Draw(Main.magicPixel, new Rectangle(x, y, width + border * 2, height + border * 2), null, Color.Black, 0f, new Vector2(0, 0), SpriteEffects.None, 0f);
            spriteBatch.Draw(Main.magicPixel, new Rectangle(x + border, y + border, width, height), null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0f);

            var i = 0;
            foreach ((string line, Color color, float size) in lines)
            {
                var lineHeight = Main.fontMouseText.MeasureString(line).Y;
                Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, line, x + padding, y + padding + (lineHeight * i) + padding, color, Color.White, new Vector2(0.3f), size);
                i++;
            }
        }
    }
}
