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

        private readonly string Title;
        private readonly List<(string Line, Color Color)> Text;

        public DebuggerUI(string title, List<(string, Color)> text = null)
        {
            this.Title = title;
            this.Text = text;
        }
        
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            var offset = new Vector2(16, 16);
            
            const int padding = 10;
            const int border = 2;

            var width = Text?.Max(t => (int)ChatManager.GetStringSize(Main.fontMouseText, t.Line, Vector2.One).X) + padding * 2 ?? 100;
            var height = this.Text == null ? 100 : (int)this.Text.ToList().Sum(t => Main.fontMouseText.MeasureString(t.Line).Y) + padding * 2;
            //var zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            var x = Main.MouseScreen.X - (width + offset.X);
            var y = Main.MouseScreen.Y + offset.Y;

            spriteBatch.Draw(Main.magicPixel, new Rectangle((int)x, (int)y, width + border * 2, height + border * 2), null, Color.Black, 0f, new Vector2(0, 0), SpriteEffects.None, 0f);
            spriteBatch.Draw(Main.magicPixel, new Rectangle((int)x + border, (int)y + border, width, height), null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 0f);

            var i = 0;
            foreach ((string line, Color color) in this.Text)
            {
                var lineHeight = Main.fontMouseText.MeasureString(line).Y;
                Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, line, x + padding, y + padding + (lineHeight * i) + padding, color, Color.White, new Vector2(0.3f), i > 0 ? 0.75f : 1f);
                i++;
            }
        }
    }
}
