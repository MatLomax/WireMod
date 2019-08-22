namespace WireMod.Devices
{
	public class TileInfo
	{
		public int X { get; set; }
		public int Y { get; set; }
		public bool Active { get; set; }
		public ushort Type { get; set; }
		public ushort Wall { get; set; }
		public byte Slope { get; set; }

		public TileInfo()
		{
		}

		public TileInfo(string data)
		{
			this.Load(data);
		}

		public void Load(string data)
		{
			var fields = data.Split(',');

			if (!int.TryParse(fields[0], out var x)) return;
			if (!int.TryParse(fields[1], out var y)) return;
			if (!ushort.TryParse(fields[2], out var type)) return;
			if (!ushort.TryParse(fields[3], out var wall)) return;
			if (!byte.TryParse(fields[4], out var slope)) return;
			if (!bool.TryParse(fields[5], out var active)) return;

			this.X = x;
			this.Y = y;
			this.Type = type;
			this.Wall = wall;
			this.Slope = slope;
			this.Active = active;
		}

		public override string ToString() => $"{this.X},{this.Y},{this.Type},{this.Wall},{this.Slope},{this.Active}";
	}
}