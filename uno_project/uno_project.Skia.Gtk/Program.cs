using System;
using GLib;
using Uno.UI.Runtime.Skia;
using System.Device.Gpio;
using System.Threading;

namespace uno_project.Skia.Gtk
{
	class Program
	{
		static void Main(string[] args)
		{
			ExceptionManager.UnhandledException += delegate (UnhandledExceptionArgs expArgs)
			{
				Console.WriteLine("GLIB UNHANDLED EXCEPTION" + expArgs.ExceptionObject.ToString());
				expArgs.ExitApplication = true;
			};

			var host = new GtkHost(() => new App(), args);

			host.Run();

			GpioController controller = new GpioController(PinNumberingScheme.Board);

			var pin = 10;
			var lightTime = 300;

			controller.OpenPin(pin, PinMode.Output);

			try
			{
				while(true)
				{
					controller.Write(pin, PinValue.High);
					System.Threading.Thread.Sleep(lightTime);
					controller.Write(pin, PinValue.Low);
					System.Threading.Thread.Sleep(lightTime);
				}
			}
			finally
			{
				controller.ClosePin(pin);
			}

		}
	}
}
