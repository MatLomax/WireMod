using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.UI;

namespace WireMod.UI.Elements
{
    internal class UIInputTextField : UIElement
    {
        public delegate void EventHandler(object sender, EventArgs e);
        public event EventHandler OnTextChange;
        public event EventHandler OnSave;
        public event EventHandler OnCancel;

        private readonly string _hintText;
        private string _currentString;
        private int _textBlinkerCount;

        public string Text
        {
            get => _currentString;
            set
            {
                if (_currentString == value) return;
                _currentString = value;
                this.OnTextChange?.Invoke(this, EventArgs.Empty);
            }
        }
        
        public UIInputTextField(string value, string hintText)
        {
            this._currentString = value;
            this._hintText = hintText;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            PlayerInput.WritingText = true;
            Main.instance.HandleIME();

            var inputText = Main.GetInputText(_currentString);

            if (inputText != _currentString)
            {
                this._currentString = inputText;
                this.OnTextChange?.Invoke(this, EventArgs.Empty);
            }

            var dimensions = this.GetDimensions();
			
            if (this._currentString.Length == 0)
            {
                Utils.DrawBorderString(spriteBatch, this._hintText, new Vector2(dimensions.X, dimensions.Y), Color.Gray);
            }
            else
            {
                var text = _currentString;

                if (++_textBlinkerCount / 20 % 2 == 0)
                {
                    text += "|";
                }

                Utils.DrawBorderString(spriteBatch, text, new Vector2(dimensions.X, dimensions.Y), Color.White);
            }

            if (Main.inputTextEnter)
            {
                this.OnSave?.Invoke(this, EventArgs.Empty);
            }

            if (Main.inputTextEscape)
            {
                this.OnCancel?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}