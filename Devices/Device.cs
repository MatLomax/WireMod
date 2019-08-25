using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using WireMod.Items;

namespace WireMod.Devices
{
    public abstract class Device
    {
        // Design
        public string Name { get; set; } = "";
        public int Width { get; set; } = 1;
        public int Height { get; set; } = 2;

        public List<string> AutoTypes { get; set; } = new List<string>();
        public bool AutoDetectType => this.AutoTypes.Count > 0;
        
        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();

        public string RightClickHelp { get; set; }

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
        public Rectangle LocationRect { get; private set; }
        public Point16 LocationTile { get; private set; }
        public Vector2 LocationWorld { get; private set; }
        public Rectangle LocationWorldRect { get; private set; }
        public Rectangle LocationScreenRect
        {
            get
            {
                var screenRect = Helpers.GetScreenRect();
                var worldRect = this.LocationWorldRect;

                return !worldRect.Intersects(screenRect) ? default(Rectangle) : new Rectangle(worldRect.X - screenRect.X, worldRect.Y - screenRect.Y, worldRect.Width, worldRect.Height);
            }
        }

        public Point16 LocationOriginTile { get; private set; }
        public Vector2 LocationOriginWorld { get; private set; }
        public Vector2 LocationOriginScreen { get; private set; }

        public void SetLocation(int x, int y)
        {
            this.LocationRect = new Rectangle(x, y, this.Width, this.Height);
            this.LocationTile = new Point16(this.LocationRect.X, this.LocationRect.Y);
            this.LocationWorld = new Vector2(this.LocationRect.X * 16, this.LocationRect.Y * 16);
            this.LocationWorldRect = new Rectangle(this.LocationRect.X * 16, this.LocationRect.Y * 16, this.Width * 16, this.Height * 16);

            this.LocationOriginTile = this.LocationTile + this.Origin;
            this.LocationOriginWorld = this.LocationOriginTile.ToWorldCoordinates();
            this.LocationOriginScreen = this.LocationOriginWorld - Main.screenPosition;
        }
        
        public string DetectType()
        {
            if (!this.AutoDetectType) return "";

            var dataType = "auto";

            foreach (var p in this.Pins.Values.SelectMany(p => p.Values).Where(p => p.Auto))
            {
                if (p.IsConnected())
                {
                    dataType = (p is PinIn pinIn) ? pinIn.ConnectedPin.DataType : ((PinOut)p).ConnectedPins.First().DataType;
                    break;
                }
            }

            return dataType;
        }

        public void SetAutoType(string dataType = null)
        {
            if (dataType == null) dataType = this.DetectType();
            if (dataType == "auto") dataType = this.AutoTypes.First();

            foreach (var p in this.Pins.Values.SelectMany(p => p.Values).Where(p => p.Auto))
            {
                p.DataType = dataType;
            }
        }

        public Pin GetPin(string name) => (Pin)GetPinIn(name) ?? GetPinOut(name);
        public PinIn GetPinIn(string name) => (PinIn)this.Pins["In"].Values.FirstOrDefault(p => p.Name == name);
        public PinIn GetPinIn(int index) => (PinIn)this.Pins["In"][index];
        public PinOut GetPinOut(string name) => (PinOut)this.Pins["Out"].Values.FirstOrDefault(p => p.Name == name);
        public PinOut GetPinOut(int index) => (PinOut)this.Pins["Out"][index];
        
        public virtual void OnRightClick(Pin pin = null) { }
        public virtual void OnHitWire(Pin pin = null) { }

        public virtual void OnKill()
        {
            // Drop a microchip at the player's location
            Item.NewItem((int)Main.LocalPlayer.position.X, (int)Main.LocalPlayer.position.Y, 16, 16, WireMod.Instance.ItemType<Microchip>());
        }

        public virtual void OnPlace() { }

        public virtual void Draw(SpriteBatch spriteBatch) { }
        public virtual void Update(GameTime gameTime) { }

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
                pin.Auto = pinDesign.Auto;

                this.Pins[pinDesign.Type].Add(pinDesign.Num, pin);
            }
        }

        public virtual List<(string Line, Color Color, float Size)> Debug(Pin pin = null)
        {
            var defaultColor = Color.Black;
            var titleColor = Color.DarkBlue;
            var highlightColor = Color.Red;

            var lines = new List<(string, Color, float)>
            {
                (this.Name, titleColor, 1f),
                ($"X: {(pin != null ? pin.Location.X : this.LocationTile.X + this.Origin.X)}, Y: {(pin != null ? pin.Location.Y : this.LocationTile.Y + this.Origin.Y)}", titleColor, WireMod.SmallText),
                ("-----------------", defaultColor, 1f),
            };

            foreach (var p in this.Pins.Values.SelectMany(p => p.Values))
            {
                lines.AddRange(p.GetDebug().Select(d => (d, p == Main.LocalPlayer.GetModPlayer<WireModPlayer>().ConnectingPin ? Color.Green : (p == pin ? highlightColor : defaultColor), WireMod.SmallText)));
            }

            if (this.Settings.Any())
            {
                lines.Add(("-----------------", defaultColor, WireMod.SmallText));
                foreach (var setting in this.Settings)
                {
                    lines.Add(($"{setting.Key}: {setting.Value}", Color.Red, WireMod.SmallText));
                }
            }

            if (!string.IsNullOrEmpty(this.RightClickHelp))
            {
                lines.Add(("-----------------", defaultColor, WireMod.SmallText));
                lines.Add((this.RightClickHelp, Color.Blue, WireMod.SmallText));

            }

            return lines;
        }
    }
}
