using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace WireMod
{
    public class Helpers
    {
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
    }
}