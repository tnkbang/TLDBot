using Discord.WebSocket;
using Discord;
using Lavalink4NET;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Vote;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using TLDBot.Utility;
using TLDBot.Structs;
using Lavalink4NET.Tracks;

namespace TLDBot.Handlers
{
	public class MusicHandler
	{
		//Action
		public static readonly string PLAY		= "Play";
		public static readonly string PAUSE		= "Pause";
		public static readonly string RESUME	= "Resume";
		public static readonly string LOOP		= "Loop";
		public static readonly string SKIP		= "Skip";
		public static readonly string SHUFFLE	= "Shuffle";
		public static readonly string SEEK_P10	= "SeekPrev10S";
		public static readonly string SEEK_N10	= "SeekNext10S";
		public static readonly string STOP		= "Stop";
		public static readonly string QUEUE		= "Queue";
		public static readonly string LYRICS	= "Lyrics";
		public static readonly string POSITION	= "Position";

		public static readonly int SECOND_WAIT	= 10;
		public static readonly int QUEUE_WAIT	= 20;

		public static Dictionary<ulong, GuildPlayerMessage> GuildPlayer = new Dictionary<ulong, GuildPlayerMessage>();

		protected PlayerResult<VoteLavalinkPlayer> _playerResult;
		protected readonly IAudioService _audioService;

		public MusicHandler(IAudioService audioService)
		{
			_audioService = audioService;
		}

		/// <summary>
		/// Button is playing message
		/// </summary>
		/// <param name="isPause"></param>
		/// <returns></returns>
		public static MessageComponent GetComponent(bool isPause)
		{

			ComponentBuilder builder = new ComponentBuilder();
			builder = Helper.CreateButtons(builder, [isPause ? RESUME : PAUSE, LOOP, SHUFFLE], Buttons.TYPE_MUSIC);
			builder = Helper.CreateButtons(builder, [SEEK_P10, STOP, SEEK_N10], Buttons.TYPE_MUSIC, 1);
			builder = Helper.CreateButtons(builder, [SKIP, QUEUE, POSITION], Buttons.TYPE_MUSIC, 2);

			return builder.Build();
		}

		/// <summary>
		/// Disconnects from the current voice channel connected to asynchronously.
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		public async Task DisconnectAsync()
		{
			await DeferAsync().ConfigureAwait(false);

			VoteLavalinkPlayer? player = await GetPlayerAsync(true).ConfigureAwait(false);
			if (player is null) return;

			await player.DisconnectAsync().ConfigureAwait(false);
			await FollowupAsync(message: "Disconnect.").ConfigureAwait(false);
		}

		/// <summary>
		/// Plays music asynchronously.
		/// </summary>
		/// <param name="query">The search query</param>
		/// <returns>A task that represents the asynchronous operation</returns>
		public async Task PlayAsync(string query, SocketUser user)
		{
			await DeferAsync().ConfigureAwait(false);

			VoteLavalinkPlayer? player = await GetPlayerAsync(true).ConfigureAwait(false);
			if (player is null) return;

			LavalinkTrack? track = await _audioService.Tracks.LoadTrackAsync(query, TrackSearchMode.YouTube).ConfigureAwait(false);
			if (track is null)
			{
				await FollowupAsync(title: "Search Playing", message: "No results.").ConfigureAwait(false);
				return;
			}

			int position = await player.PlayAsync(track).ConfigureAwait(false);
			if (position is 0)
			{
				await FollowupAsync(components: GetComponent(isPause: false), embed: Embeds.Playing(player, track, user), isPlaying: true, isUpdateEmbed: true).ConfigureAwait(false);
				return;
			}

			await FollowupAsync(title: "Playing", message: $"Added to queue: **{track.Title}**", isUpdateEmbed: true).ConfigureAwait(false);
		}

		protected async Task SearchAsync(IReadOnlyCollection<string> collection, SocketUser user)
		{
			VoteLavalinkPlayer? player = await GetPlayerAsync(true).ConfigureAwait(false);
			if (player is null) return;

			LavalinkTrack? ftrack = null;
			foreach (string item in collection)
			{
				LavalinkTrack? track = await _audioService.Tracks.LoadTrackAsync(item, TrackSearchMode.YouTube).ConfigureAwait(false);
				if (track is null) continue;

				int position = await player.PlayAsync(track).ConfigureAwait(false);
				if (position is 0) ftrack = track;
			}

			if (ftrack is not null)
			{
				await FollowupAsync(components: GetComponent(isPause: false), embed: Embeds.Playing(player, ftrack, user), isPlaying: true, isUpdateEmbed: true).ConfigureAwait(false);
				return;
			}
			await FollowupAsync(title: "Playing", message: $"Added **{collection.Count} track** to queue.", isUpdateEmbed: true).ConfigureAwait(false);
		}

		public async Task SearchAsync(string query)
		{
			await DeferAsync().ConfigureAwait(false);
			VoteLavalinkPlayer? player = await GetPlayerAsync(true).ConfigureAwait(false);
			if (player is null) return;

			TrackLoadResult tracks = await _audioService.Tracks.LoadTracksAsync(query, TrackSearchMode.YouTube).ConfigureAwait(false);
			if (tracks.Count is 0) return;

			SelectMenuBuilder menuBuilder = new SelectMenuBuilder().WithPlaceholder("Chọn các bài hát muốn phát").WithCustomId("Search").WithMaxValues(10);

			int count = 0;
			foreach (LavalinkTrack track in tracks.Tracks)
			{
				if (count >= 10) break; count++;
				menuBuilder.AddOption(track.Title, track.Uri!.ToString(), track.Duration.ToString());
			}

			MessageComponent component = new ComponentBuilder().WithSelectMenu(menuBuilder).Build();
			await FollowupAsync(component: component).ConfigureAwait(false);
		}

		/// <summary>
		/// Shows the track position asynchronously.
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		public async Task PositionAsync()
		{
			VoteLavalinkPlayer? player = await GetPlayerAsync().ConfigureAwait(false);
			if (player is null) return;

			if (player.CurrentItem is null)
			{
				await RespondAsync(message: "Nothing playing!").ConfigureAwait(false);
				return;
			}

			await RespondAsync(title: "Track position", message: $"Position: {player.Position?.Position.ToString(@"hh\:mm\:ss")} / {player.CurrentTrack?.Duration}.").ConfigureAwait(false);
		}

		/// <summary>
		/// Stops the current track asynchronously.
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		public async Task StopAsync()
		{
			VoteLavalinkPlayer? player = await GetPlayerAsync().ConfigureAwait(false);
			if (player is null) return;

			if (player.CurrentItem is null)
			{
				await RespondAsync(message: "Nothing playing!").ConfigureAwait(false);
				return;
			}

			await player.StopAsync().ConfigureAwait(false);
			await RespondAsync(message: "Stopped playing.").ConfigureAwait(false);
		}

		/// <summary>
		/// Updates the player volume asynchronously.
		/// </summary>
		/// <param name="volume">The volume (1 - 1000)</param>
		/// <returns>A task that represents the asynchronous operation</returns>
		public async Task VolumeAsync(int volume = 100)
		{
			if (volume is > 1000 or < 0)
			{
				await RespondAsync(title: "Volume err", message: "Volume out of range: 0% - 1000%!").ConfigureAwait(false);
				return;
			}

			VoteLavalinkPlayer? player = await GetPlayerAsync().ConfigureAwait(false);
			if (player is null) return;

			await player.SetVolumeAsync(volume / 100f).ConfigureAwait(false);
			await RespondAsync(title: "Volume", message: $"Volume updated: {volume}%", isUpdateEmbed: true).ConfigureAwait(false);
		}

		/// <summary>
		/// Skip to next songs in queues
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		public async Task SkipAsync()
		{
			VoteLavalinkPlayer? player = await GetPlayerAsync().ConfigureAwait(false);
			if (player is null) return;

			if (player.CurrentItem is null)
			{
				await RespondAsync(message: "Nothing playing!").ConfigureAwait(false);
				return;
			}

			await player.SkipAsync().ConfigureAwait(false);

			ITrackQueueItem track = player.CurrentItem;
			if (track is not null)
			{
				await RespondAsync(title: "Skipped", message: $"Now playing: **{track.Track!.Title}**").ConfigureAwait(false);
				return;
			}

			await RespondAsync(title: "Skipped", message: "Stopped playing because the queue is now empty.").ConfigureAwait(false);
		}

		/// <summary>
		/// Loop or unloop the current track and queue.
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		public async Task LoopAsync()
		{
			VoteLavalinkPlayer? player = await GetPlayerAsync().ConfigureAwait(false);
			if (player is null) return;

			switch (player.RepeatMode)
			{
				case TrackRepeatMode.None:
					player.RepeatMode = TrackRepeatMode.Track;
					await RespondAsync(title: "Loop", message: "Loop the current track.", isUpdateEmbed: true).ConfigureAwait(false);
					break;
				case TrackRepeatMode.Track:
					player.RepeatMode = TrackRepeatMode.Queue;
					await RespondAsync(title: "Loop", message: "Loop the current queue.", isUpdateEmbed: true).ConfigureAwait(false);
					break;
				case TrackRepeatMode.Queue:
					player.RepeatMode = TrackRepeatMode.None;
					await RespondAsync(title: "Loop", message: "Unloop the current queue.", isUpdateEmbed: true).ConfigureAwait(false);
					break;
			}
		}

		/// <summary>
		/// Shuffle queue
		/// </summary>
		/// <returns></returns>
		public async Task ShuffleAsync()
		{
			VoteLavalinkPlayer? player = await GetPlayerAsync().ConfigureAwait(false);
			if (player is null) return;

			player.Shuffle = !player.Shuffle;

			await RespondAsync(title: "Shuffle", message: (player.Shuffle ? "Shuffle" : "Un shuffle") + " the current queue.", isUpdateEmbed: true).ConfigureAwait(false);
		}

		/// <summary>
		/// Seek the current track
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		public async Task SeekAsync(TimeSpan time, bool isBegin = false)
		{
			VoteLavalinkPlayer? player = await GetPlayerAsync().ConfigureAwait(false);
			if (player is null) return;

			if (player.CurrentItem is null)
			{
				await RespondAsync(message: "Nothing playing!").ConfigureAwait(false);
				return;
			}
			
			await player.SeekAsync(time, isBegin ? SeekOrigin.Begin : SeekOrigin.Current).ConfigureAwait(false);
			await RespondAsync(title: "Seek", message: $"The track is play duration: {player.Position?.Position.ToString(@"hh\:mm\:ss")}").ConfigureAwait(false);
		}

		/// <summary>
		/// Pause song in playing
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		public async Task PauseAsync()
		{
			VoteLavalinkPlayer? player = await GetPlayerAsync().ConfigureAwait(false);
			if (player is null) return;

			if (player.State is PlayerState.Paused)
			{
				await RespondAsync(message: "Player is already paused.").ConfigureAwait(false);
				return;
			}

			await player.PauseAsync().ConfigureAwait(false);
			await RespondAsync(message: "Paused.", isUpdateEmbed: true, isUpdateComponent: true).ConfigureAwait(false);
		}

		/// <summary>
		/// Resume song in playing
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		public async Task ResumeAsync()
		{
			VoteLavalinkPlayer? player = await GetPlayerAsync().ConfigureAwait(false);
			if (player is null) return;

			if (player.State is not PlayerState.Paused)
			{
				await RespondAsync(message: "Player is not paused.").ConfigureAwait(false);
				return;
			}

			await player.ResumeAsync().ConfigureAwait(false);
			await RespondAsync(message: "Resumed.", isUpdateEmbed: true, isUpdateComponent: true).ConfigureAwait(false);
		}

		/// <summary>
		/// Get queue in the player
		/// </summary>
		/// <returns></returns>
		public async Task QueueAsync()
		{
			VoteLavalinkPlayer? player = await GetPlayerAsync().ConfigureAwait(false);
			if (player is null) return;

			await RespondAsync(QUEUE_WAIT, embed: Embeds.Queue(player)).ConfigureAwait(false);
		}

		/// <summary>
		/// Gets the guild player asynchronously.
		/// </summary>
		/// <param name="connectToVoiceChannel">A value indicating whether to connect to a voice channel</param>
		/// <returns>A task that represents the asynchronous operation. The task result is the lavalink player.</returns>
		private async ValueTask<VoteLavalinkPlayer?> GetPlayerAsync(bool connectToVoiceChannel = false)
		{
			PlayerRetrieveOptions retrieveOptions = new PlayerRetrieveOptions(ChannelBehavior: connectToVoiceChannel ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None);
			await SetPlayerAsync(retrieveOptions).ConfigureAwait(false);

			if (!_playerResult.IsSuccess)
			{
				string errorMessage = _playerResult.Status switch
				{
					PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel.",
					PlayerRetrieveStatus.BotNotConnected => "The bot is currently not connected.",
					_ => "Unknown error.",
				};

				if (connectToVoiceChannel) await FollowupAsync(errorMessage).ConfigureAwait(false);
				else await RespondAsync(errorMessage).ConfigureAwait(false);
				return null;
			}

			return _playerResult.Player;
		}

		protected virtual async Task SetPlayerAsync(PlayerRetrieveOptions retrieveOptions)
		{
			await Task.CompletedTask;
		}

		protected virtual async Task FollowupAsync(string? title = null, string? message = null, MessageComponent? components = null, Embed? embed = null, bool isPlaying = false, bool isUpdateEmbed = false)
		{
			await Task.CompletedTask;
		}

		protected virtual async Task FollowupAsync(MessageComponent component)
		{
			await Task.CompletedTask;
		}

		private async Task RespondAsync(string? title = null, string? message = null, bool isUpdateEmbed = false, bool isUpdateComponent = false)
		{
			if (isUpdateEmbed || isUpdateComponent)
			{
				await Helper.UpdatePlayingAsync(_playerResult.Player!, _playerResult.Player!.CurrentTrack!, isUpdateEmbed, isUpdateComponent).ConfigureAwait(false);
			}

			await RespondAsync(wait: SECOND_WAIT, embed: Embeds.Info(title, message)).ConfigureAwait(false);
		}

		protected virtual async Task RespondAsync(int wait, Embed? embed = null, MessageComponent? components = null)
		{
			await Task.CompletedTask;
		}

		protected virtual async Task DeferAsync()
		{
			await Task.CompletedTask;
		}
	}
}
