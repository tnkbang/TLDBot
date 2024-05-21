using Discord;
using Discord.WebSocket;
using TLDBot.Utility;

namespace TLDBot.Handlers
{
	public class HooHeyHowHandler
	{
		//Base game item
		public static readonly string DEER		= "Deer";
		public static readonly string CALABASH	= "Calabash";
		public static readonly string CHICKEN	= "Chicken";
		public static readonly string FISH		= "Fish";
		public static readonly string CRAB		= "Crab";
		public static readonly string LOBSTER	= "Lobster";

		private static readonly string FistLine = "--------------" + Environment.NewLine;
		private static readonly string LastLine = Environment.NewLine + "--------------";
		public static readonly string StartDes	= FistLine + Emotes.HooHeyHow + "ㅤ" + Emotes.HooHeyHow + "ㅤ" + Emotes.HooHeyHow + LastLine;

		private static readonly string[] Item = new string[] { DEER, CALABASH, CHICKEN, FISH, CRAB, LOBSTER };

		protected static Dictionary<string, string> strKey = new Dictionary<string, string>
		{
			{"nai", "Deer" }, {"bầu", "Calabash"}, {"gà", "Chicken"}, {"cá", "Fish"}, {"cua", "Crab"}, {"tôm", "Lobster"},
			{"deer", "Deer" }, {"calabash", "Calabash"}, {"chicken", "Chicken"}, {"fish", "Fish"}, {"crab", "Crab"}, {"lobster", "Lobster"}
		};

		private static Dictionary<ulong, dynamic[]> UserState = new Dictionary<ulong, dynamic[]>();

		//Meme thumb embed when user win
		private static readonly string[] MemeLoser = new string[]
		{
			"https://i.imgur.com/oWrb3su.png",
			"https://i.imgur.com/jHY5BRT.jpeg",
			"https://i.imgur.com/ZTipYSX.jpeg",
			"https://i.imgur.com/sqUcQm4.jpeg",
			"https://i.imgur.com/ltjIDdP.png"
		};

		//Meme thumb embed when user lose
		private static readonly string[] MemeWiner = new string[]
		{
			"https://i.imgur.com/wOqEetb.gif",
			"https://i.imgur.com/IewdrN0.gif",
			"https://i.imgur.com/CPnPMNv.gif",
			"https://i.imgur.com/tYrDTE9.gif",
			"https://i.imgur.com/sKqLgc5.png",
			"https://i.imgur.com/S2JxCnK.gif",
			"https://i.imgur.com/iCGRIQH.png"
		};

		//Button when start game
		public static MessageComponent Component
		{
			get
			{
				ComponentBuilder builder = new ComponentBuilder();
				builder = Helper.CreateButtons(builder, [DEER, CALABASH, CHICKEN], Buttons.TYPE_GAME);
				builder = Helper.CreateButtons(builder, [FISH, CRAB, LOBSTER], Buttons.TYPE_GAME, 1);
				return builder.Build();
			}
		}

		private readonly SocketUser User;

		private string[]? _baseChoice = null;
		private string? _userChoice = null;

		private int _countCorrect
		{
			get
			{
				if (_baseChoice is null || _userChoice is null) return 0;

				int cr = 0;
				foreach (var item in _baseChoice)
				{
					if (item.Equals(_userChoice)) cr++;
				}
				return cr;
			}
		}

		private bool _isCorrect
		{
			get
			{
				return _countCorrect > 0;
			}
		}

		public HooHeyHowHandler(SocketUser user)
		{
			User = user;
		}

		/// <summary>
		/// Random value choice
		/// </summary>
		/// <returns></returns>
		private string GenerateChoice(bool isChange = false)
		{
			if (isChange is false) return Item[new Random().Next(Item.Length)];

			string choice;
			do
			{
				choice = Item[new Random().Next(Item.Length)];
			} while (choice.Equals(_userChoice));

			return choice;
		}

		/// <summary>
		/// Set value choice for baseChoice
		/// </summary>
		private void SetValueChoice()
		{
			dynamic[]? state;
			UserState.TryGetValue(User.Id, out state);

			_baseChoice = [GenerateChoice(), GenerateChoice(), GenerateChoice()];

			//User first using
			if (state is null)
			{
				//Add user state, default is 1
				UserState[User.Id] = [_isCorrect, 1];
				return;
			}

			//If user wins or loses too many times (3 times)
			if (state[1] is 3)
			{
				if (state[0] is true)
				{
					_baseChoice = [GenerateChoice(true), GenerateChoice(true), GenerateChoice(true)];
				}
				else if (_baseChoice.Contains(_userChoice) is false)
				{
					//If user lose 3 round then user will win
					_baseChoice[new Random().Next(0, 2)] = _userChoice!;
				}
			}

			int count = (_isCorrect == state[0]) ? (state[1] + 1) : 1;
			UserState[User.Id] = [_isCorrect, count];
		}

		/// <summary>
		/// Set text state choice
		/// </summary>
		/// <returns></returns>
		private string GetChoiceResult()
		{
			if (_userChoice is null) return "Error";
			string rsl = "";

			//Joke if user choice is Calabash
			if (_userChoice.Equals(CALABASH) && (DateTime.Now.Second % 2 == 0))
			{
				rsl += "||";
				if (_isCorrect) rsl += Helper.HooHeyHow.JokeWin[_countCorrect.ToString()];
				else rsl += Helper.HooHeyHow.JokeLose;
				rsl += "||";

				return rsl;
			}

			if (_isCorrect) rsl += Helper.HooHeyHow.ChoiceWin[_countCorrect.ToString()];
			else rsl += Helper.HooHeyHow.ChoiceLose;

			return rsl + Emotes.GetByName(_userChoice!);
		}

		/// <summary>
		/// Embed message affter user click
		/// </summary>
		/// <param name="choice"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		protected Embed GetEmbedProcess(string choice, SocketUser user)
		{
			_userChoice = choice;
			SetValueChoice();
			if (_baseChoice is null) return new EmbedBuilder().WithDescription("Server Error").Build();

			string thumbnail = _isCorrect ? MemeWiner[new Random().Next(MemeWiner.Length)] : MemeLoser[new Random().Next(MemeLoser.Length)];
			string title = (_isCorrect ? Helper.HooHeyHow.Winner : Helper.HooHeyHow.Loser) + user.GlobalName;
			string description = FistLine + Emotes.GetByName(_baseChoice[0]) + "ㅤ" + Emotes.GetByName(_baseChoice[1]) + "ㅤ" + Emotes.GetByName(_baseChoice[2]) + LastLine;
			string strResult = GetChoiceResult();
			Color color = _isCorrect ? Color.Red : Color.DarkGrey;

			return Embeds.H3Process(title, thumbnail, description, strResult, color, user);
		}

		/// <summary>
		/// Respond message game
		/// </summary>
		/// <param name="choice"></param>
		/// <returns></returns>
		public virtual async Task RespondAsync(string choice)
		{
			await Task.CompletedTask;
		}
	}
}
