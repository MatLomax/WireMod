using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace WireMod.UI.Elements
{
    public class UIClickableButton : UIElement
    {
        private object _text;
        private UIText _uiText;
        private readonly MouseEvent _clickAction;

        public string Text
        {
            get => this._uiText?.Text ?? string.Empty;
            set => this._text = value;
        }

        public UIClickableButton(object text, MouseEvent clickAction)
        {
            this._text = text?.ToString() ?? string.Empty;
            this._clickAction = clickAction;
        }

        public override void OnInitialize()
        {
            var uiPanel = new UIPanel
            {
                Width = StyleDimension.Fill,
                Height = StyleDimension.Fill
            };

            this.Append(uiPanel);

            this._uiText = new UIText("");
            this._uiText.VAlign = this._uiText.HAlign = 0.5f;
            uiPanel.Append(this._uiText);

            uiPanel.OnClick += this._clickAction;
        }

        public override void Update(GameTime gameTime)
        {
            if (this._text == null) return;

            this._uiText.SetText(_text.ToString());
            this._text = null;

            Recalculate();

            this.MinWidth = this._uiText.MinWidth;
            this.MinHeight = this._uiText.MinHeight;

            this.Width.Set(this._uiText.Width.Pixels + 2, 0f);
            this.Height.Set(this._uiText.Height.Pixels + 2, 0f);
        }
    }
}