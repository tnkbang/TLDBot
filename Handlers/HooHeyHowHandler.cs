using Discord;
using Discord.Commands;
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
				builder = Helper.CreateButtons(builder, [DEER, CALABASH, CHICKEN], ButtonComponents.TYPE_GAME);
				builder = Helper.CreateButtons(builder, [FISH, CRAB, LOBSTER], ButtonComponents.TYPE_GAME, 1);
				return builder.Build();
			}
		}

		private readonly SocketMessageComponent? _messageComponent = null;

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

		public HooHeyHowHandler(SocketMessageComponent? messageComponent = null)
		{
			_messageComponent = messageComponent;
		}

		/// <summary>
		/// Random value choice
		/// </summary>
		/// <returns></returns>
		private string GenerateChoice()
		{
			return Item[new Random().Next(Item.Length)];
		}

		/// <summary>
		/// Set value choice for baseChoice
		/// </summary>
		private void SetValueChoice()
		{
			_baseChoice = [GenerateChoice(), GenerateChoice(), GenerateChoice()];
		}

		/// <summary>
		/// Set text state choice
		/// </summary>
		/// <returns></returns>
		private string GetChoiceResult()
		{
			if(_userChoice is null) return "Error";
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
		private Embed GetEmbedProcess(string choice, SocketUser user)
		{
			_userChoice = choice;
			SetValueChoice();
			if (_baseChoice is null) return new EmbedBuilder().WithDescription("Server Error").Build();

			string thumbnail = _isCorrect ? MemeWiner[new Random().Next(MemeWiner.Length)] : MemeLoser[new Random().Next(MemeLoser.Length)];
			string title = (_isCorrect ? Helper.HooHeyHow.Winner : Helper.HooHeyHow.Loser) + user.GlobalName;
			string description = FistLine + Emotes.GetByName(_baseChoice[0]) + "ㅤ" + Emotes.GetByName(_baseChoice[1]) + "ㅤ" + Emotes.GetByName(_baseChoice[2]) + LastLine;
			string strResult = GetChoiceResult();
			Color color = _isCorrect ? Color.Red : Color.DarkGrey;

			return UtilEmbed.H3Process(title, thumbnail, description, strResult, color, user);
		}

		/// <summary>
		/// Respond message game
		/// </summary>
		/// <param name="choice"></param>
		/// <returns></returns>
		public async Task RespondAsync(string choice)
		{
			if (_messageComponent is null) return;

			await _messageComponent!.UpdateAsync(msg =>
			{
				msg.Embed = GetEmbedProcess(choice, _messageComponent.User);
				msg.Components = new ComponentBuilder().Build();
			});
		}
	}
}
