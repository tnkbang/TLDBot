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
			public string NotMention = string.Empty;
			public string NotMentionBot = string.Empty;
		}

		public class Player
		{
			public ulong MessageId;
			public bool IsDuet = false;
			public bool IsFirstTime = true;
			public char SelectChar = TicTacToeHandler.EMPTY;
			public SocketUser? UserDuet;

			public int BoardSize = TicTacToeHandler.BOARD_SIZE;
			public char[,] Board = new char[TicTacToeHandler.BOARD_SIZE, TicTacToeHandler.BOARD_SIZE];

			public bool IsWin { get { return CheckForWinner(Board, SelectChar); } }

			public bool IsLose
			{
				get
				{
					return CheckForWinner(Board, SelectChar.Equals(TicTacToeHandler.PLAYER_X) ? TicTacToeHandler.PLAYER_O : TicTacToeHandler.PLAYER_X);
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

			#region Check for winner
			private bool CheckBoardHorizontal(char[,] board, int row, int col, char player, int count)
			{
				for (int i = 0; i < count; i++)
				{
					if (col + i < board.GetLength(1) && board[row, col + i].Equals(player)) continue;
					return false;
				}
				return true;
			}

			private bool CheckBoardVertical(char[,] board, int row, int col, char player, int count)
			{
				for (int i = 0; i < count; i++)
				{
					if (row + i < board.GetLength(0) && board[row + i, col].Equals(player)) continue;
					return false;
				}
				return true;
			}

			private bool CheckBoardDiagonalX(char[,] board, int row, int col, char player, int count)
			{
				if ((row + count <= board.GetLength(0) && col + count <= board.GetLength(1)) is false) return false;

				for (int i = 0; i < count; i++)
				{
					if (board[row + i, col + i].Equals(player)) continue;
					return false;
				}
				return true;
			}

			private bool CheckBoardDiagonalY(char[,] board, int row, int col, char player, int count)
			{
				if ((row + 1 >= count && col <= board.GetLength(1) - count) is false) return false;

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
			private bool CheckForWinner(char[,] board, char player, int count = 4)
			{
				for (int row = 0; row < board.GetLength(0); row++)
				{
					for (int col = 0; col < board.GetLength(1); col++)
					{
						// Check horizontally
						if (CheckBoardHorizontal(board, row, col, player, count)) return true;

						// Check vertically
						if (CheckBoardVertical(board, row, col, player, count)) return true;

						// Check diagonal left to right
						if (CheckBoardDiagonalX(board, row, col, player, count)) return true;

						// Check diagonal right to left
						if (CheckBoardDiagonalY(board, row, col, player, count)) return true;
					}
				}
				return false;
			}

			private char[,] FillBoard(char[,] board)
			{
				for (int row = 0; row < board.GetLength(0); row++)
				{
					for (int col = 0; col < board.GetLength(1); col++)
					{
						if (board[row, col].Equals(TicTacToeHandler.EMPTY)) board[row, col] = SelectChar;
					}
				}
				return board;
			}

			public void CheckTurns()
			{
				char[,] board = (char[,])Board.Clone();
				board = FillBoard(board);

				if(!CheckForWinner(board, SelectChar))
				{
					if (new Random().Next(1, 10) % 2 is 0) MoveRowLeft();
					else MoveColLeft();
				}
			}

			private void MoveRowLeft()
			{
				char[,] board = (char[,])Board.Clone();
				for (int row = 0; row < Board.GetLength(0) - 1; row++)
				{
					for (int col = 0; col < Board.GetLength(1); col++)
					{
						Board[row, col] = board[row +1, col];
					}
				}

				//Last row empty
				for (int col = 0; col < Board.GetLength(1); col++)
				{
					Board[Board.GetLength(0) - 1, col] = TicTacToeHandler.EMPTY;
				}
			}

			private void MoveColLeft()
			{
				char[,] board = (char[,])Board.Clone();
				for (int col = 0; col < Board.GetLength(1) - 1; col++)
				{
					for (int row = 0; row < Board.GetLength(0); row++)
					{
						Board[row, col] = board[row, col + 1];
					}
				}

				//Last row empty
				for (int row = 0; row < Board.GetLength(0); row++)
				{
					Board[row, Board.GetLength(1) - 1] = TicTacToeHandler.EMPTY;
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
						if (Board[i, j].Equals(TicTacToeHandler.EMPTY))
						{
							return false;
						}
					}
				}

				return true;
			}
			#endregion
		}
	}
}
