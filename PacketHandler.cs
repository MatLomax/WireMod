using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WireMod.Devices;

namespace WireMod
{
	internal abstract class PacketHandler
	{
		internal byte HandlerType { get; set; }

		public abstract void HandlePacket(BinaryReader reader, int from);

		protected PacketHandler(/*byte handlerType*/)
		{
			//HandlerType = handlerType;
		}

		protected ModPacket GetPacket(byte packetType, int fromWho)
		{
			var p = WireMod.Instance.GetPacket();
			//p.Write(HandlerType);
			p.Write(packetType);
			//if (Main.netMode == NetmodeID.Server)
			//{
			//	p.Write((byte)fromWho);
			//}
			return p;
		}
	}

	internal class DevicePacketHandler : PacketHandler
	{
		public const byte Place = 1;
		public const byte Remove = 2;
		public const byte Connect = 3;
		public const byte Disconnect = 4;
		public const byte ChangeValue = 5;
		public const byte TripWire = 6;
		public const byte Request = 7;

		public DevicePacketHandler(/*byte handlerType*/) /*: base(handlerType)*/
		{
		}

		public override void HandlePacket(BinaryReader reader, int from)
		{
			switch (reader.ReadByte())
			{
				case Place:
					ReceivePlace(reader, from);
					break;
				case Remove:
					ReceiveRemove(reader, from);
					break;
				case Connect:
					ReceiveConnect(reader, from);
					break;
				case Disconnect:
					ReceiveDisconnect(reader, from);
					break;
				case ChangeValue:
					ReceiveChangeValue(reader, from);
					break;
				case TripWire:
					ReceiveTripWire(reader, from);
					break;
				case Request:
					ReceiveRequest(reader, from);
					break;
			}
		}

		#region Place
		public void SendPlace(int to, int from, string name, string value, int x, int y)
		{
			var packet = GetPacket(Place, from);

			packet.Write(name);
			packet.Write(x);
			packet.Write(y);
			packet.Write(value);

			packet.Send(to, from);
		}

		public void ReceivePlace(BinaryReader reader, int from)
		{
            var name = reader.ReadString();
			var x = reader.ReadInt32();
			var y = reader.ReadInt32();
			var value = reader.ReadString();
            
			WireMod.Instance.Logger.Info($"Received Place: name {name}, value {value}, x {x}, y {y}");
			
			if (Main.netMode == NetmodeID.Server)
			{
				SendPlace(-1, from, name, value, x, y);
			}
			//else
			//{
				var device = (Device)Activator.CreateInstance(Type.GetType("WireMod.Devices." + name) ?? throw new InvalidOperationException("Device not found!"));
				device.LocationRect = new Rectangle(x, y, device.Width, device.Height);
				device.Value = value;

				WireMod.PlaceDevice(device, x, y);
			//}
		}
		#endregion

		#region Request
		public void SendRequest(int to, int from)
		{
			var packet = GetPacket(Place, from);

			packet.Write(from);

			packet.Send(to, from);
		}

		public void ReceiveRequest(BinaryReader reader, int from)
		{
			from = reader.ReadInt32();

			WireMod.Instance.Logger.Info($"Received Request from {Main.player[from].name}");

			if (Main.netMode != NetmodeID.Server) return;

			foreach (var device in WireMod.Devices)
			{
				this.SendPlace(from, 256, device.GetType().Name, device.Value, device.LocationRect.X, device.LocationRect.Y);
			}
		}
		#endregion

		#region Remove
		public void SendRemove(int to, int from, int x, int y)
		{
			var packet = GetPacket(Remove, from);
			packet.Write(x);
			packet.Write(y);
			packet.Send(to, from);
		}

		public void ReceiveRemove(BinaryReader reader, int from)
		{
			var x = reader.ReadInt32();
			var y = reader.ReadInt32();

			WireMod.Instance.Logger.Info($"Received Remove: x {x}, y {y}");
			
			if (Main.netMode == NetmodeID.Server)
			{
				SendRemove(-1, from, x, y);
			}
			//else
			//{
				WireMod.RemoveDevice(x, y);
			//}
		}
		#endregion

		#region Connect
		public void SendConnect(int to, int from, int srcX, int srcY, int destX, int destY)
		{
			var packet = GetPacket(Connect, from);

			packet.Write(srcX);
			packet.Write(srcY);
			packet.Write(destX);
			packet.Write(destY);

			packet.Send(to, from);
		}

		public void ReceiveConnect(BinaryReader reader, int from)
		{
			var srcX = reader.ReadInt32();
			var srcY = reader.ReadInt32();
			var destX = reader.ReadInt32();
			var destY = reader.ReadInt32();
		
			WireMod.Instance.Logger.Info($"Received Connect: srcX {srcX}, srcY {srcY}, destX {destX}, destY {destY}");

			if (Main.netMode == NetmodeID.Server)
			{
				this.SendConnect(-1, from, srcX, srcY, destX, destY);
			}
			//else
			//{
				var src = WireMod.GetDevicePin(srcX, srcY);
				var dest = WireMod.GetDevicePin(destX, destY);
				if (src == null || dest == null) return;

				src.Disconnect();
				dest.Disconnect();
				src.ConnectedPin = dest;
				dest.ConnectedPin = src;
			//}
		}
		#endregion

		#region Disconnect
		public void SendDisconnect(int to, int from, int x, int y)
		{
			var packet = GetPacket(Disconnect, from);

			packet.Write(x);
			packet.Write(y);

			packet.Send(to, from);
		}

		public void ReceiveDisconnect(BinaryReader reader, int from)
		{
			var x = reader.ReadInt32();
			var y = reader.ReadInt32();

			WireMod.Instance.Logger.Info($"Received Disconnect: x {x}, y {y}");
			
			if (Main.netMode == NetmodeID.Server)
			{
				this.SendDisconnect(-1, from, x, y);
			}
			//else
			//{
				var src = WireMod.GetDevicePin(x, y);
				src?.Disconnect();
			//}
		}
		#endregion

		#region Change Value
		public void SendChangeValue(int to, int from, int x, int y, string value)
		{
			var packet = GetPacket(ChangeValue, from);
			packet.Write(x);
			packet.Write(y);
			packet.Write(value);
			packet.Send(to, from);
		}

		public void ReceiveChangeValue(BinaryReader reader, int from)
		{
			var x = reader.ReadInt32();
			var y = reader.ReadInt32();
			var value = reader.ReadString();

			WireMod.Instance.Logger.Info($"Received Disconnect: x {x}, y {y}, value {value}");

			if (Main.netMode == NetmodeID.Server)
			{
				this.SendChangeValue(-1, from, x, y, value);
			}
			//else
			//{
				var device = WireMod.GetDevice(x, y);
				if (device == null) return;

				device.Value = value;
			//}
		}
		#endregion
		
		#region TripWire
		public void SendTripWire(int to, int from, int x, int y)
		{
			var packet = GetPacket(TripWire, from);

			packet.Write(x);
			packet.Write(y);

			packet.Send(to, from);
		}

		public void ReceiveTripWire(BinaryReader reader, int from)
		{
			var x = reader.ReadInt32();
			var y = reader.ReadInt32();

			WireMod.Instance.Logger.Info($"Received TripWire: x {x}, y {y}");

			if (Main.netMode == NetmodeID.Server)
			{
				Wiring.TripWire(x, y, 1, 1);
			}
		}
		#endregion
	}
}