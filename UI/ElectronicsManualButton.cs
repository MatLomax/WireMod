using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using WireMod.Devices;

namespace WireMod.UI
{
	public class ElectronicsManualButton : UIImageButton
	{
		public string Name { get; set; }
		public int ToolCat { get; set; }
		public int Tool { get; set; }
		
		public ElectronicsManualButton(string name, Texture2D texture) : base(texture)
		{
			this.Name = name;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			var modPlayer = Main.LocalPlayer.GetModPlayer<WireModPlayer>();
			if (modPlayer == null)
			{
				base.DrawSelf(spriteBatch);
				return;
			}

			var hovering = this.ContainsPoint(Main.MouseScreen);

			// Set button opacity
			if (this.ToolCat == modPlayer.ToolCategoryMode && this.Tool == modPlayer.ToolMode && modPlayer.ToolMode + modPlayer.ToolCategoryMode > 0)
			{
				this.SetVisibility(1f, 1f);
			}
			else if (hovering)
			{
				this.SetVisibility(0.75f, 0.75f);
			}
			else
			{
				this.SetVisibility(0.5f, 0.5f);
			}

			if (hovering)
			{
				Main.instance.MouseText(Constants.ToolNames[this.Name]);
				
				// Click
				if (!Main.mouseLeftRelease || !Main.mouseLeft)
				{
					base.DrawSelf(spriteBatch);
					return;
				}
				
				modPlayer.ToolCategoryMode = this.ToolCat;
				modPlayer.ToolMode = this.Tool;

				if (this.ToolCat <= 0)
				{
					base.DrawSelf(spriteBatch);
					return;
				}

				modPlayer.PlacingDevice = (Device)Activator.CreateInstance(Type.GetType("WireMod.Devices." + Constants.Tools[this.ToolCat][this.Tool]) ?? throw new InvalidOperationException("Device not found!"));
				modPlayer.PlacingDevice.SetPins();
			}

			base.DrawSelf(spriteBatch);
		}
	}
}
