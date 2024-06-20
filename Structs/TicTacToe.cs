namespace TLDBot.Structs
{
	public class TicTacToe
	{
		public static readonly int BOARD_SIZE	= 5;
		public static readonly int DEPTH		= 3;
		public static readonly char EMPTY		= '\0';
		public static readonly char PLAYER_X	= 'X';
		public static readonly char PLAYER_O	= 'O';

		public static readonly int[] ATK_POINT = { 0, 10, 100, 2000, 40000, 100000 };
		public static readonly int[] DEF_POINT = { 0, 10, 200, 4000, 80000, 100000 };

		public Info Description = new Info();

		public TicTacToe() { }

		public class Info
		{
			public string Title = string.Empty;
			public string TitleField = string.Empty;
			public string ThumbnailUrl = string.Empty;
			public Body Body = new Body();
			public State State = new State();
			public Permission Permission = new Permission();
		}

		public class Body
		{
			public string FistMove = string.Empty;
			public string SecondMoveDuet = string.Empty;
			public string SecondMoveBot = string.Empty;
		}

		public class State
		{
			public string NotSelect = string.Empty;
			public string Selected = string.Empty;
			public string Win = string.Empty;
			public string WinBot = string.Empty;
			public string Lose = string.Empty;
			public string Draws = string.Empty;
			public string Already = string.Empty;
		}

		public class Permission
		{
			public string NotAllow = string.Empty;
			public string NotTurn = string.Empty;
			public string NotMention = string.Empty;
			public string NotMentionBot = string.Empty;
		}
	}
}
