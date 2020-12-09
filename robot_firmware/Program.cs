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
            var buttonPin = 26;

            controller.OpenPin(pin, PinMode.Output);
            controller.OpenPin(buttonPin, PinMode.InputPullUp);            

            try
            {
                while (true)
                {
                    if (controller.Read(buttonPin) == false)
                    {
                        controller.Write(pin, PinValue.High);
                    }
                    else
                    {
                        controller.Write(pin, PinValue.Low);
                    }
                }
            }
            finally
            {
                controller.ClosePin(pin);
            }

        }      
    }
}