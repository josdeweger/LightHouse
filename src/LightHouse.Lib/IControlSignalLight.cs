namespace LightHouse.Lib
{
    public interface IControlSignalLight
    {
        bool IsConnected { get; }
        void TurnOnColor(SignalLightColor color, byte brightness = 5, bool flash = false);
        void TurnOnBuzzer(byte frequency, byte repeatCount, byte onTime, byte offTime);
        void TurnOffBuzzer();
        void TurnOffColor();
        void TurnOffAll();
        void SetBrightness(byte brightness);
        void Test(byte brightness = 5);
    }
}
