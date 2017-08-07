//http://wiki.seeedstudio.com/wiki/Grove_-_Circular_LED
//http://wiki.seeedstudio.com/images/7/72/Circular_LED_v0.9b.pdf
//http://www.my-semi.com/file/MY9221_BF_0.7.pdf

using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Pins;
using System.Threading;

namespace BitBangingPart2 {
    public class Program {
        private const ushort CmdMode = 0;

        private static GpioPin clock;
        private static GpioPin data;

        public static void Main() {
            var gpioController = GpioController.GetDefault();

            clock = gpioController.OpenPin(FEZ.GpioPin.D7);
            clock.SetDriveMode(GpioPinDriveMode.Output);

            data = gpioController.OpenPin(FEZ.GpioPin.D6);
            data.SetDriveMode(GpioPinDriveMode.Output);

            var leds = new ushort[24];
            var current = 0;

            while (true) {
                if (leds[current] > 0)
                    leds[current] = 0;
                else
                    leds[current] = 0x0F;

                current++;

                if (current == 24)
                    current = 0;

                WriteLEDs(leds);

                Thread.Sleep(30);
            }
        }

        private static void Write(ushort value) {
            for (var i = 0; i < 16; i++) {
                data.Write((value & 0x8000) > 0 ? GpioPinValue.High : GpioPinValue.Low);

                Toggle(clock);

                value <<= 1;
            }
        }

        private static void WriteLEDs(ushort[] leds) {
            Write(CmdMode);

            Thread.Sleep(1);

            Toggle(clock);
            Toggle(clock);
            Toggle(clock);
            Toggle(clock);

            for (var count = 0; count < 12; count++)
                Write(leds[count]);

            Write(CmdMode);

            Toggle(clock);

            for (var count = 12; count < 24; count++)
                Write(leds[count]);

            //Latch
            for (var i = 0; i < 8; i++)
                Toggle(data);
        }

        private static void Toggle(GpioPin pin) => pin.Write(pin.Read() == GpioPinValue.High ? GpioPinValue.Low : GpioPinValue.High);
    }
}
