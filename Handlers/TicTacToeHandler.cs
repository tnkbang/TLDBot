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
		/// Check title embed
		/// </summary>
		public bool IsSetTitle = true;

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
				if (IsSetTitle) IsSetTitle = false; //Check title embed

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
				string str = Emotes.CaroX + Description.Body.FistMove + Environment.NewLine;
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
				string str = Emotes.CaroX + Description.Body.FistMove + Environment.NewLine;
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

		public virtual async Task RespondAsync()
		{
			await Task.CompletedTask;
		}

		#region Computer move handle
		/// <summary>
		/// The first move of bot
		/// </summary>
		private void ComputerFirst()
		{
			MakeMove(new Random().Next(0, _player.BoardSize), new Random().Next(0, _player.BoardSize), _rivalPlayer);
		}

		/// <summary>
		/// Set bot move char
		/// </summary>
		private void ComputerMove()
		{
			int[] rowCol = new ComputerHandler(_player, _rivalPlayer).GetMove();
			MakeMove(rowCol[0], rowCol[1], _rivalPlayer);
		}

		public class ComputerHandler
		{
			private char[,] _board;
			private Player _userPlayer;
			private char _userChar;
			private char _computerChar;
			private int _winPoint = 3;

			public ComputerHandler(Player player, char computer)
			{
				_userPlayer		= player;
				_board			= player.Board;
				_computerChar	= computer;
				_userChar		= _userPlayer.SelectChar;
				_winPoint		= _userPlayer.WinPoint;
			}

			/// <summary>
			/// Minimax algorithm and Alpha–beta pruning
			/// </summary>
			private int Minimax(bool isMaximizing, int depth, int alpha, int beta)
			{
				if (depth >= DEPTH || _userPlayer.IsOver) return Evaluate(_computerChar);

				if (isMaximizing) return MaxValue(depth, alpha, beta);
				return MinValue(depth, alpha, beta);
			}

			private int MaxValue(int depth, int alpha, int beta)
			{
				int bestScore = int.MinValue;
				for (int i = 0; i < BOARD_SIZE; i++)
				{
					for (int j = 0; j < BOARD_SIZE; j++)
					{
						if (_board[i, j].Equals(EMPTY))
						{
							_board[i, j] = _computerChar;
							int score = Minimax(false, depth + 1, alpha, beta);
							_board[i, j] = EMPTY;
							bestScore = Math.Max(bestScore, score);

							// Alpha–beta pruning
							alpha = Math.Max(alpha, bestScore);
							if (beta <= alpha) break;
						}
					}
					if (beta <= alpha) break;
				}
				return bestScore;
			}

			private int MinValue(int depth, int alpha, int beta)
			{
				int bestScore = int.MaxValue;
				for (int i = 0; i < BOARD_SIZE; i++)
				{
					for (int j = 0; j < BOARD_SIZE; j++)
					{
						if (_board[i, j].Equals(EMPTY))
						{
							_board[i, j] = _userChar;
							int score = Minimax(true, depth + 1, alpha, beta);
							_board[i, j] = EMPTY;
							bestScore = Math.Min(bestScore, score);

							// Alpha–beta pruning
							beta = Math.Min(beta, bestScore);
							if (beta <= alpha) break;
						}
					}
					if (beta <= alpha) break;
				}
				return bestScore;
			}

			/// <summary>
			/// Evaluate score the main board
			/// </summary>
			private int Evaluate(char player)
			{
				int score = 0;

				// Check horizontally
				score += EvaluateHorizontal(player);

				// Check vertically
				score += EvaluateVertical(player);

				// Check the main diagonally
				score += EvaluateMainDiagonal(player);

				// Check the extra diagonally
				score += EvaluateExtraDiagonal(player);

				return score;
			}

			/// <summary>
			/// Check horizontally
			/// </summary>
			private int EvaluateHorizontal(char player)
			{
				int score = 0;
				for (int i = 0; i < BOARD_SIZE; i++)
				{
					for (int j = 0; j <= BOARD_SIZE - _winPoint; j++)
					{
						score += EvaluateLine(player, i, j, 0, 1);
					}
				}
				return score;
			}

			/// <summary>
			/// Check vertically
			/// </summary>
			private int EvaluateVertical(char player)
			{
				int score = 0;
				for (int i = 0; i <= BOARD_SIZE - _winPoint; i++)
				{
					for (int j = 0; j < BOARD_SIZE; j++)
					{
						score += EvaluateLine(player, i, j, 1, 0);
					}
				}
				return score;
			}

			/// <summary>
			/// Check the main diagonally
			/// </summary>
			private int EvaluateMainDiagonal(char player)
			{
				int score = 0;
				for (int i = 0; i <= BOARD_SIZE - _winPoint; i++)
				{
					for (int j = 0; j <= BOARD_SIZE - _winPoint; j++)
					{
						score += EvaluateLine(player, i, j, 1, 1);
					}
				}
				return score;
			}

			/// <summary>
			/// Check the extra diagonally
			/// </summary>
			private int EvaluateExtraDiagonal(char player)
			{
				int score = 0;
				for (int i = _winPoint - 1; i < BOARD_SIZE; i++)
				{
					for (int j = 0; j <= BOARD_SIZE - _winPoint; j++)
					{
						score += EvaluateLine(player, i, j, -1, 1);
					}
				}
				return score;
			}

			/// <summary>
			/// Sum score for line
			/// </summary>
			private int EvaluateLine(char player, int row, int col, int dRow, int dCol)
			{
				int score = 0;
				int playerCount = 0; // Player choice count
				int opponentCount = 0; // Rival player choice count

				for (int i = 0; i < _winPoint; i++)
				{
					int r = row + i * dRow;
					int c = col + i * dCol;
					if (_board[r, c] == player)
						playerCount++;
					else if (_board[r, c] == _userChar)
						opponentCount++;
				}

				// Calculate points when attacking
				if (opponentCount is 0) score += ATK_POINT[playerCount];

				// Calculate points when defending
				if (playerCount is 0) score -= DEF_POINT[opponentCount];

				return score;
			}

			/// <summary>
			/// Set bot move char
			/// </summary>
			public int[] GetMove()
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
							_board[i, j] = _computerChar;
							int score = Minimax(false, 0, int.MinValue, int.MaxValue);
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
				return [bestMoveRow, bestMoveCol];
			}
		}
		#endregion

		#region Player handle
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

				if (!CheckForWinner(board, SelectChar))
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
						Board[row, col] = board[row + 1, col];
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
		#endregion
	}
}
