using Discord.WebSocket;
using Discord;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Tracks;
using Lavalink4NET.Players;

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
		public static readonly string ACTION_SHUFFLE	= "Shuffle";
		public static readonly string ACTION_SEEK_P5	= "SeekPrev5S";
		public static readonly string ACTION_SEEK_P15	= "SeekPrev15S";
		public static readonly string ACTION_SEEK_N5	= "SeekNext5S";
		public static readonly string ACTION_SEEK_N15	= "SeekNext15S";
		public static readonly string ACTION_STOP		= "Stop";
		public static readonly string ACTION_QUEUE		= "Queue";

		public static readonly string[] BTN_PAUSE = new string[] { ACTION_RESUME, ACTION_LOOP, ACTION_SHUFFLE, ACTION_SKIP, ACTION_QUEUE };
		public static readonly string[] BTN_RESUME = new string[] { ACTION_PAUSE, ACTION_LOOP, ACTION_SHUFFLE, ACTION_SKIP, ACTION_QUEUE };
		public static readonly string[] BTN_SEEK = new string[] { ACTION_SEEK_P15, ACTION_SEEK_P5, ACTION_STOP, ACTION_SEEK_N5, ACTION_SEEK_N15};

		public static readonly int SECOND_WAIT = 10;

		public static Dictionary<ulong, GuildPlayerMessage> GuildPlayer = new Dictionary<ulong, GuildPlayerMessage>();

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
		public static ComponentBuilder CreateButtons(ComponentBuilder builder, string[] lstAction,string type,  int row = 0)
		{
			foreach (string action in lstAction)
			{
				ButtonComponents btnC = new ButtonComponents();
				builder.WithButton(btnC.ExecuteButtonBuilder(action, type), row);
			}

			return builder;
		}

		/// <summary>
		/// Create button for message playing
		/// </summary>
		/// <param name="isPause"></param>
		/// <returns></returns>
		public static MessageComponent CreateButtonsMusicPlaying(bool isPause)
		{
			ComponentBuilder builder = new ComponentBuilder();
			builder = CreateButtons(builder, isPause ? BTN_PAUSE : BTN_RESUME, ButtonComponents.TYPE_MUSIC);
			builder = CreateButtons(builder, BTN_SEEK, ButtonComponents.TYPE_MUSIC, 1);

			return builder.Build();
		}

		/// <summary>
		/// Catch the track start event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		/// <returns></returns>
		public static async Task TrackStartedAsync(object sender, TrackStartedEventArgs eventArgs)
		{
			await UpdatePlayingAsync(eventArgs.Player, eventArgs.Track, isUpdateEmbed: true).ConfigureAwait(false);
		}

		public static async Task UpdatePlayingAsync(ILavalinkPlayer player, LavalinkTrack track, bool isUpdateEmbed = false, bool isUpdateComponent = false)
		{
			GuildPlayerMessage? playerMessage;
			GuildPlayer.TryGetValue(player.GuildId, out playerMessage);

			if (playerMessage is not null)
			{
				await playerMessage.Channel.ModifyMessageAsync(playerMessage.MessageId, msg => {
					msg.Embed = isUpdateEmbed ? UtilEmbed.Playing(playerMessage.VotePlayer, track, playerMessage.User) : msg.Embed;
					msg.Components = isUpdateComponent ? CreateButtonsMusicPlaying(isPause: player.State is PlayerState.Paused) : msg.Components;
				}).ConfigureAwait(false);
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
			GuildPlayerMessage? playerMessage;
			GuildPlayer.TryGetValue(eventArgs.Player.GuildId, out playerMessage);

			if(playerMessage is not null)
			{
				await playerMessage.Channel.DeleteMessageAsync(playerMessage.MessageId).ConfigureAwait(false);
				GuildPlayer.Remove(eventArgs.Player.GuildId);
			}
		}
	}
}
