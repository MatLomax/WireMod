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

		public TileInfo(int x, int y)
		{
			this.X = x;
			this.Y = y;
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

			this.X = x;
			this.Y = y;
		}

		public override string ToString() => $"{this.X},{this.Y}";
	}
}