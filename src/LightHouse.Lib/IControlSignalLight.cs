namespace LightHouse.Lib
{
    public interface IControlSignalLight
    {
        bool IsConnected { get; }
        void TurnOnColor(SignalLightColor color, bool flash = false);
        void TurnOnBuzzer(byte frequency, byte repeatCount, byte onTime, byte offTime);
        void TurnOffBuzzer();
        void TurnOffColor();
        void TurnOffAll();
    }
}
