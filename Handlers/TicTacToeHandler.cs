using Discord;
using Discord.WebSocket;
using TLDBot.Utility;
using static TLDBot.Structs.TicTacToe;

namespace TLDBot.Handlers
{
	public class TicTacToeHandler
	{
		protected Player _player = new Player(BOARD_SIZE);
		protected static Dictionary<ulong, Player> T3Player = new Dictionary<ulong, Player>();
		protected static Info Description = Helper.TicTacToe.Description;

		/// <summary>
		/// Line horizontally board description
		/// </summary>
		private string Line
		{
			get
			{
				switch (BOARD_SIZE)
				{
					case 3: return "--------------------------";
					case 4: return "-----------------------------------";
					case 5: return "--------------------------------------------";
					default: return "---------";
				}
			}
		}

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

		private bool _isFirstTime
		{
			get
			{
				if (_player.IsDuet)
				{
					Player? player;
					T3Player.TryGetValue(_player.UserDuet!.Id, out player);

					if (player is not null) player.IsFirstTime = _player.IsFirstTime;
				}
				return _player.IsFirstTime;
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
				if (_isFirstTime) return PLAYER_X;

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
						if (_board[i, j].Equals(EMPTY))
						{
							builder.WithButton(btnC.GameCaro(i, j, CharToEmoij(ComponentChar)).WithDisabled(false), i);
						}
						else
						{
							builder.WithButton(btnC.GameCaro(i, j, CharToEmoij(_board[i, j])).WithDisabled(true), i);
						}

						//builder.WithButton(btnC.GameCaro(i, j, CharToEmoij(ComponentChar)).WithDisabled(!_board[i, j].Equals(EMPTY)), i);
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
				string str = Line + Environment.NewLine;
				for (int i = 0; i < BOARD_SIZE; i++)
				{
					str += "|　";
					for (int j = 0; j < BOARD_SIZE; j++)
					{
						str += CharToEmoij(_board[i, j]) + "　|　";
					}
					str += Environment.NewLine + Line + Environment.NewLine;
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
		/// Reset game (board, player,....)
		/// </summary>
		public void ResetBase()
		{
			if(_player.IsDuet && _player.UserDuet is not null)
			{
				Player? player;
				T3Player.TryGetValue(_player.UserDuet.Id, out player);
				if (player is not null)
				{
					player.Clear();
				}
			}

			_player.Clear();
		}

		/// <summary>
		/// Convert char to emoij
		/// </summary>
		protected string CharToEmoij(char c)
		{
			return Emotes.GetByName("Caro" + (c.Equals(EMPTY) ? "Blank" : c));
		}

		/// <summary>
		/// Make user move char in board
		/// </summary>
		private void MakeMove(int row, int col, char player)
		{
			if (_isFirstTime) _player.IsFirstTime = false;
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
			if (player is null) T3Player[User.Id] = _player;
			else _player = player;
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
			if (!_player.IsOver)
			{
				MakeMove(row, col, _player.SelectChar);
				if (!_player.IsOver && !_player.IsDuet) ComputerMove();
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
				if (_player.IsWin) return User.Mention + Description.State.Win;
				if (_player.IsLose) return _player.UserDuet!.Mention + Description.State.Win;
				if (_player.IsFull) return Description.State.Draws;

				_player.CheckTurns();
				return string.Empty;
			}
			if (_player.IsWin) return Description.State.WinBot;
			if (_player.IsLose) return Description.State.Lose;
			if (_player.IsFull) return Description.State.Draws;

			_player.CheckTurns();
			return string.Empty;
		}

		#region Machine move handle
		/// <summary>
		/// The first move of bot
		/// </summary>
		private void ComputerFirst()
		{
			MakeMove(new Random().Next(0, _player.BoardSize), new Random().Next(0, _player.BoardSize), _rivalPlayer);
		}

		/// <summary>
		/// Minimax algorithm
		/// </summary>
		private int Minimax(bool isMaximizing, int depth)
		{
			if (_player.IsLose) return 1;
			if (_player.IsWin) return -1;
			if (_player.IsFull) return 0;

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
