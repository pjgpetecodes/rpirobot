using Tizen.Applications;
using Uno.UI.Runtime.Skia;

namespace uno_project.Skia.Tizen
{
	class Program
	{
		static void Main(string[] args)
		{
			var host = new TizenHost(() => new uno_project.App(), args);
			host.Run();
		}
	}
}
