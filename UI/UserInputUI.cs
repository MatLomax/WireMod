using System;
using Microsoft.Xna.Framework;
using Terraria;
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
            textbox.OnSave += (sender, args) => this.Save();
            textbox.OnCancel += (sender, args) => this.Close();

            var button = new UIClickableButton("Save", (evt, element) => this.Save());

            this._panel.Append(textbox);

            button.Top.Set(50f, 0f);

            this._panel.Append(button);

            this.Append(this._panel);
        }

        private void Save()
        {
            this.OnSave?.Invoke(this, EventArgs.Empty);
            this.Close();
        }

        private void Close()
        {
            Main.blockInput = false;
            WireMod.Instance.UserInputUserInterface.SetState(null);
            Visible = false;
        }
    }
}
