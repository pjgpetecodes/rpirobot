using System;
using System.Device.Gpio;
using System.Threading;

namespace robot_firmware
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Hello World!");

            GpioController controller = new GpioController(PinNumberingScheme.Board);

            var pin = 10;
            var lightTime = 300;

            controller.OpenPin(pin, PinMode.Output);

            try
            {
                while(true)
                {
                    controller.Write(pin, PinValue.High);
                    Thread.Sleep(lightTime);
                    controller.Write(pin, PinValue.Low);
                    Thread.Sleep(lightTime);
                }
            }
            finally
            {
                controller.ClosePin(pin);
            }

        }      
    }
}