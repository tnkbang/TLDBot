using Discord.WebSocket;

namespace TLDBot.Structs
{
	public class TicTacToe
	{
		public static readonly int BOARD_SIZE	= 3;
		public static readonly char EMPTY		= '\0';
		public static readonly char PLAYER_X	= 'X';
		public static readonly char PLAYER_O	= 'O';

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
			#endregion
		}

		public class Cell
		{
			private int X;
			private int Y;
			private char State;

			public Cell()
			{
				X = -1;
				Y = -1;
				State = EMPTY;
			}

			public Cell(int x, int y)
			{
				X = x;
				Y = y;
				State = EMPTY;
			}

			public Cell(int x, int y, char state) : this(x, y)
			{
				State = state;
			}

			public void SetLocation(int x, int y)
			{
				X = x;
				Y = y;
			}

			public int GetX()
			{
				return X;
			}

			public int GetY()
			{
				return Y;
			}

			public char GetState()
			{
				return State;
			}

			public void SetState(char state)
			{
				State = state;
			}

			public bool EnableClick()
			{
				if (State.Equals(EMPTY)) return true;
				return false;
			}
		}

		public class EvalCell
		{
			private Cell Cell;
			private int Value;

			public EvalCell()
			{
				Cell = new Cell();
				Value = 0;
			}

			public EvalCell(Cell cell, int value)
			{
				Cell = cell;
				Value = value;
			}

			public EvalCell(int x, int y, int value)
			{
				Cell = new Cell(x, y);
				Value = value;
			}

			public Cell GetCell()
			{
				return Cell;
			}

			public int getX()
			{
				return Cell.GetX();
			}

			public int getY()
			{
				return Cell.GetY();
			}

			public int getValue()
			{
				return Value;
			}
		}

		public class GameBoard
		{
			private char[,] Cell;
			private List<Cell> MoveSteps;
			private int Steps;

			public GameBoard()
			{
				Steps = 0;
				MoveSteps = new List<Cell>();
				Cell = new char[BOARD_SIZE, BOARD_SIZE];
				/*for (int i = 0; i < BOARD_SIZE; i++)
				{
					for (int j = 0; j < BOARD_SIZE; j++) Cell[i, j] = 0;
				}*/
			}

			public GameBoard(char[,] cell)
			{
				Steps = 0;
				MoveSteps = new List<Cell>();
				Cell = new char[BOARD_SIZE, BOARD_SIZE];
				for (int i = 0; i < BOARD_SIZE; i++)
				{
					for (int j = 0; j < BOARD_SIZE; j++)
					{
						Cell[i, j] = cell[i, j];
						if (cell[i, j] != 0)
						{
							MoveSteps.Add(new Cell(i, j, cell[i, j]));
							Steps++;
						}
					}
				}

			}

			public void Update(int x, int y, char player)
			{
				Cell[x, y] = player;
				MoveSteps.Add(new Cell(x, y, player));
				Steps++;
			}

			public char[,] GetState()
			{
				return Cell;
			}

			public void SetState(char[,] cell)
			{
				for (int i = 0; i < BOARD_SIZE; i++)
				{
					for (int j = 0; j < BOARD_SIZE; j++) Cell[i, j] = cell[i, j];
				}
			}

			public bool CheckWinner(char player)
			{
				int[] lineX = { 1, 1, 0, 1 };
				int[] lineY = { 0, 1, 1, -1 };
				for (int x = 0; x < BOARD_SIZE; x++)
				{
					for (int y = 0; y < BOARD_SIZE; y++)
					{
						if (Cell[x, y].Equals(player) is false) continue;

						for (int i = 0; i < 4; i++)
						{
							if (CheckWinner(lineX, lineY, x, y, player)) return true;
						}
					}
				}
				return false;
			}

			public bool CheckWinner(int[] lineX, int[] lineY, int x, int y, char player)
			{
				for (int i = 0; i < 4; i++)
				{
					int count = 1;
					for (int j = 1; j <= 4; j++)
					{
						int vtx = x + lineX[i] * j;
						int vty = y + lineY[i] * j;

						if (vtx < 0 || vty < 0 || vtx >= BOARD_SIZE || vty >= BOARD_SIZE) break;
						if (Cell[vtx, vty] == player) count++;
						else break;
					}

					if (count == 5) return true;
				}

				return false;
			}

			public bool EnableClick(int x, int y)
			{
				if (x >= 0 && x < BOARD_SIZE && y >= 0 && y < BOARD_SIZE)
					if (Cell[x, y] == 0) return true;
				return false;
			}

			public bool IsOver()
			{
				if (Steps == BOARD_SIZE * BOARD_SIZE) return true;
				else return false;
			}
		}

		public class Heuristic
		{
			private int[,] EvalState;

			public Heuristic()
			{
				EvalState = new int[BOARD_SIZE, BOARD_SIZE];
			}

			private int[] point = {
						4, 4, 4,
						8, 8, 8,
						8, 8, 8, 8, 8, 8,
						8,
						500, 500, 500, 500, 500, 500, 500,
						1000, 1000, 1000, 1000, 1000, 1000,
						100000
			};

			private string[] caseUser = {
						"11001", "10101", "10011",
						"00110", "01010", "01100",
						"11100", "11010", "10110", "01101", "01011", "00111",
						"01110",
						"011100", "011010", "010110", "001110", "1010101", "1011001", "1001101",
    					"01111","10111", "11011", "11101", "11101",  "11110",
						"11111"
			};

			private string[] caseAI = {
						"22002", "20202", "20022",
						"00220", "02020", "02200",
						"22200", "22020", "20220", "02202", "02022", "00222",
						"02220",
						"022200", "022020", "020220", "002220", "2020202", "2022002", "2002202",
						"02222", "20222", "22022", "22202", "22202",  "22220",
						"22222"
			};

			int[] DefenseScore = { 0, 1, 9, 81, 729, 6534 };
			int[] AttackScore = { 0, 3, 24, 192, 1536, 12288 };

			public int EvaluateState(GameBoard state)
			{
				string rem = ";";
				char[,] cell = state.GetState();
				for (int i = 0; i < BOARD_SIZE; i++)
				{
					for (int j = 0; j < BOARD_SIZE; j++)
					{
						rem += cell[i, j];
					}
					rem += ";";
					for (int j = 0; j < BOARD_SIZE; j++)
					{
						rem += cell[j, i];
					}
					rem += ";";
				}

				for (int i = 0; i < BOARD_SIZE - 4; i++)
				{
					for (int j = 0; j < BOARD_SIZE - i; j++)
					{
						rem += cell[j, i + j];
					}
					rem += ";";
				}

				for (int i = BOARD_SIZE - 5; i > 0; i--)
				{
					for (int j = 0; j < BOARD_SIZE - i; j++)
					{
						rem += cell[i + j, j];
					}
					rem += ";";
				}

				for (int i = 4; i < BOARD_SIZE; i++)
				{
					for (int j = 0; j <= i; j++)
					{
						rem += cell[i - j, j];
					}
					rem += ";";
				}

				for (int i = BOARD_SIZE - 5; i > 0; i--)
				{
					for (int j = BOARD_SIZE - 1; j >= i; j--)
					{
						rem += cell[j, i + BOARD_SIZE - j - 1];
					}
					rem += ";\n";
				}

				string find1, find2;
				int diem = 0;

				for (int i = 0; i < caseUser.Length; i++)
				{
					find1 = caseAI[i];
					find2 = caseUser[i];
					diem += point[i] * Count(rem, find1);
					diem -= point[i] * Count(rem, find2);
				}
				return diem;
			}

			public int Count(string text, string find)
			{
				return 0;
			}

			void ResetValue()
			{
				for (int i = 0; i < BOARD_SIZE; i++)
				{
					for (int j = 0; j < BOARD_SIZE; j++)
					{
						EvalState[i, j] = EMPTY;
					}
				}
			}

			public void EvaluateEachCell(GameBoard state, char player)
			{
				ResetValue();
				int x, y, i, countAI, countUser;
				char[,] cell = state.GetState();

				for (x = 0; x < BOARD_SIZE; x++)
				{
					for (y = 0; y < BOARD_SIZE - 4; y++)
					{
						countAI = 0; countUser = 0;
						for (i = 0; i < 5; i++)
						{
							if (cell[x, y + i] == PLAYER_O) countAI++;
							else if (cell[x, y + i] == PLAYER_X) countUser++;
						}

						if (countAI * countUser == 0 && countAI != countUser)
						{
							for (i = 0; i < 5; i++)
							{
								if (cell[x, y + i] == 0)
								{
									if (countAI == 0)
									{
										if (player == PLAYER_O)
										{
											EvalState[x, y + i] += DefenseScore[countUser];
										}
										else EvalState[x, y + i] += AttackScore[countUser];
									}
									else if (countUser == 0)
									{
										if (player == PLAYER_X)
										{
											EvalState[x, y + i] += DefenseScore[countAI];
										}
										else EvalState[x, y + i] += AttackScore[countAI];
									}
									if (countAI == 4 || countUser == 4)
									{
										EvalState[x, y + i] *= 2;
									}
								}
							}
						}
					}
				}

				for (x = 0; x < BOARD_SIZE - 4; x++)
				{
					for (y = 0; y < BOARD_SIZE; y++)
					{
						countAI = 0; countUser = 0;
						for (i = 0; i < 5; i++)
						{
							if (cell[x + i, y] == PLAYER_O) countAI++;
							else if (cell[x + i, y] == PLAYER_X) countUser++;
						}
						if (countAI * countUser == 0 && countAI != countUser)
						{
							for (i = 0; i < 5; i++)
							{
								if (cell[x + i, y] == 0)
								{
									if (countAI == 0)
									{
										if (player == PLAYER_O)
										{
											EvalState[x + i, y] += DefenseScore[countUser];
										}
										else EvalState[x + i, y] += AttackScore[countUser];
									}
									else if (countUser == 0)
									{
										if (player == PLAYER_X)
										{
											EvalState[x + i, y] += DefenseScore[countAI];
										}
										else EvalState[x + i, y] += AttackScore[countAI];
									}
									if (countAI == 4 || countUser == 4)
									{
										EvalState[x + i, y] *= 2;
									}
								}
							}
						}
					}
				}

				for (x = 0; x < BOARD_SIZE - 4; x++)
				{
					for (y = 0; y < BOARD_SIZE - 4; y++)
					{
						countAI = 0; countUser = 0;
						for (i = 0; i < 5; i++)
						{
							if (cell[x + i, y + i] == PLAYER_O) countAI++;
							else if (cell[x + i, y + i] == PLAYER_X) countUser++;
						}
						if (countAI * countUser == 0 && countAI != countUser)
						{
							for (i = 0; i < 5; i++)
							{
								if (cell[x + i, y + i] == 0)
								{
									if (countAI == 0)
									{
										if (player == PLAYER_O)
										{
											EvalState[x + i, y + i] += DefenseScore[countUser];
										}
										else EvalState[x + i, y + i] += AttackScore[countUser];
									}
									else if (countUser == 0)
									{
										if (player == PLAYER_X)
										{
											EvalState[x + i, y + i] += DefenseScore[countAI];
										}
										else EvalState[x + i, y + i] += AttackScore[countAI];
									}
									if (countAI == 4 || countUser == 4)
									{
										EvalState[x + i, y + i] *= 2;
									}
								}
							}
						}
					}
				}

				for (x = 4; x < BOARD_SIZE; x++)
				{
					for (y = 0; y < BOARD_SIZE - 4; y++)
					{
						countAI = 0; countUser = 0;
						for (i = 0; i < 5; i++)
						{
							if (cell[x - i, y + i] == PLAYER_O) countAI++;
							else if (cell[x - i, y + i] == PLAYER_X) countUser++;
						}
						if (countAI * countUser == 0 && countAI != countUser)
						{
							for (i = 0; i < 5; i++)
							{
								if (cell[x - i, y + i] == 0)
								{
									if (countAI == 0)
									{
										if (player == PLAYER_O)
										{
											EvalState[x - i, y + i] += DefenseScore[countUser];
										}
										else EvalState[x - i, y + i] += AttackScore[countUser];
									}
									else if (countUser == 0)
									{
										if (player == PLAYER_X)
										{
											EvalState[x - i, y + i] += DefenseScore[countAI];
										}
										else EvalState[x - i, y + i] += AttackScore[countAI];
									}
									if (countAI == 4 || countUser == 4)
									{
										EvalState[x - i, y + i] *= 2;
									}
								}
							}
						}
					}
				}
			}

			public List<EvalCell> GetOptimalList()
			{
				int size = 8;
				int[] maxValueList = new int[size];
				Cell[] maxCellList = new Cell[size];
				for (int i = 0; i < size; i++)
				{
					maxValueList[i] = int.MaxValue;
					maxCellList[i] = new Cell();
				}

				for (int x = 0; x < BOARD_SIZE; x++)
				{
					for (int y = 0; y < BOARD_SIZE; y++)
					{
						int value = GetEvalCellValue(x, y);
						for (int i = size - 1; i >= 0; i--)
						{
							if (maxValueList[i] <= value && value != 0)
							{
								for (int j = 0; j < i; j++)
								{
									maxValueList[j] = maxValueList[j + 1];
									maxCellList[j].SetLocation(maxCellList[j + 1].GetX(), maxCellList[j + 1].GetY());
								}

								maxValueList[i] = value;
								maxCellList[i].SetLocation(x, y);
								break;
							}
						}
					}
				}
				int maxLength = LengthNum(maxValueList[size - 1]);
				int[] difference = { 0, 2, 8, 32, 128, 512 };

				List<EvalCell> list = new List<EvalCell>();
				list.Add(new EvalCell(maxCellList[size - 1], maxValueList[size - 1]));
				for (int i = size - 2; i >= 0; i--)
				{
					if (maxValueList[size - 1] - maxValueList[i] <= difference[maxLength])
					{
						list.Add(new EvalCell(maxCellList[i], maxValueList[i]));
					}
					else break;
				}
				return list;
			}

			public int GetEvalCellValue(int x, int y)
			{
				return EvalState[x, y];
			}

			public void SetEvalCell(int x, int y, char value)
			{
				EvalState[x, y] = value;
			}

			private int LengthNum(int a)
			{
				if (a == 0) return 1;
				if (a < 0) a *= -1;
				int dem = 0;
				while (a > 0)
				{
					a /= 10;
					dem++;
				}
				return dem;
			}
		}

		public class AIHandler
		{
			Random rand;
			private int NextX;
			private int NextY;
			private int Mode;
			private GameBoard Root;
			private Heuristic Heuristic;

			public AIHandler(int mode)
			{
				rand = new Random();
				Mode = mode;
				if (Mode == 1)
				{
					Root = new GameBoard();
					Root.Update(BOARD_SIZE / 2, BOARD_SIZE / 2, PLAYER_O); //To do.........
					NextX = BOARD_SIZE / 2;
					NextY = BOARD_SIZE / 2;
				}
				else Root = new GameBoard();
				Heuristic = new Heuristic();
			}

			public int GetNextX()
			{
				return NextX;
			}

			public int GetNextY()
			{
				return NextY;
			}

			public bool CheckWinner(char player)
			{
				return Root.CheckWinner(player);
			}

			public void Update(int x, int y, char value)
			{
				Root.Update(x, y, value);
			}

			public bool EnableClick(int x, int y)
			{
				return Root.EnableClick(x, y);
			}

			public bool IsOver()
			{
				return Root.IsOver();
			}

			public void NextStep()
			{
				Cell choice = AlphaBeta(Root);
				if (choice == null) Console.WriteLine("Err, khong tim thay duong di");
				else
				{
					NextX = choice.GetX();
					NextY = choice.GetY();
					Console.WriteLine("=> AI: " + NextX + " " + NextY);

					if (!EnableClick(NextX, NextY)) Console.WriteLine("Nuoc di lap lai!");
					else Update(NextX, NextY, PLAYER_O);
				}
			}

			public Cell AlphaBeta(GameBoard state)
			{
				GameBoard remState = new GameBoard(state.GetState());
				Heuristic.EvaluateEachCell(remState, PLAYER_O);
				List<EvalCell> list = Heuristic.GetOptimalList();

				int maxValue = int.MinValue;
				int n = list.Count();
				List<EvalCell> ListChoose = new List<EvalCell>();
				for (int i = 0; i < n; i++)
				{
					remState.GetState()[list[i].getX(), list[i].getY()] = PLAYER_O;
					int value = MinValue(remState, int.MinValue, int.MaxValue, 0);
					if (maxValue < value)
					{
						maxValue = value;
						ListChoose.Clear();
						ListChoose.Add(list[i]);
					}
					else if (maxValue == value) ListChoose.Add(list[i]);
					remState.GetState()[list[i].getX(), list[i].getY()] = EMPTY;
				}
				n = ListChoose.Count();
				int x = rand.Next(n);
				return ListChoose.GetRange(x, 1).First().GetCell();
			}

			private int MaxValue(GameBoard state, int alpha, int beta, int depth)
			{
				if (depth >= 3 || state.CheckWinner(PLAYER_O) || state.IsOver())
				{
					int val = Heuristic.EvaluateState(state);
					return val;
				}
				Heuristic.EvaluateEachCell(state, PLAYER_O);
				List<EvalCell> list = Heuristic.GetOptimalList();
				for (int i = 0; i < list.Count(); i++)
				{
					state.GetState()[list[i].GetCell().GetX(), list[i].GetCell().GetY()] = PLAYER_O;
					alpha = Math.Max(alpha, MinValue(state, alpha, beta, depth + 1));
					state.GetState()[list[i].GetCell().GetX(), list[i].GetCell().GetY()] = EMPTY;
					if (alpha >= beta)
					{
						break;
					}
				}
				return alpha;
			}

			private int MinValue(GameBoard state, int alpha, int beta, int depth)
			{
				if (depth >= 3 || state.CheckWinner(PLAYER_X) || state.IsOver())
				{
					int val = Heuristic.EvaluateState(state);
					return val;
				}
				Heuristic.EvaluateEachCell(state, PLAYER_X);
				List<EvalCell> list = Heuristic.GetOptimalList();
				for (int i = 0; i < list.Count(); i++)
				{
					state.GetState()[list[i].GetCell().GetX(), list[i].GetCell().GetY()] = PLAYER_X;
					beta = Math.Min(beta, MaxValue(state, alpha, beta, depth + 1));
					state.GetState()[list[i].GetCell().GetX(), list[i].GetCell().GetY()] = EMPTY;
					if (alpha >= beta)
					{
						break;
					}
				}
				return beta;
			}
		}
	}
}
