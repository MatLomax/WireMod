using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace WireMod.Tiles
{
    public class TextTile : ModTile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.Origin = Point16.Zero;
            TileObjectData.newTile.CoordinateHeights = new [] { 16 };

            TileObjectData.addTile(Type);
        }
    }
}