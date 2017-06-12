//http://wiki.seeedstudio.com/wiki/Grove_-_Circular_LED
//http://wiki.seeedstudio.com/images/7/72/Circular_LED_v0.9b.pdf
//http://www.my-semi.com/file/MY9221_BF_0.7.pdf

using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Pins;
using System.Threading;

namespace BitBangingPart2 {
    public class Program {
        static GpioController GPIO = GpioController.GetDefault();
        static GpioPin ClockPin = GPIO.OpenPin(FEZPandaIII.GpioPin.D7);
        static GpioPin DataPin = GPIO.OpenPin(FEZPandaIII.GpioPin.D6);
        static ushort CmdMode = 0;

        static void Send16bit(ushort data) {
            for (int i = 0; i < 16; i++) {
                if ((data & 0x8000) > 0)
                    DataPin.Write(GpioPinValue.High);
                else
                    DataPin.Write(GpioPinValue.Low);

                if (ClockPin.Read() == GpioPinValue.High)
                    ClockPin.Write(GpioPinValue.Low);
                else
                    ClockPin.Write(GpioPinValue.High);

                data <<= 1;
            }
        }

        static void WriteLEDs(ushort[] leds) {
            Send16bit(CmdMode);

            Thread.Sleep(1);

            if (ClockPin.Read() == GpioPinValue.High)
                ClockPin.Write(GpioPinValue.Low);
            else
                ClockPin.Write(GpioPinValue.High);

            if (ClockPin.Read() == GpioPinValue.High)
                ClockPin.Write(GpioPinValue.Low);
            else
                ClockPin.Write(GpioPinValue.High);

            if (ClockPin.Read() == GpioPinValue.High)
                ClockPin.Write(GpioPinValue.Low);
            else
                ClockPin.Write(GpioPinValue.High);

            if (ClockPin.Read() == GpioPinValue.High)
                ClockPin.Write(GpioPinValue.Low);
            else
                ClockPin.Write(GpioPinValue.High);

            for (int count = 0; count < 12; count++) {
                Send16bit(leds[count]);
            }

            Send16bit(CmdMode);

            if (ClockPin.Read() == GpioPinValue.High)
                ClockPin.Write(GpioPinValue.Low);
            else
                ClockPin.Write(GpioPinValue.High);

            for (int count = 12; count < 24; count++) {
                Send16bit(leds[count]);
            }

            //latch
            for (int i = 0; i < 8; i++) {
                if (DataPin.Read() == GpioPinValue.High)
                    DataPin.Write(GpioPinValue.Low);
                else
                    DataPin.Write(GpioPinValue.High);
            }
        }

        public static void Main() {
            ClockPin.SetDriveMode(GpioPinDriveMode.Output);
            DataPin.SetDriveMode(GpioPinDriveMode.Output);

            ushort[] leds = new ushort[24];// 24 LEDs
            int k = 0;

            while (true) {
                if (leds[k] > 0)
                    leds[k] = 0;
                else
                    leds[k] = 0x0f;

                k++;
                if (k == 24)
                    k = 0;

                WriteLEDs(leds);

                Thread.Sleep(30);
            }
        }
    }
}
