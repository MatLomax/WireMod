using Terraria.ID;
using Terraria.ModLoader;

namespace WireMod.Items
{
    internal class MicrochipItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            item.SetNameOverride("Microchip");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.maxStack = 99;
            item.consumable = true;
            item.value = 50;
        }

        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Wire);
            recipe.AddIngredient(ItemID.Actuator);
            recipe.AddIngredient(ItemID.CopperCoin);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
