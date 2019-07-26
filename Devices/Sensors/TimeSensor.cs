using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;

namespace WireMod.Devices
{
	internal class TimeSensor : Device, IOutput
	{
		public TimeSensor()
		{
			this.Name = "Time Sensor";
			this.Width = 3;
			this.Height = 2;
			this.Origin = new Point16(1, 0);

			this.PinLayout = new List<PinDesign>
			{
				new PinDesign("Out", 0, new Point16(0, 0), "int", "Hours"),
				new PinDesign("Out", 1, new Point16(1, 1), "int", "Minutes"),
				new PinDesign("Out", 2, new Point16(2, 0), "int", "Seconds"),
			};
		}

		public string Output(Pin pin = null) => this.GetOutput(pin);

		private string GetOutput(Pin pin = null)
		{
			var time = Main.time;

			var hours = (int)(((((time / 60) + 30) / 60) + 19) % 24);
			if (Main.dayTime) hours = (hours + 9) % 24;
			var minutes = (int)((((time % 3600) / 60) + 30) % 60);
			var seconds = (int)(time % 60);

			if (pin != null)
			{
				switch (pin.Num)
				{
					case 0:
						return hours.ToString();
					case 1:
						return minutes.ToString();
					case 2:
						return seconds.ToString();
				}
			}
			
			return $"{hours}:{minutes}:{seconds}";
		}
	}
}
