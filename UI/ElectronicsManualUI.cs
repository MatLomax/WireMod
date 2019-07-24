using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Chat;

namespace WireMod.UI
{
	public class ElectronicsManualUI : UIState
	{
		public static bool Visible { get; set; }

		public static UIPanel BackgroundPanel;

		public float OffsetX = 200f;
		public float OffsetY = 0f;
		public float PanelWidth = 250f;
		public float PanelHeight = 320f;
		public float ButtonSize = 32f;
		public float Padding = 20f;
		public float InnerPadding = 10f;
		public float ScaleX;
		public float ScaleY;
		public int PerRow = 5;

		public override void OnInitialize()
		{
			BackgroundPanel = new UIPanel();
			BackgroundPanel.Left.Set(((Main.screenWidth - PanelWidth) / 2) + OffsetX, 0f);
			BackgroundPanel.Top.Set(((Main.screenHeight - PanelHeight) / 2) + OffsetY, 0f);
			BackgroundPanel.Width.Set(PanelWidth, 0f);
			BackgroundPanel.Height.Set(PanelHeight, 0f);
			BackgroundPanel.BorderColor = new Color(0, 0, 0, 0);

			var textHeight = ChatManager.GetStringSize(Main.fontMouseText, Constants.ToolCategories[0], new Vector2(0, 0)).Y;

			var currentY = 0f;
			var currentX = this.Padding;

			for (var cat = 0; cat < Constants.ToolCategories.Count; cat++)
			{
				var uiTitle = new UIText(Constants.ToolCategories[cat]);
				uiTitle.Height.Set(textHeight, 0f);
				uiTitle.Top.Set(currentY, 0f);
				BackgroundPanel.Append(uiTitle);

				//currentY += textHeight;

				var i = 0;
				foreach (var tool in Constants.Tools[cat])
				{
					if (i % this.PerRow == 0)
					{
						currentY += this.ButtonSize + this.InnerPadding;
						currentX = 0;
					}
					else
					{
						currentX += this.ButtonSize + this.InnerPadding;
					}

					var button = new ElectronicsManualButton(tool, WireMod.Instance.GetTexture($"Images/Icons/{tool}Icon"))
					{
						ToolCat = cat,
						Tool = i
					};

					button.Height.Set(this.ButtonSize, 0f);
					button.Width.Set(this.ButtonSize, 0f);
					button.Left.Set(currentX, 0f);
					button.Top.Set(currentY - (this.ButtonSize / 2), 0f);
					button.SetVisibility(0f, 0.25f);
					
					BackgroundPanel.Append(button);

					i++;
				}

				currentY += this.Padding + this.InnerPadding;
				currentX = this.Padding;
			}

			BackgroundPanel.Height.Set(currentY + this.Padding, 0f);

			this.Append(BackgroundPanel);
			Recalculate();
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);

			if (BackgroundPanel.ContainsPoint(Main.MouseScreen)) Main.LocalPlayer.mouseInterface = true;

			var modPlayer = Main.LocalPlayer.GetModPlayer<WireModPlayer>();
			if (modPlayer != null && modPlayer.ShowPreview) this.DrawPreview(spriteBatch);
		}

		protected void DrawPreview(SpriteBatch spriteBatch)
		{
			if (Main.netMode == NetmodeID.Server) return;

			var modPlayer = Main.LocalPlayer.GetModPlayer<WireModPlayer>(WireMod.Instance);
			var dev = modPlayer?.PlacingDevice;

			if (dev == null) return;

			var screenRect = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);

			//var mouseTile = new Point16((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
			var mouseTile = WireMod.Instance.GetMouseTilePosition();
			//Main.NewText($"Mouse Tile: X {mouseTile.X}, Y {mouseTile.Y}");

			var offsetMouseTile = new Point16(mouseTile.X - dev.Origin.X, mouseTile.Y - dev.Origin.Y);
			//Main.NewText($"Offset Mouse Tile: X {offsetMouseTile.X}, Y {offsetMouseTile.Y}");

			var offsetMouseWorld = new Point16(
				(int)(offsetMouseTile.X * 16 * Main.GameZoomTarget),
				(int)(offsetMouseTile.Y * 16 * Main.GameZoomTarget)
			);
			//Main.NewText($"Offset Mouse World: X {offsetMouseWorld.X}, Y {offsetMouseWorld.Y}");

			var offsetMouseScreen = new Point16(offsetMouseWorld.X - screenRect.X, offsetMouseWorld.Y - screenRect.Y);
			//Main.NewText($"Offset Mouse Screen: X {offsetMouseScreen.X}, Y {offsetMouseScreen.Y}");

			var texture = WireMod.Instance.GetTexture($"Images/{dev.GetType().Name}");

			var deviceScreenRect = new Rectangle(
				offsetMouseScreen.X,
				offsetMouseScreen.Y,
				(int)(dev.Width * 16 * Main.GameZoomTarget),
				(int)(dev.Height * 16 * Main.GameZoomTarget)
			);

			spriteBatch.Draw(texture, deviceScreenRect, dev.GetSourceRect(0), Color.White * 0.5f);
		}
	}
}
