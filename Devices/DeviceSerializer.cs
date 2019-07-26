using System;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.IO;

namespace WireMod.Devices
{
	public class DeviceSerializer : TagSerializer<Device, TagCompound>
	{
		public override TagCompound Serialize(Device device)
		{
			return new TagCompound
			{
				["name"] = device.GetType().Name,
				["x"] = device.LocationRect.X + device.Origin.X,
				["y"] = device.LocationRect.Y + device.Origin.Y,
				["value"] = device.Value,
			};
		}

		public override Device Deserialize(TagCompound tag)
		{
			var device = (Device)Activator.CreateInstance(Type.GetType("WireMod.Devices." + tag.GetString("name")) ?? throw new InvalidOperationException("Device not found!"));
			device.LocationRect = new Rectangle(tag.GetInt("x"), tag.GetInt("y"), device.Width, device.Height);
			device.Value = tag.GetString("value");
            
			return device;
		}
	}
}