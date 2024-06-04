using Discord;
using Discord.WebSocket;
using TLDBot.Utility;
using static TLDBot.Structs.TicTacToe;

namespace TLDBot.Handlers
{
	public class TicTacToeHandler
	{
		private static readonly int BOARD_SIZE = 3;
		public static readonly char EMPTY  = '\0';
		public static readonly char PLAYER_X = 'X';
		public static readonly char PLAYER_O = 'O';

		protected Player _player = new Player(BOARD_SIZE);
		protected static Dictionary<ulong, Player> T3Player = new Dictionary<ulong, Player>();
		protected static Info Description = Helper.TicTacToe.Description;

		/// <summary>
		/// Board game in process (read from T3Player)
		/// </summary>
		protected char[,] _board
		{
			get
			{
				if (_player.IsDuet)
				{
					Player? player;
					T3Player.TryGetValue(_player.UserDuet!.Id, out player);

					if(player is not null) player.Board = _player.Board;
				}
				return _player.Board;
			}
		}

		/// <summary>
		/// Player is bot if sigle or rival if duet
		/// </summary>
		private char _rivalPlayer
		{
			get { return _player.SelectChar.Equals(PLAYER_X) ? PLAYER_O : PLAYER_X; }
		}

		/// <summary>
		/// Char in process component
		/// </summary>
		private char ComponentChar
		{
			get
			{
				if (!_player.IsDuet) return _player.SelectChar;
				return _player.SelectChar.Equals(PLAYER_X) ? PLAYER_O : PLAYER_X;
			}
		}

		/// <summary>
		/// Process component
		/// </summary>
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
						builder.WithButton(btnC.GameCaro(i, j, CharToEmoij(ComponentChar)).WithDisabled(!_board[i, j].Equals(EMPTY)), i);
					}
				}
				return builder.Build();
			}
		}

		/// <summary>
		/// Component when user choose char X or O
		/// </summary>
		public MessageComponent ComponentChooseXO
		{
			get
			{
				ComponentBuilder builder = new ComponentBuilder();
				Buttons btnC = new Buttons();
				builder.WithButton(btnC.GameCaroX().WithDisabled(_player.SelectChar.Equals(PLAYER_X)));
				builder.WithButton(btnC.GameCaroO().WithDisabled(_player.SelectChar.Equals(PLAYER_O)));
				return builder.Build();
			}
		}

		/// <summary>
		/// Description in process game
		/// </summary>
		public string BodyBoardPorcess
		{
			get
			{
				string str = "--------------------------" + Environment.NewLine;
				for (int i = 0; i < BOARD_SIZE; i++)
				{
					str += "|　";
					for (int j = 0; j < BOARD_SIZE; j++)
					{
						str += CharToEmoij(_board[i, j]) + "　|　";
					}
					str += Environment.NewLine + "--------------------------" + Environment.NewLine;
				}
				return str;
			}
		}

		/// <summary>
		/// Description first call game with machine
		/// </summary>
		public static string DescriptionWithBot
		{
			get
			{
				string str = Description.Title + Environment.NewLine;
				str += Emotes.CaroX + Description.Body.FistMove + Environment.NewLine;
				str += Emotes.CaroO + Description.Body.SecondMoveBot;

				return str;
			}
		}

		/// <summary>
		/// Description first call game duet
		/// </summary>
		public static string DescriptionDuet
		{
			get
			{
				string str = Description.Title + Environment.NewLine;
				str += Emotes.CaroX + Description.Body.FistMove + Environment.NewLine;
				str += Emotes.CaroO + Description.Body.SecondMoveDuet;

				return str;
			}
		}

		private readonly SocketUser User;

		public TicTacToeHandler(SocketUser user)
		{
			User = user;
			SetPlayer();
		}

		/// <summary>
		/// Init board game
		/// </summary>
		public void InitializeBoard()
		{
			for (int i = 0; i < BOARD_SIZE; i++)
			{
				for (int j = 0; j < BOARD_SIZE; j++)
				{
					_player.Board[i, j] = EMPTY;
				}
			}
			_player.SelectChar = EMPTY;
		}

		/// <summary>
		/// Reset game (board, player,....)
		/// </summary>
		public void ResetBase()
		{
			InitializeBoard();
			_player.MessageId = 0;

			if(_player.IsDuet && _player.UserDuet is not null)
			{
				Player? player;
				T3Player.TryGetValue(_player.UserDuet.Id, out player);
				if (player is not null)
				{
					player.MessageId = 0;
					player.SelectChar = EMPTY;
					player.Board = new char[BOARD_SIZE, BOARD_SIZE];
					player.UserDuet = null;
				}
			}

			_player.UserDuet = null;
			_player.IsDuet = false;
		}

		/// <summary>
		/// Convert char to emoij
		/// </summary>
		protected string CharToEmoij(char c)
		{
			return Emotes.GetByName("Caro" + (c.Equals(EMPTY) ? "Blank" : c));
		}

		/// <summary>
		/// Check game over
		/// </summary>
		public bool IsGameOver()
		{
			// Check if there is a winner or if the board is full
			return CheckForWinner(PLAYER_X) || CheckForWinner(PLAYER_O) || IsBoardFull();
		}

		/// <summary>
		/// Check board is full (not cell empty)
		/// </summary>
		private bool IsBoardFull()
		{
			for (int i = 0; i < BOARD_SIZE; i++)
			{
				for (int j = 0; j < BOARD_SIZE; j++)
				{
					if (_board[i, j].Equals(EMPTY))
					{
						return false;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Check user win
		/// </summary>
		private bool CheckForWinner(char player)
		{
			// Check rows, columns, and diagonals for a winner
			for (int i = 0; i < BOARD_SIZE; i++)
			{
				if (_board[i, 0].Equals(player) && _board[i, 1].Equals(player) && _board[i, 2].Equals(player))
				{
					return true;
				}
				if (_board[0, i].Equals(player) && _board[1, i].Equals(player) && _board[2, i].Equals(player))
				{
					return true;
				}
			}
			if (_board[0, 0].Equals(player) && _board[1, 1].Equals(player) && _board[2, 2].Equals(player))
			{
				return true;
			}
			if (_board[0, 2].Equals(player) && _board[1, 1].Equals(player) && _board[2, 0].Equals(player))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Make user move char in board
		/// </summary>
		private void MakeMove(int row, int col, char player)
		{
			_board[row, col] = player;
		}

		/// <summary>
		/// Set player game
		/// </summary>
		private void SetPlayer()
		{
			Player? player;
			T3Player.TryGetValue(User.Id, out player);

			//User first using
			if (player is null)
			{
				InitializeBoard();
				T3Player[User.Id] = _player;
			}
			else
			{
				_player = player;
			}
		}

		/// <summary>
		/// Set char of user choice
		/// </summary>
		protected void SetChooseXO(char name)
		{
			_player.SelectChar = name;
			if (_player.SelectChar.Equals(PLAYER_O) && !_player.IsDuet) ComputerFirst();
		}

		/// <summary>
		/// Set mode duet or single
		/// </summary>
		protected void SetMode(SocketUser? user)
		{
			if(user is null)
			{
				_player.IsDuet = false;
				return;
			}

			_player.IsDuet = true;
			_player.UserDuet = user;
			SetModeDuet(user.Id);
		}

		/// <summary>
		/// Set mode of user duet
		/// </summary>
		private void SetModeDuet(ulong uid)
		{
			Player? player;
			T3Player.TryGetValue(uid, out player);

			if(player is null)
			{
				T3Player[uid] = new Player(User, true);
			}
			else
			{
				player.IsDuet = true;
				player.UserDuet = User;
			}
		}

		/// <summary>
		/// Set message base board (using for check permission)
		/// </summary>
		protected void SetMessageBoard(ulong mid)
		{
			_player.MessageId = mid;
			if (!_player.IsDuet && _player.UserDuet is null) return;

			Player? player;
			T3Player.TryGetValue(_player.UserDuet!.Id, out player);
			if (player is not null)
			{
				player.MessageId = mid;
			}
		}

		/// <summary>
		/// Get string for duet choice
		/// </summary>
		protected string GetStringDuetChoice(ulong uid)
		{
			Player? player;
			T3Player.TryGetValue(uid, out player);

			if (player is null || player.SelectChar.Equals(EMPTY)) return Description.State.NotSelect;
			return Description.State.Selected + CharToEmoij(player.SelectChar);
		}

		/// <summary>
		/// Run board of user
		/// </summary>
		private void PlayBoard(int row, int col)
		{
			if (!IsGameOver())
			{
				MakeMove(row, col, _player.SelectChar);
				if (!IsGameOver() && !_player.IsDuet) ComputerMove();
			}
		}

		/// <summary>
		/// Get state play game
		/// </summary>
		public string GetStatePlay(int row, int col)
		{
			PlayBoard(row, col);

			if (_player.IsDuet)
			{
				if (CheckForWinner(_player.SelectChar)) return User.Mention + Description.State.Win;
				if (CheckForWinner(_rivalPlayer)) return _player.UserDuet!.Mention + Description.State.Win;
				if (IsBoardFull()) return Description.State.Draws;

				return "";
			}
			if (CheckForWinner(_player.SelectChar)) return Description.State.WinBot;
			if (CheckForWinner(_rivalPlayer)) return Description.State.Lose;
			if (IsBoardFull()) return Description.State.Draws;

			return "";
		}

		#region Machine move handle
		/// <summary>
		/// The first move of bot
		/// </summary>
		private void ComputerFirst()
		{
			MakeMove(new Random().Next(0, 2), new Random().Next(0, 2), _rivalPlayer);
		}

		/// <summary>
		/// Minimax algorithm
		/// </summary>
		private int Minimax(bool isMaximizing, int depth)
		{
			if (CheckForWinner(_rivalPlayer))
			{
				return 1;
			}
			if (CheckForWinner(_player.SelectChar))
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
						if (_board[i, j].Equals(EMPTY))
						{
							_board[i, j] = _rivalPlayer;
							int score = Minimax(false, depth + 1);
							_board[i, j] = EMPTY;
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
						if (_board[i, j].Equals(EMPTY))
						{
							_board[i, j] = _player.SelectChar;
							int score = Minimax(true, depth + 1);
							_board[i, j] = EMPTY;
							bestScore = Math.Min(bestScore, score);
						}
					}
				}
				return bestScore;
			}
		}

		/// <summary>
		/// Set bot move char
		/// </summary>
		private void ComputerMove()
		{
			int bestScore = int.MinValue;
			int bestMoveRow = -1;
			int bestMoveCol = -1;
			for (int i = 0; i < BOARD_SIZE; i++)
			{
				for (int j = 0; j < BOARD_SIZE; j++)
				{
					if (_board[i, j].Equals(EMPTY))
					{
						_board[i, j] = _rivalPlayer;
						int score = Minimax(false, 0);
						_board[i, j] = EMPTY;
						if (score > bestScore)
						{
							bestScore = score;
							bestMoveRow = i;
							bestMoveCol = j;
						}
					}
				}
			}
			MakeMove(bestMoveRow, bestMoveCol, _rivalPlayer);
		}
		#endregion

		public virtual async Task RespondAsync()
		{
			await Task.CompletedTask;
		}
	}
}
