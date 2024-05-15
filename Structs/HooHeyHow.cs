namespace TLDBot.Structs
{
	public class HooHeyHow
	{
		public Dictionary<string, string> Vietnamese = new Dictionary<string, string>();

		public string StartTitle = string.Empty;
		public string StartIcon = string.Empty;

		public string Winner = string.Empty;
		public string Loser = string.Empty;
		public string Result = string.Empty;

		public Dictionary<string, string> ChoiceWin =  new Dictionary<string, string>();
		public string ChoiceLose = string.Empty;

		public Dictionary<string, string> JokeWin = new Dictionary<string, string>();
		public string JokeLose = string.Empty;

		public HooHeyHow() { }
	}
}
