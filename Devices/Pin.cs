using Terraria.DataStructures;

namespace WireMod.Devices
{
    public abstract class Pin
    {
        public Device Device { get; set; }
        public int Num { get; set; }
        public string Type { get; set; }
        public string DataType { get; set; } = "bool";
        public Pin ConnectedPin { get; set; }

        public Point16 Location { get; set; }


        private string _name = "";
        public string Name
        {
            get => !string.IsNullOrEmpty(this._name) ? this._name : $"Pin{this.Type}{this.Num}";
            set => this._name = value;
        }

        public bool IsConnected() => this.ConnectedPin != null;

        public void Disconnect()
        {
            if (!this.IsConnected()) return;
            this.ConnectedPin.ConnectedPin = null;
            this.ConnectedPin = null;
        }

        public abstract string GetValue();
    }

    internal class PinIn : Pin
    {
        private string _value => this.ConnectedPin?.GetValue();

        public bool GetBool()
        {
            if (!this.IsConnected()) return false;
            if (!int.TryParse(this._value, out var i)) return false;
            return i == 1;
        }

        public override string GetValue() => this._value;

        public PinIn(Device device)
        {
            this.Device = device;
            this.Type = "In";
        }
    }

    internal class PinOut : Pin
    {
        private string _value => (this.Device is IOutput ? ((IOutput)this.Device).Output() : "");

        public PinOut(Device device)
        {
            this.Device = device;
            this.Type = "Out";
        }

        public override string GetValue() => this._value;
    }
}
