using Discord.WebSocket;
using TLDBot.Handlers;

namespace TLDBot.Structs
{
	public class TicTacToe
	{
		public Info Description = new Info();

		public TicTacToe() { }

		public class Info
		{
			public string Title = string.Empty;
			public string TitleField = string.Empty;
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
		}

		public class Permission
		{
			public string NotAllow = string.Empty;
			public string NotTurn = string.Empty;
		}

		public class Player
		{
			public ulong MessageId;
			public bool IsDuet = false;
			public char SelectChar = TicTacToeHandler.EMPTY;
			public SocketUser? UserDuet;

			//private int BoardSize = 3;
			public char[,] Board = new char[3, 3];

			public Player(int board)
			{
				Board = new char[board, board];
			}

			public Player(SocketUser user, bool isDuet)
			{
				UserDuet = user;
				IsDuet = isDuet;
			}
		}
	}
}
