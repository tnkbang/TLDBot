using Discord.WebSocket;

namespace TLDBot.Structs
{
	public class TicTacToe
	{
		public static readonly int BOARD_SIZE	= 5;
		public static readonly int DEPTH		= 5;
		public static readonly char EMPTY		= '\0';
		public static readonly char PLAYER_X	= 'X';
		public static readonly char PLAYER_O	= 'O';

		public static readonly int[] ATK_POINT = { 0, 3, 24, 192, 1536, 12288 };
		public static readonly int[] DEF_POINT = { 0, 1, 9, 81, 729, 6534 };

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
		}

		public class Permission
		{
			public string NotAllow = string.Empty;
			public string NotTurn = string.Empty;
			public string NotMention = string.Empty;
			public string NotMentionBot = string.Empty;
		}

		public class Player
		{
			public ulong MessageId;
			public bool IsDuet = false;
			public bool IsFirstTime = true;
			public char SelectChar = EMPTY;
			public SocketUser? UserDuet;

			public int BoardSize = BOARD_SIZE;
			public char[,] Board = new char[BOARD_SIZE, BOARD_SIZE];

			public int WinPoint
			{
				get
				{
					switch (BoardSize)
					{
						case 3: return 3;
						case 5: return 4;
						default: return 3;
					}
				}
			}

			public bool IsWin { get { return CheckForWinner(Board, SelectChar); } }

			public bool IsLose
			{
				get
				{
					return CheckForWinner(Board, SelectChar.Equals(PLAYER_X) ? PLAYER_O : PLAYER_X);
				}
			}

			public bool IsFull { get { return IsBoardFull(); } }

			public bool IsOver { get { return IsWin || IsLose || IsFull; } }

			public Player(int board)
			{
				Board = new char[board, board];
			}

			public Player(SocketUser user, bool isDuet)
			{
				UserDuet = user;
				IsDuet = isDuet;
			}

			public void Clear()
			{
				MessageId = 0;
				IsDuet = false;
				IsFirstTime = true;
				SelectChar = EMPTY;
				UserDuet = null;
				BoardSize = BOARD_SIZE;
				Board = new char[BOARD_SIZE, BOARD_SIZE];
			}

			#region Check for winner
			private bool CheckBoardHorizontal(char[,] board, int row, int col, char player, int count)
			{
				for (int i = 0; i < count; i++)
				{
					if (col + i < BoardSize && board[row, col + i].Equals(player)) continue;
					return false;
				}
				return true;
			}

			private bool CheckBoardVertical(char[,] board, int row, int col, char player, int count)
			{
				for (int i = 0; i < count; i++)
				{
					if (row + i < BoardSize && board[row + i, col].Equals(player)) continue;
					return false;
				}
				return true;
			}

			private bool CheckBoardDiagonalX(char[,] board, int row, int col, char player, int count)
			{
				if ((row + count <= BoardSize && col + count <= BoardSize) is false) return false;

				for (int i = 0; i < count; i++)
				{
					if (board[row + i, col + i].Equals(player)) continue;
					return false;
				}
				return true;
			}

			private bool CheckBoardDiagonalY(char[,] board, int row, int col, char player, int count)
			{
				if ((row + 1 >= count && col <= BoardSize - count) is false) return false;

				for (int i = 0; i < count; i++)
				{
					if (board[row - i, col + i].Equals(player)) continue;
					return false;
				}
				return true;
			}

			/// <summary>
			/// Check user win
			/// </summary>
			private bool CheckForWinner(char[,] board, char player)
			{
				for (int row = 0; row < BoardSize; row++)
				{
					for (int col = 0; col < BoardSize; col++)
					{
						// Check horizontally
						if (CheckBoardHorizontal(board, row, col, player, WinPoint)) return true;

						// Check vertically
						if (CheckBoardVertical(board, row, col, player, WinPoint)) return true;

						// Check diagonal left to right
						if (CheckBoardDiagonalX(board, row, col, player, WinPoint)) return true;

						// Check diagonal right to left
						if (CheckBoardDiagonalY(board, row, col, player, WinPoint)) return true;
					}
				}
				return false;
			}
			#endregion

			private char[,] FillBoard(char[,] board)
			{
				for (int row = 0; row < BoardSize; row++)
				{
					for (int col = 0; col < BoardSize; col++)
					{
						if (board[row, col].Equals(EMPTY)) board[row, col] = SelectChar;
					}
				}
				return board;
			}

			/// <summary>
			/// If board has not active turn, move random row or col
			/// </summary>
			public void CheckTurns()
			{
				char[,] board = (char[,])Board.Clone();
				board = FillBoard(board);

				if(!CheckForWinner(board, SelectChar))
				{
					switch (new Random().Next(0, 1))
					{
						case 0: MoveRowLeft(); break;
						case 1: MoveColLeft(); break;
					}
				}
			}

			private void MoveRowLeft()
			{
				char[,] board = (char[,])Board.Clone();
				for (int row = 0; row < BoardSize - 1; row++)
				{
					for (int col = 0; col < BoardSize; col++)
					{
						Board[row, col] = board[row +1, col];
					}
				}

				//Last row empty
				for (int col = 0; col < BoardSize; col++)
				{
					Board[BoardSize - 1, col] = EMPTY;
				}
			}

			private void MoveColLeft()
			{
				char[,] board = (char[,])Board.Clone();
				for (int col = 0; col < BoardSize - 1; col++)
				{
					for (int row = 0; row < BoardSize; row++)
					{
						Board[row, col] = board[row, col + 1];
					}
				}

				//Last row empty
				for (int row = 0; row < BoardSize; row++)
				{
					Board[row, BoardSize - 1] = EMPTY;
				}
			}

			/// <summary>
			/// Check board is full (not cell empty)
			/// </summary>
			private bool IsBoardFull()
			{
				for (int i = 0; i < BoardSize; i++)
				{
					for (int j = 0; j < BoardSize; j++)
					{
						if (Board[i, j].Equals(EMPTY))
						{
							return false;
						}
					}
				}

				return true;
			}
		}
	}
}
