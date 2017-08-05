using GHIElectronics.TinyCLR.Devices.Spi;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Pins;

namespace LED_ring {
    class Program {
        public void BrainPadSetup() {
            var settings = new SpiConnectionSettings(GHIElectronics.TinyCLR.Pins.BrainPad.GpioPin.Cs) {
                ClockFrequency = 12 * 1000 * 1000,
                DataBitLength = 8,
                Mode = SpiMode.Mode0

            };

            var spi = SpiDevice.FromId(GHIElectronics.TinyCLR.Pins.BrainPad.SpiBus.Spi1,settings);
            var mr = GpioController.GetDefault().OpenPin(GHIElectronics.TinyCLR.Pins.BrainPad.GpioPin.Rst);
            mr.SetDriveMode(GpioPinDriveMode.Output);

            mr.Write(GpioPinValue.High);
            int count = 1;
            byte[] data = new byte[4];
            while (true) {
                count <<=1;
                if (count == 0)
                    count = 1;
                //count++;
                data[0] = (byte)(count >> 0);
                data[1] = (byte)(count >> 8);
                data[2] = (byte)(count >> 8 + 8);
                data[3] = (byte)(count >> 8 + 8 + 8);
                spi.Write(data);

                BrainPad.Wait.Milliseconds(BrainPad.LightSensor.ReadLightLevel()*3);
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
