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

			if (this.ToolCat == modPlayer.ToolCategoryMode && this.Tool == modPlayer.ToolMode && modPlayer.ToolMode + modPlayer.ToolCategoryMode > 0)
			{
				this.SetVisibility(1f, 1f);
			}
			else if (hovering)
			{
				this.SetVisibility(0.5f, 0.5f);
			}
			else
			{
				this.SetVisibility(0.25f, 0.25f);
			}

			if (hovering)
			{
				//Main.NewText("Hovering DrawSelf");
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

	//public class CategoryPanel : UIPanel
	//{
	//	public ElectronicsManualUI UI { get; set; }
	//	public int Category { get; set; }

	//	public List<UIImageButton> Buttons = new List<UIImageButton>();


	//	public CategoryPanel(ElectronicsManualUI ui, int cat)
	//	{
	//		this.UI = ui;
	//		this.Category = cat;

	//		var title = ElectronicsManualUI.ToolCategories[this.Category];
	//		var tools = ElectronicsManualUI.Tools[this.Category];



	//		var uiTitle = new UIText(title);
	//		uiTitle.Height.Set(ChatManager.GetStringSize(Main.fontMouseText, title, new Vector2(0, 0)).Y, 0f);
	//		this.Append(uiTitle);

	//		var currentY = 0f;
	//		var currentX = 0f;

	//		var i = 0;
	//		foreach (var device in tools)
	//		{
	//			if (i % this.UI.PerRow == 0)
	//			{
	//				currentY += (this.UI.ButtonSize + (this.UI.Padding / 2));
	//				currentX = 0;
	//			}
	//			else
	//			{
	//				currentX += (this.UI.ButtonSize + (this.UI.Padding / 2));
	//			}

	//			var tool = tools[i];

	//			var texture = ModLoader.GetMod("WireMod").GetTexture(tool == "Wiring" ? "Items/MicrochipItem" : $"Items/{tool}Item");
	//			var button = new UIImageButton(texture);
	//			button.Height.Set(this.UI.ButtonSize, 0f);
	//			button.Width.Set(this.UI.ButtonSize, 0f);
	//			button.Left.Set(currentX, 0f);
	//			button.Top.Set(currentY, 0f);

	//			button.OnClick += (evt, element) =>
	//			{
	//				ElectronicsManualUI.Settings.ToolCategoryMode = this.Category;
	//				ElectronicsManualUI.Settings.ToolMode = index;
	//			};

	//			this.Buttons.Add(button);
	//			rowPanel.Append(button);
	//		}

	//		for (var row = 0; row < tools.Count / this.UI.PerRow; row++)
	//		{
	//			var rowPanel = new UIPanel();
	//			rowPanel.Top.Set(row * this.UI.ButtonSize, 0f);
	//			rowPanel.Height.Set(this.UI.ButtonSize, 0f);
	//			rowPanel.Width.Set((this.UI.ButtonSize * this.UI.PerRow) + (this.UI.InnerPadding * (this.UI.PerRow - 1)), 0f);

	//			for (var rowTool = row * this.UI.PerRow; rowTool < (row + 1) * this.UI.PerRow || rowTool < tools.Count; rowTool++)
	//			{

	//			}

	//			this.Append(rowPanel);
	//		}

	//			//foreach (var device in tools.Skip(row * this.UI.PerRow).Take(this.UI.PerRow))
	//			//{
	//			//	if (i % this.UI.PerRow == 0)
	//			//	{
	//			//		currentY += (this.UI.ButtonSize + (this.UI.Padding / 2));
	//			//		currentX = 0;
	//			//	}
	//			//	else
	//			//	{
	//			//		currentX += (this.UI.ButtonSize + (this.UI.Padding / 2));
	//			//	}


	//			//	button.Left.Set(currentX, 0f);
	//			//	button.Top.Set(currentY, 0f);

	//			//	var index = i;

	//			//	button.OnClick += (evt, element) =>
	//			//	{
	//			//		ElectronicsManualUI.Settings.ToolCategoryMode = this.Category;
	//			//		ElectronicsManualUI.Settings.ToolMode = index;
	//			//	};

	//			//	this.Buttons.Add(button);
	//			//	this.Append(button);

	//			//	i++;
	//			//}
	//		//}

	//	}
	//}
}
