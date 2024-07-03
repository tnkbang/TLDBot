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
				if (Model is null) return "Model is null";
				GenerateContentResponse response = await Model.GenerateContent(prompt).ConfigureAwait(false);

				if (response.Text is null) return "Bad request";
				return response.Text;
			}
			catch { return "Your API key error!"; }
		}
	}
}
