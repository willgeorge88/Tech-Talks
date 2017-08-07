using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Spi;
using BrainPadPins = GHIElectronics.TinyCLR.Pins.BrainPad;

namespace LEDRing {
    class Program {
        public void BrainPadSetup() {
            var spiSettings = new SpiConnectionSettings(BrainPadPins.GpioPin.Cs) {
                ClockFrequency = 12_000_000,
                DataBitLength = 8,
                Mode = SpiMode.Mode0
            };

            var gpioController = GpioController.GetDefault();
            var mr = gpioController.OpenPin(BrainPadPins.GpioPin.Rst);
            var spi = SpiDevice.FromId(BrainPadPins.SpiBus.Spi1, spiSettings);

            mr.SetDriveMode(GpioPinDriveMode.Output);
            mr.Write(GpioPinValue.High);

            var count = 1;
            var data = new byte[4];

            while (true) {
                count <<= 1;

                if (count == 0)
                    count = 1;

                data[0] = (byte)(count >> 0);
                data[1] = (byte)(count >> 8);
                data[2] = (byte)(count >> 16);
                data[3] = (byte)(count >> 24);

                spi.Write(data);

                BrainPad.Wait.Milliseconds(BrainPad.LightSensor.ReadLightLevel() * 3);
            }
        }

        public void BrainPadLoop() {
            //Put your program code here. It runs repeatedly after the BrainPad starts up.

            BrainPad.LightBulb.TurnWhite();
            BrainPad.Wait.Seconds(1);
            BrainPad.LightBulb.TurnOff();
            BrainPad.Wait.Seconds(1);
        }
    }
}
