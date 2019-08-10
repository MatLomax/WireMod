using System.Linq;
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

        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Wire, 50);
            recipe.AddIngredient(ItemID.Book);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool AltFunctionUse(Player player) => true;

        public override void HoldItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server) return;
            if (player != Main.LocalPlayer) return;

            var modPlayer = player.GetModPlayer<WireModPlayer>();
            
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
                var point = WireMod.Instance.GetMouseTilePosition();
                var dev = WireMod.GetDevice(point);
                if (dev == null) return;

                var pin = WireMod.GetDevicePin(point.X, point.Y);

                WireMod.Instance.DebuggerHoverUserInterface.SetState(new HoverDebuggerUI(dev.Debug(pin)));
                HoverDebuggerUI.Visible = true;
            }
        }

        public override bool CanUseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server) return false;
            if (player != Main.LocalPlayer) return false;

            var modPlayer = player.GetModPlayer<WireModPlayer>();

            var (x, y) = WireMod.Instance.GetMouseTilePosition();
            var device = WireMod.GetDevice(x, y);

            var pin = WireMod.Pins.FirstOrDefault(p => p.Location == new Point16(x, y));

            // Wiring tools
            if (modPlayer.ToolCategoryMode == 0)
            {
                // No tool selected
                if (modPlayer.ToolMode == 0)
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

                if (modPlayer.ToolMode == 1)
                {
                    if (player.altFunctionUse == 2)
                    {
                        if (device == null)
                        {
                            modPlayer.ConnectingPin = null;
                            modPlayer.PlacingWire = null;
                            return false;
                        }

                        if (pin == null)
                        {
                            device.OnRightClick();
                            return true;
                        }

                        pin.Disconnect();

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            WireMod.PacketHandler.SendDisconnect(256, Main.myPlayer, x, y);
                        }

                        return true;
                    }

                    if (pin == null)
                    {
                        modPlayer.PlacingWire.Points.Add(new Point16(x, y));
                        return false;
                    }

                    #region Connect Wires
                    // Connect wires
                    if (modPlayer.ConnectingPin == null)
                    {
                        Main.NewText("Connecting...");
                        modPlayer.ConnectingPin = pin;
                        modPlayer.PlacingWire = new Wire(pin);

                        return true;
                    }

                    if (modPlayer.ConnectingPin.Type == pin.Type)
                    {
                        Main.NewText("Cancelled - must connect a PinIn to a PinOut (or vice versa)");
                        modPlayer.ConnectingPin = null;
                        modPlayer.PlacingWire = null;
                        return false;
                    }

                    if (modPlayer.ConnectingPin.Device == pin.Device)
                    {
                        Main.NewText("Cancelled - cannot connect to same device");
                        modPlayer.ConnectingPin = null;
                        modPlayer.PlacingWire = null;
                        return false;
                    }

                    if (modPlayer.ConnectingPin.DataType != pin.DataType)
                    {
                        Main.NewText("Cancelled - cannot connect different data types");
                        modPlayer.ConnectingPin = null;
                        modPlayer.PlacingWire = null;
                        return false;
                    }

                    if (modPlayer.ConnectingPin.Device.Pins.SelectMany(p => p.Value.Values).Any(p =>
                    {
                        return p.Type == "In"
                            ? ((PinIn)p).ConnectedPin?.Device == pin.Device
                            : ((PinOut)p).ConnectedPins.Any(cp => ((PinIn)cp).ConnectedPin?.Device == pin.Device);
                    }))
                    {
                        Main.NewText("Cancelled - circular connection detected");
                        modPlayer.ConnectingPin = null;
                        modPlayer.PlacingWire = null;
                        return false;
                    }

                    modPlayer.PlacingWire.EndPin = pin;
                    modPlayer.ConnectingPin.Connect(pin, modPlayer.PlacingWire);
                    pin.Connect(modPlayer.ConnectingPin, modPlayer.PlacingWire);

                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        WireMod.PacketHandler.SendConnect(256, Main.myPlayer, modPlayer.ConnectingPin.Location.X, modPlayer.ConnectingPin.Location.Y, pin.Location.X, pin.Location.Y, modPlayer.PlacingWire);
                    }

                    modPlayer.ConnectingPin = null;
                    modPlayer.PlacingWire = null;

                    return true;
                    #endregion
                }

                if (modPlayer.ToolMode == 2)
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

                if (modPlayer.ToolMode == 3)
                {
                    if (player.altFunctionUse == 2)
                    {
                        WireMod.Instance.DebuggerUserInterface.SetState(null);
                        DebuggerUI.Visible = true;

                        return true;
                    }

                    // Debug device
                    if (device == null) return false;

                    WireMod.Instance.DebuggerUserInterface.SetState(new DebuggerUI(device));
                    DebuggerUI.Visible = true;
                }
            }

            if (device != null) return false;

            // Find microchips in inventory
            var microChipIndex = -1;
            for (var i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].type != mod.ItemType<Microchip>()) continue;

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
                WireMod.PacketHandler.SendPlace(256, Main.myPlayer, modPlayer.PlacingDevice.GetType().Name, modPlayer.PlacingDevice.Settings, x, y);
            }

            // Consume a microchip
            player.inventory[microChipIndex].stack--;

            // Reset
            modPlayer.PlacingDevice = null;
            modPlayer.ConnectingPin = null;
            modPlayer.PlacingWire = null;
            modPlayer.ToolCategoryMode = 0;
            modPlayer.ToolMode = 0;

            return true;
        }
    }
}
