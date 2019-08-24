using System.Linq;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using WireMod.Devices;

namespace WireMod
{
    internal class WireModGlobalTile : GlobalTile
    {
        public override bool CanPlace(int i, int j, int type)
        {
            var protect = WireMod.Devices.Any(d => d is ProtectArea dev && (((TileArea)dev.GetProtectPlaceArea())?.Contains(new Vector2(i, j)) ?? false));

            return !protect && base.CanPlace(i, j, type);
        }

        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            var protect = WireMod.Devices.Any(d => d is ProtectArea dev && (((TileArea)dev.GetProtectDestroyArea())?.Contains(new Vector2(i, j)) ?? false));

            return !protect && base.CanKillTile(i, j, type, ref blockDamaged);
        }
    }
}
