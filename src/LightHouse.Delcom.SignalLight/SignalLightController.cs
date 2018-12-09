using System;
using HidSharp;
using LightHouse.Lib;
using Serilog;

namespace LightHouse.Delcom.SignalLight
{
    public class SignalLightController : IControlSignalLight, IDisposable
    {
        private readonly ILogger _logger;
        private const int VendorId = 0x0fc5;
        private const int ProductId = 0xb080;
        private readonly HidStream _stream;

        private const int Green = 0xFE;
        private const int Red = 0xFD;
        private const int Orange = 0xFB;
        private const int Off = 0xFF;
        private const int BuzzerOn = 1;
        private const int BuzzerOff = 0;

        private const int EightBytesFlag = 101;
        private const int SixteenBytesFlag = 102;
        private const int SolidCommand = 2;
        private const int FlashCommand = 20;
        private const int BuzzCommand = 70;

        public bool IsConnected { get; }
        
        public SignalLightController(ILogger logger)
        {
            _logger = logger;

            _logger.Information($"Getting HID device with vendor id: {VendorId} and product id: {ProductId}");
            try
            {
                if (DeviceList.Local.TryGetHidDevice(out var signalLight, VendorId, ProductId))
                {
                    _logger.Information("Signal Light device found!");
                    _logger.Information("Opening device");
                    _stream = signalLight.Open();
                    IsConnected = true;
                }
                else
                {
                    IsConnected = false;
                    _logger.Fatal("Signal Light device not found!");
                }
            }
            catch (Exception e)
            {
                IsConnected = false;
                _logger.Fatal($"Signal Light device threw an exception with message {e.Message}");
            }
        }

        public void TurnOnColor(SignalLightColor color, bool flash = false)
        {
            TurnOffColor();
            
            switch (color)
            {
                case SignalLightColor.Green:
                    WriteToDevice(new byte[] { EightBytesFlag, SolidCommand, Green });
                    if (flash)
                        WriteToDevice(new byte[] { EightBytesFlag, FlashCommand, 0, 1 });
                    break;
                case SignalLightColor.Red:
                    WriteToDevice(new byte[] { EightBytesFlag, SolidCommand, Red });
                    if(flash)
                        WriteToDevice(new byte[] {EightBytesFlag, FlashCommand, 0, 2});
                    break;
                case SignalLightColor.Orange:
                    WriteToDevice(new byte[] { EightBytesFlag, SolidCommand, Orange });
                    if (flash)
                        WriteToDevice(new byte[] { EightBytesFlag, FlashCommand, 0, 4 });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(color), color, null);
            }
        }

        public void TurnOnBuzzer(byte frequency, byte repeatCount, byte onTime, byte offTime)
        {
            var writeData = new byte[] { SixteenBytesFlag, BuzzCommand, BuzzerOn, frequency, 0, 0, 0, 0, repeatCount, onTime, offTime };
            WriteToDevice(writeData);
        }

        public void TurnOffBuzzer()
        {
            WriteToDevice(new byte[] { SixteenBytesFlag, BuzzCommand, BuzzerOff });
        }

        public void TurnOffColor()
        {
            WriteToDevice(new byte[] { EightBytesFlag, SolidCommand, Off });
            WriteToDevice(new byte[] { EightBytesFlag, FlashCommand, Off });
        }

        public void TurnOffAll()
        {
            TurnOffColor();
            TurnOffBuzzer();
        }

        public void Dispose()
        {
            _logger.Information("Turning off Signal Light lights and buzzer");
            TurnOffAll();

            _logger.Information("Disposing Signal Light of stream");
            _stream?.Dispose();
        }

        private void WriteToDevice(byte[] values)
        {
            if (values[0] == EightBytesFlag)
            {
                _stream.SetFeature(values.PadRight());
            }
            else if(values[0] == SixteenBytesFlag)
            {
                _stream.SetFeature(values.PadRight(16));
            }
        }
    }
}