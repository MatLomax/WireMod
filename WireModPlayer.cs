using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WireMod.Devices;

namespace WireMod
{
	public class WireModPlayer : ModPlayer
	{
		public Device PlacingDevice;
		public Pin ConnectingPin;
		public Wire PlacingWire;

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

		public override TagCompound Save()
		{
			return new TagCompound
			{
				["menuPosX"] = WireMod.Instance.ElectronicsManualUI.Panel.Left.Pixels,
				["menuPosY"] = WireMod.Instance.ElectronicsManualUI.Panel.Top.Pixels,
			};
		}
		public override void Load(TagCompound tag)
		{
			WireMod.Instance.ElectronicsManualUI.Panel.Left.Set(tag.GetFloat("menuPosX"), 0f);
			WireMod.Instance.ElectronicsManualUI.Panel.Top.Set(tag.GetFloat("menuPosY"), 0f);
		}
	}
}