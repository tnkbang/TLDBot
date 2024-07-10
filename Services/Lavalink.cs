using System.Diagnostics;

namespace TLDBot.Services
{
	public class Lavalink
	{
		private static Process _process = new Process();
		public static void Start()
		{
			ProcessStartInfo psi = new ProcessStartInfo
			{
				FileName = "java.exe",
				Arguments = "-jar " + Environment.CurrentDirectory + "\\Lavalink\\Lavalink.jar",
				WorkingDirectory = Environment.CurrentDirectory + "\\Lavalink",
				CreateNoWindow = true,
				UseShellExecute = true
			};
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
