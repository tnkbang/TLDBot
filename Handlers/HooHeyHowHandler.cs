using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TLDBot.Utility;

namespace TLDBot.Handlers
{
	public class HooHeyHowHandler
	{
		//Game HooHeyHow
		public static readonly string CALABASH = "Calabash";
		public static readonly string LOBSTER = "Lobster";
		public static readonly string CRAB = "Crab";
		public static readonly string CHICKEN = "Chicken";
		public static readonly string FISH = "Fish";
		public static readonly string DEER = "Deer";

		public static string[] HooHeyHow = new string[]
		{
			Emotes.Deer,
			Emotes.Calabash,
			Emotes.Chicken,
			Emotes.Fish,
			Emotes.Crab,
			Emotes.Lobster
		};

		private readonly SocketMessageComponent? _messageComponent = null;
		private readonly SocketCommandContext? _commandContext = null;

		public HooHeyHowHandler(SocketMessageComponent? messageComponent = null, SocketCommandContext? commandContext = null)
		{
			_messageComponent = messageComponent;
			_commandContext = commandContext;
		}

		private int GenerateValue()
		{
			return new Random().Next(0, 5);
		}

		public Embed Start()
		{
			EmbedBuilder embed = new EmbedBuilder();
			embed.WithTitle("Please choose a mascot")
				.AddField("ㅤ", "--------------" + Environment.NewLine + Emotes.HooHeyHow + "ㅤ" + Emotes.HooHeyHow + "ㅤ" + Emotes.HooHeyHow + Environment.NewLine + "--------------", inline: false)
				.WithColor(Color.LightOrange).WithCurrentTimestamp();

			return embed.Build();
		}

		public Embed Value(int choice)
		{
			int[] value = [GenerateValue(), GenerateValue(), GenerateValue()];
			bool equal = value.Contains(choice);
			string strResult = HooHeyHow[value[0]] + "ㅤ" + HooHeyHow[value[1]] + "ㅤ" + HooHeyHow[value[2]];

			EmbedBuilder embed = new EmbedBuilder();
			embed.WithTitle(equal ? "Congratulations" : "Condolences")
				.AddField("ㅤ", "--------------" + Environment.NewLine + strResult + Environment.NewLine + "--------------", inline: false)
				.AddField("ㅤ", equal ? "you win" : "you lose", inline: false)
				.WithColor(equal ? Color.Red : Color.DarkGrey).WithCurrentTimestamp();

			return embed.Build();
		}

		public MessageComponent Buttons()
		{
			ComponentBuilder builder = new ComponentBuilder();
			builder = Helper.CreateButtons(builder, [DEER, CALABASH, CHICKEN], ButtonComponents.TYPE_GAME);
			builder = Helper.CreateButtons(builder, [FISH, CRAB, LOBSTER], ButtonComponents.TYPE_GAME, 1);
			return builder.Build();
		}

		public async Task RespondAsync(int choice)
		{
			await _messageComponent!.UpdateAsync(msg =>
			{
				msg.Embed = Value(choice);
				msg.Components = new ComponentBuilder().Build();
			});
		}
	}
}
