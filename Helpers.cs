using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;

namespace WireMod
{
    public static class Helpers
    {
        public static Vector2 Drift => new Vector2(Main.screenWidth, Main.screenHeight) * (Main.UIScale - 1) * 0.5f;
        public static Rectangle GetScreenRect() => new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, (int)(Main.screenWidth * Main.UIScale), (int)(Main.screenHeight * Main.UIScale));
        
        public static Texture2D CreateCircle(int diameter)
        {
            var texture = new Texture2D(Main.graphics.GraphicsDevice, diameter, diameter);
            var colorData = new Color[diameter * diameter];

            var radius = diameter / 2f;

            for (var x = 0; x < diameter; x++)
            {
                for (var y = 0; y < diameter; y++)
                {
                    var index = x * diameter + y;
                    if (new Vector2(x - radius, y - radius).LengthSquared() <= radius * radius)
                    {
                        colorData[index] = Color.White;
                    }
                    else
                    {
                        colorData[index] = Color.Transparent;
                    }
                }
            }

            texture.SetData(colorData);
            return texture;
        }

        public static Texture2D CreateRectangle(int width, int height)
        {
            var texture = new Texture2D(Main.graphics.GraphicsDevice, width, height);
            var colorData = new Color[width * height];

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    colorData[x * width + y] = Color.White;
                }
            }

            texture.SetData(colorData);
            return texture;
        }

        public static bool TryParsePoint(string input, out Point16? output)
        {
            if (string.IsNullOrEmpty(input) || !input.Contains(","))
            {
                output = null;
                return false;
            }

            var split = input.Split(',');

            if (!int.TryParse(split[0], out var x) || !int.TryParse(split[1], out var y))
            {
                output = null;
                return false;
            }

            output = new Point16(x, y);
            return true;
        }
    }
}