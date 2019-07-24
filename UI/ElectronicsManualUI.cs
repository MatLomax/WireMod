using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Chat;
using WireMod.Devices;

namespace WireMod.UI
{
	internal class UIInputTextField : UIElement
	{
		public delegate void EventHandler(object sender, EventArgs e);

		private string hintText;

		private string currentString = "";

		public string Text = "";

		public event EventHandler OnTextChange;

		public UIInputTextField(string hintText)
		{
			this.hintText = hintText;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			PlayerInput.WritingText = true;
			Main.instance.HandleIME();

			Main.inputTextEnter = false;

			//if ()
			//while (!Main.inputTextEnter)
			//{
			//	this.Text += Main.GetInputText(currentString);
			//}

			Main.NewText(this.Text);
			this.OnTextChange?.Invoke(this, EventArgs.Empty);
			//if (Main.inputTextEnter)
			//{
			//	//byte[] bytes = new byte[1]
			//	//{
			//	//	10
			//	//};
			//	//this.Text += Encoding.ASCII.GetString(bytes);
			//	this.OnTextChange?.Invoke(this, EventArgs.Empty);
			//}
			//else if (Main.inputTextEscape)
			//{
			//	Main.InputTextSignCancel();
			//}


			//if (inputText != currentString)
			//{
			//	currentString = inputText;
			//	this.OnTextChange?.Invoke(this, EventArgs.Empty);
			//}

			//if (Main.inputTextEnter)
			//{

			//}

			//string text = currentString;

			//var dimensions = GetDimensions();
			//if (currentString.Length == 0)
			//{
			//	Utils.DrawBorderString(spriteBatch, hintText, new Vector2(dimensions.X, dimensions.Y), Color.Gray);
			//}
			//else
			//{
			//	Utils.DrawBorderString(spriteBatch, text, new Vector2(dimensions.X, dimensions.Y), Color.White);
			//}
		}
	}

	public class TextboxUI : UIState
	{
		internal UIInputTextField TextBox;

		public override void OnInitialize()
		{
			this.TextBox = new UIInputTextField("");
			this.TextBox.Top.Set(300f, 0f);
			this.TextBox.Left.Set(300f, 0f);

			this.Append(this.TextBox);
		}
	}

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

					//var a = WireMod.Instance.GetTexture($"Images/{tool}");
					
					//var cropped = new Texture2D(Main.graphics.GraphicsDevice, 16, 16);

					//// Copy the data from the cropped region into a buffer, then into the new texture
					//var data = new Color[16 * 16];
					//var r = new Rectangle(, , 16, 16);
					//a.GetData(0, newBounds, data, 0, newBounds.Width * newBounds.Height);
					//croppedTexture.SetData(data);

					var button = new UIImageButton(WireMod.Instance.GetTexture($"Images/Icons/{tool}Icon"));
					button.Height.Set(this.ButtonSize, 0f);
					button.Width.Set(this.ButtonSize, 0f);
					button.Left.Set(currentX, 0f);
					button.Top.Set(currentY - (this.ButtonSize / 2), 0f);

					var catIndex = cat;
					var index = i;
					var name = tool;

					button.OnClick += (e, el) =>
					{
						//Main.NewText(typeof(AndGate).AssemblyQualifiedName);
						//Main.NewText("WireMod.Devices." + Constants.Tools[catIndex][index]);
						if (Main.dedServ) return;

						var modPlayer = Main.LocalPlayer.GetModPlayer<WireModPlayer>();
						if (modPlayer == null) return;
						
						modPlayer.ToolCategoryMode = catIndex;
						modPlayer.ToolMode = index;

						if (catIndex == 0) return;
						
						modPlayer.PlacingDevice = (Device)Activator.CreateInstance(Type.GetType("WireMod.Devices." + Constants.Tools[catIndex][index]) ?? throw new InvalidOperationException("Device not found!"));
						modPlayer.PlacingDevice.SetPins();
					};

					//button.OnMouseOver += (e, el) =>
					//{
					//	Main.hoverItemName = name;
					//};

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
			if (BackgroundPanel.ContainsPoint(new Vector2((float)Main.mouseX, (float)Main.mouseY)))
			{
				Main.LocalPlayer.mouseInterface = true;
			}

			var modPlayer = Main.player[Main.myPlayer]?.GetModPlayer<WireModPlayer>(WireMod.Instance);
			
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

			var offsetMouseWorld = new Point16(offsetMouseTile.X * 16, offsetMouseTile.Y * 16);
			//Main.NewText($"Offset Mouse World: X {offsetMouseWorld.X}, Y {offsetMouseWorld.Y}");

			var offsetMouseScreen = new Point16(offsetMouseWorld.X - screenRect.X, offsetMouseWorld.Y - screenRect.Y);
			//Main.NewText($"Offset Mouse Screen: X {offsetMouseScreen.X}, Y {offsetMouseScreen.Y}");

			var texture = WireMod.Instance.GetTexture($"Images/{dev.GetType().Name}");

			var deviceScreenRect = new Rectangle(
				offsetMouseScreen.X,
				offsetMouseScreen.Y,
				dev.Width * 16,
				dev.Height * 16
			);

			spriteBatch.Draw(texture, deviceScreenRect, Color.White * 0.5f);
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
