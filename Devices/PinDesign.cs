using Terraria.DataStructures;

namespace WireMod.Devices
{
    public class PinDesign
    {
        public string Type { get; set; }
        public int Num { get; set; }
        public Point16 Offset { get; set; }
        public string DataType { get; set; }
        public string Name { get; set; }
        public bool Auto { get; set; }

        public PinDesign(string type, int num, Point16 offset, string dataType, string name = "", bool auto = false)
        {
            this.Type = type;
            this.Num = num;
            this.Offset = offset;
            this.DataType = dataType;
            this.Name = name;
            this.Auto = auto;
        }

        public override string ToString() => this.Name != "" ? this.Name : $"Pin{this.Type}{this.Num}";
    }
}
