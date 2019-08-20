using Terraria.ID;
using Terraria.ModLoader;

namespace WireMod.Items
{
    internal class TextTileItem : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.maxStack = 99;
            item.consumable = true;
            item.autoReuse = true;
            item.useStyle = 1;
            item.value = 50;
            item.createTile = mod.TileType("TextTile");
        }

        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Wire);
            recipe.AddIngredient(ItemID.StoneBlock);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
