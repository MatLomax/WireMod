using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

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
        //public Player Player { get; set; }
        //public int Ref { get; set; }

        //public (Dictionary<int, PinIn> In, Dictionary<int, PinOut> Out) Pins = (new Dictionary<int, PinIn>(), new Dictionary<int, PinOut>());
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

        public Point16 Location => this.LocationRect != default(Rectangle) ? new Point16(this.LocationRect.X, this.LocationRect.Y) : default(Point16);

        public virtual void OnRightClick(Pin pin = null) { }

        public virtual void OnKill()
        {
            // Drop a microchip at the device's location
            Item.NewItem(this.LocationRect.X * 16, this.LocationRect.Y * 16, 16, 16, WireMod.Instance.ItemType("Microchip"));
        }

        public virtual void OnPlace() { }

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

        public virtual List<(string Line, Color Color)> Debug()
        {
            var defaultColor = Color.Black;
            var titleColor = Color.DarkBlue;
            var highlightColor = Color.Red;

            var lines = new List<(string, Color)>
            {
                ($"{this.Name}           X: {this.Location.X}, Y: {this.Location.Y}", titleColor),
                ($"-----------------", defaultColor),
            };

            foreach (var pin in this.Pins.Values.SelectMany(p => p.Values))
            {
                lines.AddRange(pin.GetDebug().Select(d => (d, defaultColor)));

                //if (pin.IsConnected())
                //{
                //    if (pin.Type == "Out")
                //    {
                //        foreach (var p in ((PinOut) pin).ConnectedPins)
                //        {
                //            lines.Add(($"{pin.Name} [{(pin.DataType == "bool" ? (pin.GetValue() == "1" ? "True" : (pin.GetValue() == "0" ? "False" : "Disconnected")) : pin.GetValue())}] => {p.Device.Name} {p.Name}", /*pin.Key == pin ? highlightColor :*/ defaultColor));
                //        }
                //    }
                //    else
                //    {
                //        lines.Add(($"{pin.Name} [{(pin.DataType == "bool" ? (pin.GetValue() == "1" ? "True" : (pin.GetValue() == "0" ? "False" : "Disconnected")) : pin.GetValue())}] => {((PinIn)pin).ConnectedPin.Device.Name} {((PinIn)pin).ConnectedPin.Name}", /*pin.Key == pin ? highlightColor :*/ defaultColor));
                //    }
                //}
                //else
                //{
                //    if (pin.Type == "Out")
                //    {
                //        lines.Add(($"{pin.Name} (Disconnected) [{(pin.DataType == "bool" ? (pin.GetValue() == "1" ? "True" : (pin.GetValue() == "0" ? "False" : "Disconnected")) : pin.GetValue())}]", /*pin.Key == pin ? highlightColor :*/ defaultColor));
                //    }
                //    else
                //    {
                //        lines.Add(($"{pin.Name} (Disconnected)", /*pin.Key == pin ? highlightColor :*/ defaultColor));
                //    }
                //}
            }

            return lines;
        }
    }

    public class DeviceSerializer : TagSerializer<Device, TagCompound>
    {
        public override TagCompound Serialize(Device device)
        {
            return new TagCompound
            {
                ["name"] = device.GetType().Name,
                ["x"] = device.LocationRect.X + device.Origin.X,
                ["y"] = device.LocationRect.Y + device.Origin.Y,
                ["value"] = device.Value,
            };
        }

        public override Device Deserialize(TagCompound tag)
        {
            var device = (Device)Activator.CreateInstance(Type.GetType("WireMod.Devices." + tag.GetString("name")) ?? throw new InvalidOperationException("Device not found!"));
            //device.LocationRect = new Rectangle(tag.GetInt("x") - device.Origin.X, tag.GetInt("y") - device.Origin.Y, device.Width, device.Height);
            device.LocationRect = new Rectangle(tag.GetInt("x"), tag.GetInt("y"), device.Width, device.Height);
            device.Value = tag.GetString("value");
            
            return device;
        }
    }
}
