using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Chat;

namespace WireMod.UI
{
	public class ElectronicsManualUI : UIState
	{
		public static bool Visible { get; set; }

		private DragableUIPanel _panel;

		public float OffsetX = 300f;
		public float OffsetY = 0f;
		public float PanelWidth = 260f;
		public float PanelHeight = 320f;
		public float ButtonSize = 32f;
		public float Padding = 20f;
		public float InnerPadding = 10f;
		public float ScaleX;
		public float ScaleY;
		public int PerRow = 5;

		public override void OnInitialize()
		{
			this._panel = new DragableUIPanel();
			this._panel.Left.Set(((Main.screenWidth - this.PanelWidth) / 2) + this.OffsetX, 0f);
			this._panel.Width.Set(this.PanelWidth, 0f);
			this._panel.BorderColor = new Color(0, 0, 0, 0);

			var textHeight = ChatManager.GetStringSize(Main.fontMouseText, Constants.ToolCategories[0], new Vector2(0, 0)).Y;

			var currentY = 0f;
			var currentX = this.Padding;

			var uiTitle = new UIText("Electronics Manual", 1.5f);
			uiTitle.Height.Set(textHeight, 0f);
			uiTitle.Top.Set(currentY, 0f);
			this._panel.Append(uiTitle);

			currentY += 40;

			for (var cat = 0; cat < Constants.ToolCategories.Count; cat++)
			{
				var uiCatTitle = new UIText(Constants.ToolCategories[cat]);
				uiCatTitle.Height.Set(textHeight, 0f);
				uiCatTitle.Top.Set(currentY, 0f);
				this._panel.Append(uiCatTitle);

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

					// TODO: Issue #6: Remove need for separate device icons
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

					this._panel.Append(button);

					i++;
				}

				currentY += this.Padding + this.InnerPadding;
				currentX = this.Padding;
			}

			this._panel.Height.Set(currentY + this.Padding, 0f);
			this._panel.Top.Set(((Main.screenHeight - this.PanelHeight) / 2) + this.OffsetY, 0f);

			this.Append(this._panel);
			Recalculate();
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);

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

			var mouseTile = WireMod.Instance.GetMouseTilePosition();
			var offsetMouseTile = new Point16(mouseTile.X - dev.Origin.X, mouseTile.Y - dev.Origin.Y);
			var offsetMouseWorld = new Point16(offsetMouseTile.X * 16, offsetMouseTile.Y * 16);
			var offsetMouseScreen = new Point16(offsetMouseWorld.X - screenRect.X, offsetMouseWorld.Y - screenRect.Y);

			var texture = WireMod.Instance.GetTexture($"Images/{dev.GetType().Name}");

			var deviceScreenRect = new Rectangle(offsetMouseScreen.X, offsetMouseScreen.Y, dev.Width * 16, dev.Height * 16);

			spriteBatch.Draw(texture, deviceScreenRect, dev.GetSourceRect(0), Color.White * 0.5f);
		}
	}
}
