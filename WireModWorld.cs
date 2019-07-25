using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
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
				["dest"] = ((PinIn)p).ConnectedPin.Location
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

				//if (Main.netMode == NetmodeID.Server)
				//{
				//	WireMod.PacketHandler.SendPlace(-1, -1, device.Name, device.Value, device.LocationRect.X, device.LocationRect.Y);
				//}

				WireMod.PlaceDevice(device, device.LocationRect.X, device.LocationRect.Y);
			}

			foreach (var conn in tag.GetList<TagCompound>("wires"))
			{
				var src = WireMod.GetDevicePin(conn.Get<Point16>("src"));
				var dest = WireMod.GetDevicePin(conn.Get<Point16>("dest"));
				if (src == null || dest == null) continue;

				src.Connect(dest);
				dest.Connect(src);
			}
		}
	}
}