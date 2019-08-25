using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

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
            this.Settings.Add("TriggerType", "All");
            this.Settings.Add("TriggerTarget", "0");

            this.RightClickHelp = "Right Click to toggle trigger type";

            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("In", 0, new Point16(1, 0), "bool", "Trigger"),
                new PinDesign("In", 1, new Point16(0, 1), "bool", "Reset"),
                new PinDesign("Out", 0, new Point16(1, 2), "bool"),
            };
        }

        public string Output(Pin pin = null) => this.Settings["Value"];

        public override void OnHitWire(Pin pin = null)
        {
            this.TriggerDevices();
        }

        public override void Update(GameTime gameTime)
        {
            if (!int.TryParse(this.Settings["Value"], out var val)) return;
            if (!this.Pins["In"][0].IsConnected()) return;
            if (!int.TryParse(this.Pins["In"][0].GetValue(), out var trigger)) return;

            if (this.GetPin("Reset").IsConnected() && int.TryParse(this.GetPin("Reset").GetValue(), out var reset))
            {
                if (reset == 1) this._reset = true;
            }

            if (trigger > 0 && val <= 0 && this._reset)
            {
                this._reset = false;

                if (!this.GetPinOut(0).IsConnected())
                {
                    this.Settings["Value"] = trigger.ToString();
                    return;
                }

                this.TriggerDevices();

                this.Settings["Value"] = trigger.ToString();
                return;
            }

            if (trigger <= 0 && val > 0 && !this.GetPinIn(1).IsConnected())
            {
                this._reset = true;
            }

            this.Settings["Value"] = trigger.ToString();
        }

        private void TriggerDevices()
        {
            var pins = this.GetPinOut(0).ConnectedPins.Where(p => p.Device is ITriggered).ToList();
            if (!int.TryParse(this.Settings["TriggerTarget"], out var target)) return;
            if (target >= pins.Count) target = pins.Count - 1;

            // Trigger connected devices
            if (this.Settings["TriggerType"] == "All")
            {
                pins.ForEach(pin =>
                {
                    ((ITriggered)pin.Device).Trigger(pin);
                });
                this.Settings["TriggerTarget"] = "0";
            }
            else
            {
                var pin = this.Settings["TriggerType"] == "Sequential" ? pins[(target + 1) % pins.Count] : pins[WorldGen.genRand.Next(0, pins.Count)];
                ((ITriggered)pin.Device).Trigger(pin);
                this.Settings["TriggerTarget"] = pins.IndexOf(pin).ToString();
            }
        }

        public override List<(string Line, Color Color, float Size)> Debug(Pin pin = null)
        {
            var debug = base.Debug(pin);

            debug.Add(($"Reset: {(this._reset ? "True" : "False")}", Color.Black, WireMod.SmallText));

            return debug;
        }

        public override void OnRightClick(Pin pin = null)
        {
            this.Settings["TriggerType"] = TriggerTypes[(TriggerTypes.IndexOf(this.Settings["TriggerType"]) + 1) % TriggerTypes.Count];
        }

        private static readonly List<string> TriggerTypes = new List<string>
        {
            "All",
            "Sequential",
            "Random",
        };
    }
}
