using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace WireMod.Devices
{
    internal class Trigger : Device, IOutput
    {
        private bool _reset;

        public Trigger()
        {
            this.Name = "Trigger";
            this.Width = 2;
            this.Height = 3;
            this.Origin = new Point16(1, 1);

            this.Settings.Add("Value", "-1");

            this.RightClickHelp = "Right Click to reset trigger";

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("In", 0, new Point16(1, 0), "bool", "Trigger"),
                new PinDesign("In", 1, new Point16(0, 1), "bool", "Reset"),
                new PinDesign("Out", 0, new Point16(1, 2), "bool"),
            };
        }

        public string Output(Pin pin = null) => this.GetOutput().ToString();

        private int GetOutput()
        {
            if (!int.TryParse(this.Settings["Value"], out var val)) return -1;
            if (!this.Pins["In"][0].IsConnected()) return -1;
            if (!int.TryParse(this.Pins["In"][0].GetValue(), out var trigger)) return -1;

            if (this.Pins["In"][1].IsConnected() && int.TryParse(this.Pins["In"][1].GetValue(), out var reset))
            {
                if (reset == 1) this._reset = true;
            }
            
            if (trigger > 0 && val <= 0 && this._reset)
            {
                this._reset = false;

                if (!this.Pins["Out"][0].IsConnected())
                {
                    this.Settings["Value"] = trigger.ToString();
                    return 1;
                }

                // Trigger connected devices
                foreach (var pin in ((PinOut)this.Pins["Out"][0]).ConnectedPins)
                {
                    if (pin.Device is ITriggered triggered) triggered.Trigger(pin);
                }

                this.Settings["Value"] = trigger.ToString();
                return 1;
            }

            if ((trigger <= 0 && val > 0) && !this.Pins["In"][1].IsConnected())
            {
                this._reset = true;
            }

            this.Settings["Value"] = trigger.ToString();
            return 0;
        }

        public override void OnRightClick(Pin pin = null)
        {
            this._reset = true;
        }

        public override List<(string Line, Color Color)> Debug(Pin pin = null)
        {
            var debug = base.Debug(pin);
            debug.Add(($"Reset: {(this._reset ? "True" : "False")}", Color.Black));
            return debug;
        }
    }
}
