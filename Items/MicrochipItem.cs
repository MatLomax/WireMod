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
    }
}
