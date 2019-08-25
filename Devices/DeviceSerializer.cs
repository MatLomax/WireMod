using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.IO;

namespace WireMod.Devices
{
	public class DeviceSerializer : TagSerializer<Device, TagCompound>
	{
		public override TagCompound Serialize(Device device)
		{
			var tag = new TagCompound
			{
				["name"] = device.GetType().Name,
				["x"] = device.LocationRect.X + device.Origin.X,
				["y"] = device.LocationRect.Y + device.Origin.Y,
			};

			foreach (var setting in device.Settings)
			{
				tag[setting.Key] = setting.Value;
			}

			return tag;
		}

		public override Device Deserialize(TagCompound tag)
		{
			var device = (Device)Activator.CreateInstance(Type.GetType("WireMod.Devices." + tag.GetString("name")) ?? throw new InvalidOperationException("Device not found!"));
			device.SetLocation(tag.GetInt("x"), tag.GetInt("y"));

			foreach (var setting in tag.Where(t => !new[] {"name", "x", "y"}.Contains(t.Key)))
			{
				if (!device.Settings.ContainsKey(setting.Key)) continue;
				device.Settings[setting.Key] = setting.Value.ToString();
			}
            
			return device;
		}
	}
}