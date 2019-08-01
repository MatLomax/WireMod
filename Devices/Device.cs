using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;

namespace WireMod.Devices
{
    public abstract class Device
    {
        // Design
        public string Name { get; set; } = "";
        public int Width { get; set; } = 1;
        public int Height { get; set; } = 2;


        public string Value { get; set; } = "";
        public string ValueType { get; set; }
        

        /// <summary>
        /// Local origin location, almost always the 'chip' part of the 'tile'.
        /// This is also where the 'tile' is centered when placing.
        /// </summary>
        public Point16 Origin { get; set; } = new Point16(0, 0);
        public List<PinDesign> PinLayout { get; set; } = new List<PinDesign>();

        // Instance
        public Dictionary<string, Dictionary<int, Pin>> Pins = new Dictionary<string, Dictionary<int, Pin>>
        {
            {"In", new Dictionary<int, Pin>()},
            {"Out", new Dictionary<int, Pin>()},
        };

        /// <summary>
        /// Location of this device in the world (Tile Co-ords)
        /// </summary>
        //public Point16 Location { get; set; }
        public Rectangle LocationRect { get; set; }

        public Point16 LocationTile => this.LocationRect != default(Rectangle) ? new Point16(this.LocationRect.X, this.LocationRect.Y) : default(Point16);
        public Point16 LocationWorld => this.LocationRect != default(Rectangle) ? new Point16(this.LocationRect.X * 16, this.LocationRect.Y * 16) : default(Point16);

        public virtual void OnRightClick(Pin pin = null) { }

        public virtual void OnKill()
        {
            // Drop a microchip at the device's location
            Item.NewItem(this.LocationRect.X * 16, this.LocationRect.Y * 16, 16, 16, WireMod.Instance.ItemType("Microchip"));
        }

        public virtual void OnPlace() { }

        public virtual void Draw(SpriteBatch spriteBatch) { }

        public virtual Rectangle GetSourceRect(int style = -1) => new Rectangle(0, 0, this.Width * 16, this.Height * 16);

        public void SetPins()
        {
            if (this.LocationRect == default(Rectangle)) return;

            this.Pins["In"].Clear();
            this.Pins["Out"].Clear();

            foreach (var pinDesign in this.PinLayout)
            {
                var pin = pinDesign.Type == "In" ? (Pin)(new PinIn(this)) : (Pin)(new PinOut(this));
                pin.Num = pinDesign.Num;
                pin.DataType = pinDesign.DataType;
                pin.Type = pinDesign.Type;
                pin.Name = pinDesign.Name;

                this.Pins[pinDesign.Type].Add(pinDesign.Num, pin);
            }
        }

        public virtual List<(string Line, Color Color)> Debug(Pin pin = null)
        {
            var defaultColor = Color.Black;
            var titleColor = Color.DarkBlue;
            var highlightColor = Color.Red;
            var (x, y) = WireMod.Instance.GetMouseTilePosition();

            var lines = new List<(string, Color)>
            {
                ($"{this.Name}      X: {x}, Y: {y}", titleColor),
                ("-----------------", defaultColor),
            };

            foreach (var p in this.Pins.Values.SelectMany(p => p.Values))
            {
                lines.AddRange(p.GetDebug().Select(d => (d, p == pin ? highlightColor : defaultColor)));
            }

            return lines;
        }
    }
}
