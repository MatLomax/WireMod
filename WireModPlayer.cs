using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using WireMod.Devices;
using WireMod.UI;

namespace WireMod
{
	public class WireModPlayer : ModPlayer
	{
		public Device PlacingDevice;

		//public bool ElectronicsManualVisible;
		//public bool ElectronicsVisionVisible;
		//public bool DebuggerVisible;
		public bool ShowPreview;
		public int ToolCategoryMode = 0;
		public int ToolMode = 0;

		//internal UserInterface ElectronicsManualUserInterface = new UserInterface();
		//internal UserInterface ElectronicsVisionUserInterface = new UserInterface();
		//internal UserInterface DebuggerUserInterface = new UserInterface();

		//internal ElectronicsManualUI ElectronicsManualUI = new ElectronicsManualUI();
		//internal ElectronicsVisionUI ElectronicsVisionUI = new ElectronicsVisionUI();

		//public override void Load(TagCompound tag)
		//{
		//	mod.Logger.Info($"Player loading: {this.Name}");

		//	this.ElectronicsManualUserInterface = new UserInterface();
		//	this.ElectronicsVisionUserInterface = new UserInterface();
		//	this.ElectronicsManualUI = new ElectronicsManualUI();
		//	this.ElectronicsVisionUI = new ElectronicsVisionUI();
		//	this.DebuggerUserInterface = new UserInterface();

		//	if (Main.netMode == NetmodeID.Server) return;

		//	mod.Logger.Info("Loading UIs...");

		//	this.ElectronicsManualUI.Activate();
		//	this.ElectronicsManualUserInterface.SetState(this.ElectronicsManualUI);

		//	this.ElectronicsVisionUI.Activate();
		//	this.ElectronicsVisionUserInterface.SetState(this.ElectronicsVisionUI);
		//}

		public override void OnEnterWorld(Player p)
		{
			base.OnEnterWorld(p);

			mod.Logger.Info($"Player entered world: {p.name}");

			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				WireMod.PacketHandler.SendRequest(256, Main.myPlayer);
			}

			var modPlayer = p.GetModPlayer<WireModPlayer>();

			//this.ElectronicsManualUI.Activate();
			//this.ElectronicsManualUserInterface.SetState(this.ElectronicsManualUI);

			//this.ElectronicsVisionUI.Activate();
			//this.ElectronicsVisionUserInterface.SetState(this.ElectronicsVisionUI);
		}
	}
}