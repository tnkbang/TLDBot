using Discord.WebSocket;
using Discord.Rest;
using Discord;
using Lavalink4NET.Events.Players;

namespace TLDBot.Utility
{
	public class Helper
	{
		//Action
		public static readonly string ACTION_PLAY		= "Play";
		public static readonly string ACTION_PAUSE		= "Pause";
		public static readonly string ACTION_RESUME		= "Resume";
		public static readonly string ACTION_LOOP		= "Loop";
		public static readonly string ACTION_SKIP		= "Skip";
		public static readonly string ACTION_STOP		= "Stop";

		public static readonly string[] BTN_PAUSE = new string[] { ACTION_RESUME, ACTION_LOOP, ACTION_SKIP, ACTION_STOP };
		public static readonly string[] BTN_RESUME = new string[] { ACTION_PAUSE, ACTION_LOOP, ACTION_SKIP, ACTION_STOP };

		public static readonly int SECOND_WAIT = 10;

		public static Dictionary<ulong, RestFollowupMessage> GuildPlayer = new Dictionary<ulong, RestFollowupMessage>();

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
		/// Create button
		/// </summary>
		/// <param name="lstAction">List button action type</param>
		/// <returns></returns>
		public static MessageComponent CreateButtons(string[] lstAction)
		{
			ComponentBuilder builder = new ComponentBuilder();

			foreach (string action in lstAction)
			{
				ButtonComponents btnC = new ButtonComponents();
				builder.WithButton(btnC.ExecuteButtonBuilder(action, ButtonComponents.TYPE_MUSIC));
			}

			return builder.Build();
		}

		/// <summary>
		/// Create button for message playing
		/// </summary>
		/// <param name="isPause"></param>
		/// <returns></returns>
		public static MessageComponent CreateButtonsMusicPlaying(bool isPause)
		{
			return CreateButtons(isPause ? BTN_PAUSE : BTN_RESUME);
		}

		/// <summary>
		/// Catch the track start event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		/// <returns></returns>
		public static async Task TrackStartedAsync(object sender, TrackStartedEventArgs eventArgs)
		{
			RestFollowupMessage? followupMessage;
			GuildPlayer.TryGetValue(eventArgs.Player.GuildId, out followupMessage);

			if (followupMessage is not null)
			{
				await followupMessage.ModifyAsync(msg => msg.Content = "Playing: " + eventArgs.Player.CurrentTrack!.Uri).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Catch the player destroy event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		/// <returns></returns>
		public static async Task PlayerDestroyedAsync(object sender, PlayerDestroyedEventArgs eventArgs)
		{
			RestFollowupMessage? followupMessage;
			GuildPlayer.TryGetValue(eventArgs.Player.GuildId, out followupMessage);

			if(followupMessage is not null)
			{
				await followupMessage.DeleteAsync().ConfigureAwait(false);
				GuildPlayer.Remove(eventArgs.Player.GuildId);
			}
		}
	}
}
