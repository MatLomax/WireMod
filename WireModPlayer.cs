using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WireMod.Devices;

namespace WireMod
{
	public class WireModPlayer : ModPlayer
	{
		public Device PlacingDevice;

		public bool ShowPreview;
		public int ToolCategoryMode = 0;
		public int ToolMode = 0;
		
		public override void OnEnterWorld(Player p)
		{
			base.OnEnterWorld(p);
			
			// Send sync request
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				WireMod.PacketHandler.SendRequest(256, Main.myPlayer);
			}
		}
	}
}