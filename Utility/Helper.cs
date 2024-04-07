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
		public static readonly string ACTION_SEEK_P10	= "SeekPrev10S";
		public static readonly string ACTION_SEEK_N10	= "SeekNext10S";
		public static readonly string ACTION_STOP		= "Stop";
		public static readonly string ACTION_QUEUE		= "Queue";
		public static readonly string ACTION_LYRICS		= "Lyrics";

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
			builder = CreateButtons(builder, [isPause ? ACTION_RESUME : ACTION_PAUSE, ACTION_LOOP, ACTION_SHUFFLE], ButtonComponents.TYPE_MUSIC);
			builder = CreateButtons(builder, [ACTION_SEEK_P10, ACTION_STOP, ACTION_SEEK_N10], ButtonComponents.TYPE_MUSIC, 1);
			builder = CreateButtons(builder, [ACTION_SKIP, ACTION_QUEUE, ACTION_LYRICS], ButtonComponents.TYPE_MUSIC, 2);

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
