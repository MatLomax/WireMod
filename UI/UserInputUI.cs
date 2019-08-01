using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using WireMod.UI.Elements;

namespace WireMod.UI
{
    internal class UserInputUI : UIState
    {
        public static bool Visible { get; set; }

        public delegate void EventHandler(object sender, EventArgs e);
        public event EventHandler OnSave;

        public string Value { get; set; }

        private DragableUIPanel _panel;
        public float OffsetX = 0f;
        public float OffsetY = -200f;
        public float PanelWidth = 200f;
        public float PanelHeight = 100f;

        public UserInputUI(string value)
        {
            this.Value = value;
        }

        public override void OnInitialize()
        {
            this._panel = new DragableUIPanel();
            this._panel.Left.Set(((Main.screenWidth - this.PanelWidth) / 2) + this.OffsetX, 0f);
            this._panel.Top.Set(((Main.screenHeight - this.PanelHeight) / 2) + this.OffsetY, 0f);
            this._panel.Width.Set(this.PanelWidth, 0f);
            this._panel.Height.Set(this.PanelHeight, 0f);
            this._panel.BorderColor = new Color(0, 0, 0, 0);

            var textbox = new UIInputTextField(this.Value, "Enter a value");
            textbox.OnTextChange += (s, e) => { this.Value = textbox.Text; };
            //textbox.OnSave += this.Save;
            textbox.OnCancel += this.Close;

            this._panel.Append(textbox);

            var button = new UIImageButton(WireMod.Instance.GetTexture("UI/Icons/pencil"));
            button.Width.Set(32f, 0f);
            button.Height.Set(32f, 0f);
            button.Top.Set(50f, 0f);

            button.OnClick += (e, el) =>
            {
                this.OnSave?.Invoke(this, EventArgs.Empty);
                WireMod.Instance.UserInputUserInterface.SetState(null);
                Visible = false;
			};

            this._panel.Append(button);

            this.Append(this._panel);
        }

        private void Save(object sender, EventArgs e)
        {
            this.Close(sender, e);
        }

        private void Close(object sender, EventArgs e)
        {
            WireMod.Instance.UserInputUserInterface.SetState(null);
            Visible = false;
        }


        //protected override void DrawSelf(SpriteBatch spriteBatch)
        //{
        //    base.DrawSelf(spriteBatch);

        //    var hovering = Main.mouseX > reforgeX - 15 && Main.mouseX < reforgeX + 15 && Main.mouseY > reforgeY - 15 && Main.mouseY < reforgeY + 15 && !PlayerInput.IgnoreMouseInterface;

        //    if (!Main.mouseLeftRelease || !Main.mouseLeft)
        //    {

        //    }
        //}

    }
}
