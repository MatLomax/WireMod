using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using WireMod.Devices;

namespace WireMod
{
    class WireModGlobalTile : GlobalTile
    {


        public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
        {
            foreach(var u in WireMod.Devices.Where(a=> a is IndestructibleArea))
            {
                var item = u as IndestructibleArea;
                if(item.GetTileArea() != null)
                {
                    if (item.GetTileArea().Contains(new Microsoft.Xna.Framework.Vector2(i, j))) return false;
                }
            }
            
            return base.CanKillTile(i, j, type, ref blockDamaged);
        }
    }
}
