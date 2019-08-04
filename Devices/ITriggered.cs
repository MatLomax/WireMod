namespace WireMod.Devices
{
    public interface ITriggered
    {
        void Trigger(Pin pin = null);
    }
}
