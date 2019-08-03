using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WireMod.Devices;
using WireMod.UI;

namespace WireMod
{
	public class WireModPlayer : ModPlayer
	{
		public Device PlacingDevice;
		public Pin ConnectingPin;

		public bool ShowPreview;
		public int ToolCategoryMode = 0;
		public int ToolMode = 0;

		public ElectronicsManualUI ElectronicsManualUI { get; set; }
		public ElectronicsVisionUI ElectronicsVisionUI { get; set; }
		
		public override void OnEnterWorld(Player p)
		{
			base.OnEnterWorld(p);

			this.ElectronicsManualUI = new ElectronicsManualUI();
			this.ElectronicsVisionUI = new ElectronicsVisionUI();
			
			// Send sync request
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				WireMod.PacketHandler.SendRequest(256, Main.myPlayer);
			}
		}
	}
}