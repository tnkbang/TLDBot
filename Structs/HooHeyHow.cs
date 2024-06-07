namespace TLDBot.Structs
{
	public class HooHeyHow
	{
		public Info Description = new Info();

		public HooHeyHow() { }

		public class Info
		{
			public string StartTitle = string.Empty;
			public string StartIcon = string.Empty;
			public string Winner = string.Empty;
			public string Loser = string.Empty;
			public string Result = string.Empty;
			public StateWin ChoiceWin = new StateWin();
			public string ChoiceLose = string.Empty;
			public StateWin JokeWin = new StateWin();
			public string JokeLose = string.Empty;
		}

		public class StateWin
		{
			public string One = string.Empty;
			public string Two = string.Empty;
			public string Three = string.Empty;

			public string GetValue(int count)
			{
				switch (count)
				{
					case 1: return One;
					case 2: return Two;
					case 3: return Three;
					default: return string.Empty;
				}
			}
		}
	}
}
