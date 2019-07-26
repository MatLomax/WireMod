using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.UI;
using Terraria.UI.Chat;

namespace WireMod.UI
{
    internal class DebuggerUI : UIState
    {
		public static bool Visible { get; set; }
        
        private readonly List<(string Line, Color Color)> _lines;

        public DebuggerUI(List<(string, Color)> lines = null)
        {
            this._lines = lines;
        }
        
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            if (this._lines == null) return;

            var offset = new Vector2(16, 16);
            
            const int padding = 10;
            const int border = 2;

            var width = this._lines.Max(t => (int)ChatManager.GetStringSize(Main.fontMouseText, t.Line, Vector2.One).X) + padding * 2;
            var height = (int)this._lines.ToList().Sum(t => Main.fontMouseText.MeasureString(t.Line).Y) + padding * 2;

            var x = Main.MouseScreen.X - (width + offset.X);
            var y = Main.MouseScreen.Y + offset.Y;

            spriteBatch.Draw(Main.magicPixel, new Rectangle((int)x, (int)y, width + border * 2, height + border * 2), null, Color.Black, 0f, new Vector2(0, 0), SpriteEffects.None, 0f);
            spriteBatch.Draw(Main.magicPixel, new Rectangle((int)x + border, (int)y + border, width, height), null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0f);

            var i = 0;
            foreach ((string line, Color color) in this._lines)
            {
                var lineHeight = Main.fontMouseText.MeasureString(line).Y;
                Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, line, x + padding, y + padding + (lineHeight * i) + padding, color, Color.White, new Vector2(0.3f), i > 0 ? 0.75f : 1f);
                i++;
            }
        }
    }
}
