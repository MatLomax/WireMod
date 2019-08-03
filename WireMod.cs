using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using WireMod.Devices;
using WireMod.UI;

namespace WireMod
{
    internal class WireMod : Mod
    {
        internal static WireMod Instance;
        internal const bool Debug = true;
        internal static DevicePacketHandler PacketHandler = new DevicePacketHandler();

        internal UserInterface ElectronicsManualUserInterface;
        internal UserInterface ElectronicsVisionUserInterface;
        internal UserInterface DebuggerUserInterface;
        internal UserInterface UserInputUserInterface;


        public WireMod()
        {
            Instance = this;
        }

        public override void Load()
        {
            Terraria.ModLoader.IO.TagSerializer.AddSerializer(new DeviceSerializer());

            if (Main.netMode == NetmodeID.Server) return;

            this.ElectronicsManualUserInterface = new UserInterface();
            this.ElectronicsVisionUserInterface = new UserInterface();
            this.DebuggerUserInterface = new UserInterface();
            this.UserInputUserInterface = new UserInterface();
        }

        public override void UpdateUI(GameTime gameTime)
        {
            // TODO: Fix this bullshit
            foreach (var device in Devices.Where(d => d.Pins["Out"].Count > 0)) device.Pins["Out"][0].GetValue();

            if (Main.netMode == NetmodeID.Server) return;

            if (DebuggerUI.Visible)
            {
                this.DebuggerUserInterface.Update(gameTime);
            }

            if (ElectronicsManualUI.Visible)
            {
                this.ElectronicsManualUserInterface.Update(gameTime);
            }

            if (ElectronicsVisionUI.Visible)
            {
                this.ElectronicsVisionUserInterface.Update(gameTime);
            }

            if (UserInputUI.Visible)
            {
                this.UserInputUserInterface.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if (Main.netMode == NetmodeID.Server) return;

            var mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex - 1, new LegacyGameInterfaceLayer(
                    "WireMod: Devices",
                    delegate
                    {
                        if (ElectronicsVisionUI.Visible)
                        {
                            this.ElectronicsVisionUserInterface?.Draw(Main.spriteBatch, new GameTime());
                            ElectronicsVisionUI.Visible = false;
                        }

                        if (ElectronicsManualUI.Visible)
                        {
                            this.ElectronicsManualUserInterface?.Draw(Main.spriteBatch, new GameTime());
                            ElectronicsManualUI.Visible = false;
                        }

                        if (DebuggerUI.Visible)
                        {
                            this.DebuggerUserInterface?.Draw(Main.spriteBatch, new GameTime());
                            DebuggerUI.Visible = false;
                        }

                        if (UserInputUI.Visible)
                        {
                            this.UserInputUserInterface?.Draw(Main.spriteBatch, new GameTime());
                        }

                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public override void HandlePacket(BinaryReader reader, int from) => PacketHandler.HandlePacket(reader, from);


        public (float X, float Y) GetMouseWorldPosition()
        {
            return (
                Main.mouseX + Main.screenPosition.X,
                Main.mouseY + Main.screenPosition.Y
            );
        }

        public (int X, int Y) GetMouseTilePosition()
        {
            var (x, y) = this.GetMouseWorldPosition();

            return (
                (int)(x / 16),
                (int)(y / 16)
            );
        }


        // For lack of a better place to put all this...

        public static List<Device> Devices = new List<Device>();
        public static List<Pin> Pins = new List<Pin>();

        #region Device Functions
        public static Device GetDevice(Point16 location) => GetDevice(location.X, location.Y);
        public static Device GetDevice((int X, int Y) location) => GetDevice(location.X, location.Y);
        public static Device GetDevice(int x, int y)
        {
            return Devices.FirstOrDefault(d => d.LocationRect.Intersects(new Rectangle(x, y, 1, 1)));
        }

        public static Pin GetDevicePin(Point16 location) => GetDevicePin(location.X, location.Y);
        public static Pin GetDevicePin(int x, int y)
        {
            return Pins.FirstOrDefault(p => p.Location.X == x && p.Location.Y == y);
        }

        public static bool CanPlace(Device device, Point16 location) => CanPlace(device, location.X, location.Y);
        public static bool CanPlace(Device device, int x, int y)
        {
            // Check for devices and pins in the new device destination area
            return !Devices.Any(d => d.LocationRect.Intersects(new Rectangle(
                x - device.Origin.X,
                y - device.Origin.Y,
                device.Width,
                device.Height
            )));
        }

        public static void PlaceDevice(Device device, Point16 location) => PlaceDevice(device, location.X, location.Y);
        public static void PlaceDevice(Device device, int x, int y)
        {
            // Check if the target area is clear of other devices
            if (!CanPlace(device, x, y)) return;

            // Add to arrays
            device.LocationRect = new Rectangle(x - device.Origin.X, y - device.Origin.Y, device.Width, device.Height);
            device.SetPins();
            Devices.Add(device);

            foreach (var pinDesign in device.PinLayout)
            {
                var pin = device.Pins[pinDesign.Type][pinDesign.Num];
                pin.Location = new Point16(device.LocationRect.X + pinDesign.Offset.X, device.LocationRect.Y + pinDesign.Offset.Y);
                Pins.Add(pin);
            }

            device.OnPlace();
        }

        public static void RemoveDevice(Point16 location) => RemoveDevice(location.X, location.Y);
        public static void RemoveDevice(int x, int y) => RemoveDevice(new Rectangle(x, y, 1, 1));
        public static void RemoveDevice(Rectangle rect)
        {
            var device = Devices.FirstOrDefault(d => d.LocationRect.Intersects(rect));

            if (device == null) return;

            RemoveDevice(device);
        }
        public static void RemoveDevice(Device device)
        {
            device.OnKill();

            // Disconnect from connected pins
            foreach (var pin in device.Pins.Values.SelectMany(p => p.Values))
            {
                pin.Disconnect();
                Pins.Remove(pin);
            }

            Devices.Remove(device);
        }
        #endregion
    }
}
