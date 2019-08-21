using System.Collections.Generic;
using Terraria.DataStructures;

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

            this.AutoTypes.AddRange(new [] {"int", "bool", "string"});
           
            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("In", 0, new Point16(1, 0), "bool", "Condition"),
                new PinDesign("In", 1, new Point16(0, 1), "int", "TrueValue", true),
                new PinDesign("In", 2, new Point16(2, 1), "int", "FalseValue", true),
                new PinDesign("Out", 0, new Point16(1, 2), "int", "", true),
            };
        }

        public string Output(Pin pin = null)
        {
            if (!this.GetPin("Condition").IsConnected()) return "-1";
            if (!int.TryParse(this.GetPin("Condition").GetValue(), out var condition)) return "-2";

            switch (this.DetectType())
            {
                case "auto": return "-1";
                case "bool":
                case "int":
                    if (!int.TryParse(this.GetPin("TrueValue").GetValue(), out var trueInput)) trueInput = 0;
                    if (!int.TryParse(this.GetPin("FalseValue").GetValue(), out var falseInput)) falseInput = 0;
                    return condition == 1 ? trueInput.ToString() : falseInput.ToString();
                case "string":
                    return condition == 1 ? this.GetPin("TrueValue").GetValue() : this.GetPin("FalseValue").GetValue();

                // TODO: Add other data types
            }

            return "-2";
        }
    }
}
