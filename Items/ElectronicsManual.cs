using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WireMod.Devices;
using WireMod.UI;

namespace WireMod.Items
{
    internal class ElectronicsManual : ModItem
    {
        private Pin ConnectingPin;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electronics Manual");
            Tooltip.SetDefault("Grants use of WireMod functionality");
        }
        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.ActuationRod);
            item.autoReuse = false;
            item.holdStyle = 0;
            item.noMelee = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override void HoldItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server) return;

            var modPlayer = player.GetModPlayer<WireModPlayer>(WireMod.Instance);
            ElectronicsManualUI.Visible = true;
            ElectronicsVisionUI.Visible = true;
            modPlayer.ShowPreview = false;


            if (modPlayer.PlacingDevice != null)
            {
                if (modPlayer.ToolCategoryMode > 0)
                {
                    modPlayer.ShowPreview = true;
                }
            }
            else
            {
                var dev = WireMod.GetDevice((int)(Main.MouseWorld.X / 16f), (int)(Main.MouseWorld.Y / 16f));
                if (dev == null) return;

                //Main.NewText("Debug");

                WireMod.Instance.DebuggerUserInterface.SetState(new DebuggerUI(dev.Name, dev.Debug()));
                DebuggerUI.Visible = true;
            }
        }

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server) return false;

            var modPlayer = player.GetModPlayer<WireModPlayer>(WireMod.Instance);

            // Replace this - does not work with zoom
            //var x = (int)(Main.MouseWorld.X / 16f);
            //var y = (int)(Main.MouseWorld.Y / 16f);
            var (x, y) = WireMod.Instance.GetMouseTilePosition();
            var device = WireMod.GetDevice(x, y);

            var pin = WireMod.Pins.FirstOrDefault(p => p.Location == new Point16(x, y));

            // No tool selected
            if (modPlayer.ToolMode == -1)
            {
                if (device == null) return false;

                if (player.altFunctionUse == 2)
                {
                    // Right click
                    device.OnRightClick(pin);

                    return true;
                }

                // Do something?
            }

            // Wiring tools
            if (modPlayer.ToolCategoryMode == 0)
            {
                if (modPlayer.ToolMode == 0)
                {
                    if (pin == null) return true;

                    if (player.altFunctionUse == 2)
                    {
                        pin.Disconnect();

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            WireMod.PacketHandler.SendDisconnect(256, Main.myPlayer, x, y);
                        }

                        return true;
                    }

                    #region Connect Wires
                    // Connect wires
                    if (this.ConnectingPin == null)
                    {
                        Main.NewText("Connecting...");
                        this.ConnectingPin = pin;
                        return true;
                    }

                    if (this.ConnectingPin.Type == pin.Type)
                    {
                        Main.NewText("Cancelled - must connect a PinIn to a PinOut (or vice versa)");
                        this.ConnectingPin = null;
                        return false;
                    }

                    if (this.ConnectingPin.Device == pin.Device)
                    {
                        Main.NewText("Cancelled - cannot connect to same device");
                        this.ConnectingPin = null;
                        return false;
                    }

                    if (this.ConnectingPin.DataType != pin.DataType)
                    {
                        Main.NewText("Cancelled - cannot connect different data types");
                        this.ConnectingPin = null;
                        return false;
                    }

                    if (this.ConnectingPin.Device.Pins.SelectMany(p => p.Value.Values).Any(p => p.ConnectedPin?.Device == pin.Device))
                    {
                        Main.NewText("Cancelled - circular connection detected");
                        this.ConnectingPin = null;
                        return false;
                    }

                    this.ConnectingPin.ConnectedPin = pin;
                    pin.ConnectedPin = this.ConnectingPin;

                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        WireMod.PacketHandler.SendConnect(256, Main.myPlayer, this.ConnectingPin.Location.X, this.ConnectingPin.Location.Y, pin.Location.X, pin.Location.Y);
                    }

                    this.ConnectingPin = null;

                    return true;
                    #endregion
                }

                if (modPlayer.ToolMode == 1)
                {
                    // Delete device
                    if (device == null) return false;

                    WireMod.RemoveDevice(device);

                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        WireMod.PacketHandler.SendRemove(256, Main.myPlayer, x, y);
                    }

                    return true;
                }
            }

            if (device != null) return false;

            // Find microchips in inventory
            var microChipIndex = -1;
            for (var i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].type != mod.ItemType<MicrochipItem>()) continue;

                microChipIndex = i;
                break;
            }

            // No microchips in inventory
            if (microChipIndex == -1) return false;
            if (modPlayer.PlacingDevice == null) return false;
            if (!WireMod.CanPlace(modPlayer.PlacingDevice, x, y)) return false;

            // Place the device
            WireMod.PlaceDevice(modPlayer.PlacingDevice, x, y);

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                WireMod.PacketHandler.SendPlace(256, Main.myPlayer, modPlayer.PlacingDevice.GetType().Name, modPlayer.PlacingDevice.Value ?? "", x, y);
            }

            // Consume a microchip
            player.inventory[microChipIndex].stack--;

            // Reset
            modPlayer.PlacingDevice = null;
            modPlayer.ToolCategoryMode = -1;
            modPlayer.ToolMode = -1;

            return true;
        }

        public override bool UseItem(Player player)
        {
            ////Main.NewText("Begin");
            //this.usingItem = (this.usingItem + 1) % 3;

            //if (this.usingItem > 0) return true;

            //var ent = mod.GetTileEntity<DeviceEntity>();

            //var point = new Point16((int)(Main.MouseWorld.X / 16f), (int)(Main.MouseWorld.Y / 16f));
            //var drp = ent.GetTileDevice(point);

            //// No device found
            //if (drp.DeviceRef == null)
            //{
            //    ent.CurrentDevice = null;
            //    ent.CurrentPin = null;
            //    return true;
            //}

            //// Right Click
            //if (player.altFunctionUse == 2)
            //{
            //    // TODO: Add Right Click device non-pin behaviour
            //    if (drp.Pin == null)
            //    {
            //        ent.CurrentDevice = null;
            //        ent.CurrentPin = null;
            //        //Main.NewText($"No pin found");
            //        return true;
            //    }

            //    if (!drp.Pin.IsConnected()) return true;

            //    //Main.NewText($"Disconnected: Pin{drp.Pin.Type}{drp.Pin.Num} <=> Pin{drp.Pin.ConnectedPin.Type}{drp.Pin.ConnectedPin.Num}");
            //    drp.Pin.ConnectedPin.ConnectedPin = null;
            //    drp.Pin.ConnectedPin = null;
            //    ent.CurrentDevice = null;

            //    mod.GetTileEntity<DeviceEntity>().Changes = true;

            //    return true;
            //}

            //// Left Click
            //if (drp.Pin != null)
            //{
            //    // Found Pin
            //    if (ent.CurrentPin == null)
            //    {
            //        if (!drp.Pin.IsConnected())
            //        {
            //            ent.CurrentDevice = drp.DeviceRef.Device;
            //            ent.CurrentPin = drp.Pin;
            //            Main.NewText($"Connecting Pin{drp.Pin.Type}{drp.Pin.Num}...");
            //        }
            //        else
            //        {
            //            //Main.NewText($"Pin{drp.Pin.Type}{drp.Pin.Num} [{(drp.Pin.DataType == "bool" ? (drp.Pin.GetValue() == "1" ? "True" : "False") : drp.Pin.GetValue())}] => Connected to [{drp.Pin.ConnectedPin.Device.Name}]#{drp.Pin.ConnectedPin.Device.Ref} Pin{drp.Pin.ConnectedPin.Type}{drp.Pin.ConnectedPin.Num}");
            //        }
            //    }
            //    else
            //    {
            //        if (ent.CurrentDevice == drp.DeviceRef.Device)
            //        {
            //            if (ent.CurrentPin != drp.Pin)
            //            {
            //                Main.NewText($"Cancelled - Same device");
            //                ent.CurrentDevice = null;
            //                ent.CurrentPin = null;
            //            }
            //        }
            //        else
            //        {
            //            if (drp.Pin.Type == ent.CurrentPin.Type)
            //            {
            //                Main.NewText($"Must connect a PinOut to a PinIn (and vice versa)");
            //                return true;
            //            }
            //            else
            //            {
            //                if (drp.Pin.IsConnected())
            //                {
            //                    Main.NewText($"Target pin is already connected");
            //                    return true;
            //                }
            //                else
            //                {
            //                    drp.Pin.ConnectedPin = ent.CurrentPin;
            //                    ent.CurrentPin.ConnectedPin = drp.Pin;

            //                    ent.CurrentDevice = null;
            //                    ent.CurrentPin = null;

            //                    mod.GetTileEntity<DeviceEntity>().Changes = true;
            //                    //Main.NewText($"Connected: [{drp.Pin.Device.Name}]#{drp.Pin.Device.Ref} Pin{drp.Pin.Type}{drp.Pin.Num} <=> [{drp.Pin.ConnectedPin.Device.Name}]#{drp.Pin.ConnectedPin.Device.Ref} Pin{drp.Pin.ConnectedPin.Type}{drp.Pin.ConnectedPin.Num}");
            //                }
            //            }
            //        }
            //    }
            //}

            return true;
        }
    }
}
