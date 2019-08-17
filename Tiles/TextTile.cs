using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;
using WireMod.Items;

namespace WireMod.Tiles
{
    public class TextTile : ModTile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            Main.tileFrameImportant[Type] = true;
            //Main.tileBlockLight[Type] = false;
            //Main.tileLighted[Type] = true;
            //Main.tileNoAttach[Type] = true;

            //this.dustType = mod.DustType("Sparkle");
            //this.drop = mod.ItemType("TextTileItem");

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.Origin = Point16.Zero;
            TileObjectData.newTile.CoordinateHeights = new [] { 16 };

            for (var i = 0; i < 3; i++)
            {
                TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
                TileObjectData.addAlternate(i);
            }

            TileObjectData.addTile(Type);
        }
    }
}