using System.Collections.Generic;
using Terraria.DataStructures;

namespace WireMod.Devices
{
    public class Wire
    {
        public Pin StartPin { get; set; }
        public Pin EndPin { get; set; }
        public List<Point16> Points { get; set; } = new List<Point16>();

        public Wire(Pin startPin)
        {
            this.StartPin = startPin;
        }
        public Wire(Pin startPin, Pin endPin, List<Point16> points)
        {
            this.StartPin = startPin;
            this.EndPin = endPin;
            this.Points = points;
        }

        public List<Point16> GetPoints(Pin start = null, bool ends = true)
        {
            if (start == null) start = this.StartPin;

            var points = new List<Point16>();
            if (start == this.StartPin)
            {
                if (ends) points.Add(start.Location);
                points.AddRange(this.Points);
                if (ends && this.EndPin != null) points.Add(this.EndPin.Location);
            }
            else
            {
                if (this.EndPin == null) return points;
                if (ends) points.Add(start.Location);
                foreach (var p in this.Points) points.Insert(ends ? 1 : 0, p);
                if (ends) points.Add(this.StartPin.Location);
            }

            return points;
        }
    }
}