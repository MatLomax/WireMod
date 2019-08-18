using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace WireMod.Devices
{
    internal class If : Device, IOutput
    {
        public If()
        {
            this.Name = "If Condition";
            this.Width = 3;
            this.Height = 3;
            this.Origin = new Point16(1, 1);

            this.Settings.Add("DataType", DataTypes.First());
            this.RightClickHelp = $"Right Click to change data type ({string.Join("/", DataTypes)})";
           
            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("In", 0, new Point16(1, 0), "bool", "Condition"),
                new PinDesign("In", 1, new Point16(0, 1), this.Settings["DataType"], "TrueValue"),
                new PinDesign("In", 2, new Point16(2, 1), this.Settings["DataType"], "FalseValue"),
                new PinDesign("Out", 0, new Point16(1, 2), this.Settings["DataType"]),
            };
        }

        public string Output(Pin pin = null)
        {
            if (!this.Pins["In"][0].IsConnected()) return "-1";
            if (!int.TryParse(this.Pins["In"][0].GetValue(), out var condition)) return "-2";

            switch (this.Settings["DataType"])
            {
                case "bool":
                case "int":
                    if (!int.TryParse(this.Pins["In"][1].GetValue(), out var trueInput)) trueInput = 0;
                    if (!int.TryParse(this.Pins["In"][2].GetValue(), out var falseInput)) falseInput = 0;

                    return condition == 1 ? trueInput.ToString() : falseInput.ToString();
                case "string":
                    return condition == 1 ? this.Pins["In"][1].GetValue() : this.Pins["In"][2].GetValue();

                // TODO: Add other data types
            }

            return "-2";
        }

        public override void OnRightClick(Pin pin = null)
        {
            this.Settings["DataType"] = DataTypes[(DataTypes.IndexOf(this.Settings["DataType"]) + 1) % DataTypes.Count];

            this.Pins["In"][1].DataType = this.Settings["DataType"];
            this.Pins["In"][2].DataType = this.Settings["DataType"];
            this.Pins["Out"][0].DataType = this.Settings["DataType"];
        }

        private static readonly List<string> DataTypes = new List<string>
        {
            "bool",
            "int",
            "string",
            //"teamColor",
            //"point",
            //"area"
        };
    }
}
