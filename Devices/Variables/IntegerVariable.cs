using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using WireMod.UI;

namespace WireMod.Devices
{
    internal class IntegerVariable : Device, IOutput
    {
        public IntegerVariable()
        {
            this.Name = "Variable Integer";

            this.Width = 3;
            this.Height = 2;
            this.Origin = new Point16(1, 0);


            this.Settings.Add("Value", "0");

            this.Settings.Add("WriteEnabled", "0");

           


            this.PinLayout = new List<PinDesign>
            {
                new PinDesign("Out", 0, new Point16(1, 1), "int"),
                new PinDesign("In",0, new Point16(0,0),"bool"),
                new PinDesign("In",1,new Point16(2,0),"int")
            };



        }


        public string Output(Pin pin = null) => this.GetOutput();

        private string GetOutput()
        {
            var WriteEnabledPin = this.Pins["In"][0];
            var InputIntPin = this.Pins["In"][1];

            if (WriteEnabledPin.IsConnected())
            {
                if (WriteEnabledPin.GetValue() != Settings["WriteEnabled"])
                {
                    Settings["WriteEnabled"] = WriteEnabledPin.GetValue();
                }
            }

            if(Settings["WriteEnabled"] == "1")
            {
                if (InputIntPin.IsConnected())
                {
                    if(InputIntPin.GetValue() != Settings["Value"])
                    {
                        Settings["Value"] = InputIntPin.GetValue();
                    }
                }
            }

            return Settings["Value"];
        }


        
    }
}