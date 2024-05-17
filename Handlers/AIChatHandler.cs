using Mscc.GenerativeAI;

namespace TLDBot.Handlers
{
	public class AIChatHandler
	{
		private static GoogleAI? GoogleAI;
		private static GenerativeModel? Model;

		public AIChatHandler() { }

		public static void GenerateGoogleAI(string key)
		{
			GoogleAI = new GoogleAI(key);
			Model = GoogleAI.GenerativeModel();
		}

		public async Task<string> GenerateContent(string prompt)
		{
			try
			{
				GenerateContentResponse response = await Model!.GenerateContent(prompt).ConfigureAwait(false);
				return response.Text!.ToString() ?? "Bad request";
			}
			catch { return "Your API key error!"; }
		}
	}
}
