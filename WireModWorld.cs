using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WireMod.Devices;

namespace WireMod
{
	public class WireModWorld : ModWorld
	{
		public override TagCompound Save()
		{
			var wires = WireMod.Pins.Where(p => p.Type == "In" && p.IsConnected()).Select(p => new TagCompound
			{
				["src"] = p.Location,
				["dest"] = ((PinIn)p).ConnectedPin.Location,
				["points"] = p.GetWire(((PinIn)p).ConnectedPin).GetPoints(p, false),
			}).ToList();
			
			return new TagCompound
			{
				["devices"] = WireMod.Devices,
				["wires"] = wires
			};
		}
		public override void Load(TagCompound tag)
		{
			foreach (var device in tag.Get<List<Device>>("devices"))
			{
				mod.Logger.Info($"Loading device \"{device.Name}\": X {device.LocationRect.X}, Y: {device.LocationRect.Y}");
				
				WireMod.PlaceDevice(device, device.LocationRect.X, device.LocationRect.Y);
			}

			foreach (var conn in tag.GetList<TagCompound>("wires"))
			{
				var src = WireMod.GetDevicePin(conn.Get<Point16>("src"));
				var dest = WireMod.GetDevicePin(conn.Get<Point16>("dest"));
				if (src == null || dest == null) continue;

				var points = conn.GetList<Point16>("points").ToList();
				var wire = new Wire(src, dest, points);

				src.Connect(dest, wire);
				dest.Connect(src, wire);
			}
		}
	}
}