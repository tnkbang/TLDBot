using Discord.WebSocket;
using Discord;

namespace TLDBot.Structs
{
	public class Helper
	{
		//Action
		public static readonly string ACTION_PLAY		= "Play";
		public static readonly string ACTION_PAUSE		= "Pause";
		public static readonly string ACTION_RESUME		= "Resume";
		public static readonly string ACTION_SKIP		= "Skip";
		public static readonly string ACTION_STOP		= "Stop";

		/// <summary>
		/// Static discord socket client
		/// </summary>
		public static DiscordSocketClient Client
		{
			get
			{
				return new DiscordSocketClient(new DiscordSocketConfig
				{
					GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent,
					LogLevel = LogSeverity.Info
				});
			}
		}

		/// <summary>
		/// Create button in music playing message
		/// </summary>
		/// <param name="id">Context interaction id</param>
		/// <returns></returns>
		public static MessageComponent CreateButtonPlaying()
		{
			ComponentBuilder builder = new ComponentBuilder();
			ActionRowBuilder firstRow = new ActionRowBuilder();

			builder.WithButton(ACTION_PAUSE,	"btn" + ACTION_PAUSE,		ButtonStyle.Primary)
					.WithButton(ACTION_SKIP,	"btn" + ACTION_SKIP,		ButtonStyle.Success)
					.WithButton(ACTION_STOP,	"btn" + ACTION_STOP,		ButtonStyle.Danger);

			builder.AddRow(firstRow);

			return builder.Build();
		}
	}
}
