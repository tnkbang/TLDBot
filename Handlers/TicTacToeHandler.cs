using Discord;
using Discord.WebSocket;
using TLDBot.Utility;

namespace TLDBot.Handlers
{
	public class TicTacToeHandler
	{
		private static readonly int BOARD_SIZE = 3;
		private static readonly char EMPTY  = ' ';
		private static readonly char PLAYER = 'X';
		private static readonly char COMPUTER = 'O';

		private char[,] board = new char[BOARD_SIZE, BOARD_SIZE];

		protected static Dictionary<ulong, char[,]> Player = new Dictionary<ulong, char[,]>();

		public MessageComponent Component
		{
			get
			{
				ComponentBuilder builder = new ComponentBuilder();
				for (int i = 0; i < BOARD_SIZE; i++)
				{
					for (int j = 0; j < BOARD_SIZE; j++)
					{
						Buttons btnC = new Buttons();
						builder.WithButton(btnC.GameCaro(i, j).WithDisabled(!board[i, j].Equals(EMPTY)), i);
					}
				}
				return builder.Build();
			}
		}

		public string Description
		{
			get
			{
				string str = "--------------------------" + Environment.NewLine;
				for (int i = 0; i < BOARD_SIZE; i++)
				{
					str += "|　";
					for (int j = 0; j < BOARD_SIZE; j++)
					{
						str += CharToEmoij(board[i, j]) + "　|　";
					}
					str += Environment.NewLine + "--------------------------" + Environment.NewLine;
				}
				return str;
			}
		}

		private readonly SocketUser User;

		public TicTacToeHandler(SocketUser user)
		{
			User = user;
			SetPlayer();
		}

		public void InitializeBoard(bool isOver = true)
		{
			if (isOver is false) return;

			for (int i = 0; i < BOARD_SIZE; i++)
			{
				for (int j = 0; j < BOARD_SIZE; j++)
				{
					board[i, j] = EMPTY;
				}
			}
		}

		private string CharToEmoij(char c)
		{
			return Emotes.GetByName("Caro" + (c.Equals(' ') ? "Blank" : c));
		}

		public bool IsGameOver()
		{
			// Check if there is a winner or if the board is full
			return CheckForWinner(PLAYER) || CheckForWinner(COMPUTER) || IsBoardFull();
		}

		private bool IsBoardFull()
		{
			for (int i = 0; i < BOARD_SIZE; i++)
			{
				for (int j = 0; j < BOARD_SIZE; j++)
				{
					if (board[i, j].Equals(EMPTY))
					{
						return false;
					}
				}
			}

			return true;
		}

		private bool CheckForWinner(char player)
		{
			// Check rows, columns, and diagonals for a winner
			for (int i = 0; i < BOARD_SIZE; i++)
			{
				if (board[i, 0].Equals(player) && board[i, 1].Equals(player) && board[i, 2].Equals(player))
				{
					return true;
				}
				if (board[0, i].Equals(player) && board[1, i].Equals(player) && board[2, i].Equals(player))
				{
					return true;
				}
			}
			if (board[0, 0].Equals(player) && board[1, 1].Equals(player) && board[2, 2].Equals(player))
			{
				return true;
			}
			if (board[0, 2].Equals(player) && board[1, 1].Equals(player) && board[2, 0].Equals(player))
			{
				return true;
			}
			return false;
		}

		private void MakeMove(int row, int col, char player)
		{
			board[row, col] = player;
		}

		private int Minimax(bool isMaximizing, int depth)
		{
			if (CheckForWinner(COMPUTER))
			{
				return 1;
			}
			if (CheckForWinner(PLAYER))
			{
				return -1;
			}
			if (IsBoardFull())
			{
				return 0;
			}
			if (isMaximizing)
			{
				int bestScore = int.MinValue;
				for (int i = 0; i < BOARD_SIZE; i++)
				{
					for (int j = 0; j < BOARD_SIZE; j++)
					{
						if (board[i, j].Equals(EMPTY))
						{
							board[i, j] = COMPUTER;
							int score = Minimax(false, depth + 1);
							board[i, j] = EMPTY;
							bestScore = Math.Max(bestScore, score);
						}
					}
				}
				return bestScore;
			}
			else
			{
				int bestScore = int.MaxValue;
				for (int i = 0; i < BOARD_SIZE; i++)
				{
					for (int j = 0; j < BOARD_SIZE; j++)
					{
						if (board[i, j].Equals(EMPTY))
						{
							board[i, j] = PLAYER;
							int score = Minimax(true, depth + 1);
							board[i, j] = EMPTY;
							bestScore = Math.Min(bestScore, score);
						}
					}
				}
				return bestScore;
			}
		}

		private void ComputerMove()
		{
			int bestScore = int.MinValue;
			int bestMoveRow = -1;
			int bestMoveCol = -1;
			for (int i = 0; i < BOARD_SIZE; i++)
			{
				for (int j = 0; j < BOARD_SIZE; j++)
				{
					if (board[i, j].Equals(EMPTY))
					{
						board[i, j] = COMPUTER;
						int score = Minimax(false, 0);
						board[i, j] = EMPTY;
						if (score > bestScore)
						{
							bestScore = score;
							bestMoveRow = i;
							bestMoveCol = j;
						}
					}
				}
			}
			MakeMove(bestMoveRow, bestMoveCol, COMPUTER);
		}

		private void SetPlayer()
		{
			char[,]? state;
			Player.TryGetValue(User.Id, out state);

			//User first using
			if (state is null)
			{
				InitializeBoard();
				Player[User.Id] = board;
			}
			else
			{
				board = state;
			}
		}

		private void PlayBoard(int row, int col)
		{
			if (!IsGameOver())
			{
				MakeMove(row, col, PLAYER);
				if (!IsGameOver()) ComputerMove();
			}
		}

		public string GetStatePlay(int row, int col)
		{
			PlayBoard(row, col);

			if (CheckForWinner(PLAYER)) return "Bạn đã thắng!";
			if (CheckForWinner(COMPUTER)) return "Bạn đã thua!";
			if (IsBoardFull()) return "Trận đấu hòa!";

			return "";
		}

		public virtual async Task RespondAsync()
		{
			await Task.CompletedTask;
		}
	}
}
