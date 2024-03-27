using System.Diagnostics;

namespace TLDBot.Structs
{
	public class Lavalink
	{
		private static Process _process = new Process();
		public static void Start()
		{
			ProcessStartInfo psi = new ProcessStartInfo("java.exe", " -jar " + Environment.CurrentDirectory + "\\Lavalink.jar");
			psi.CreateNoWindow = true;
			psi.UseShellExecute = true;
			_process = new Process();
			_process.StartInfo = psi;
			_process.Start();
		}

		public static void Stop()
		{
			_process.Kill();
			_process.Dispose();
		}
	}
}
