using Discord.Interactions;
using Lavalink4NET.Players.Vote;
using Lavalink4NET.Players;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using TLDBot.Structs;

namespace TLDBot.Commands
{
	public sealed class MusicCommands : InteractionModuleBase<SocketInteractionContext>
	{
		private readonly IAudioService _audioService;

		/// <summary>
		/// Initializes a new instance of the <see cref="MusicCommands"/> class.
		/// </summary>
		/// <param name="audioService">The audio service</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if the specified <paramref name="audioService"/> is <see langword="null"/>.
		/// </exception>
		public MusicCommands(IAudioService audioService)
		{
			ArgumentNullException.ThrowIfNull(audioService);

			_audioService = audioService;
		}

		#region Disconnect command
		/// <summary>
		/// Disconnects from the current voice channel connected to asynchronously.
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		[SlashCommand("disconnect", "Disconnects from the current voice channel connected to", runMode: RunMode.Async)]
		public async Task Disconnect()
		{
			var player = await GetPlayerAsync().ConfigureAwait(false);

			if (player is null)
			{
				return;
			}

			await player.DisconnectAsync().ConfigureAwait(false);
			await RespondAsync("Disconnected.").ConfigureAwait(false);
		}
		#endregion

		#region Play music command
		/// <summary>
		/// Plays music asynchronously.
		/// </summary>
		/// <param name="query">The search query</param>
		/// <returns>A task that represents the asynchronous operation</returns>
		[SlashCommand("play", description: "Plays music", runMode: RunMode.Async)]
		public async Task Play(string query)
		{
			await DeferAsync().ConfigureAwait(false);

			var player = await GetPlayerAsync(connectToVoiceChannel: true).ConfigureAwait(false);

			if (player is null)
			{
				return;
			}

			var track = await _audioService.Tracks
				.LoadTrackAsync(query, TrackSearchMode.YouTube)
				.ConfigureAwait(false);

			if (track is null)
			{
				await FollowupAsync("😖 No results.").ConfigureAwait(false);
				return;
			}

			var position = await player.PlayAsync(track).ConfigureAwait(false);

			if (position is 0)
			{
				await FollowupAsync($"🔈 Playing: {track.Uri}", components: Helper.CreateButtonPlaying()).ConfigureAwait(false);
			}
			else
			{
				await FollowupAsync($"🔈 Added to queue: {track.Uri}").ConfigureAwait(false);
			}
		}
		#endregion

		#region Time song in playing
		/// <summary>
		/// Shows the track position asynchronously.
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		[SlashCommand("position", description: "Shows the track position", runMode: RunMode.Async)]
		public async Task Position()
		{
			var player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);

			if (player is null)
			{
				return;
			}

			if (player.CurrentItem is null)
			{
				await RespondAsync("Nothing playing!").ConfigureAwait(false);
				return;
			}

			await RespondAsync($"Position: {player.Position?.Position} / {player.CurrentTrack?.Duration}.").ConfigureAwait(false);
		}
		#endregion

		#region Stop command
		/// <summary>
		/// Stops the current track asynchronously.
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		[SlashCommand("stop", description: "Stops the current track", runMode: RunMode.Async)]
		public async Task Stop()
		{
			var player = await GetPlayerAsync(connectToVoiceChannel: false);

			if (player is null)
			{
				return;
			}

			if (player.CurrentItem is null)
			{
				await RespondAsync("Nothing playing!").ConfigureAwait(false);
				return;
			}

			await player.StopAsync().ConfigureAwait(false);
			await RespondAsync("Stopped playing.").ConfigureAwait(false);
		}
		#endregion

		#region Volume command
		/// <summary>
		/// Updates the player volume asynchronously.
		/// </summary>
		/// <param name="volume">The volume (1 - 1000)</param>
		/// <returns>A task that represents the asynchronous operation</returns>
		[SlashCommand("volume", description: "Sets the player volume (0 - 1000%)", runMode: RunMode.Async)]
		public async Task Volume(int volume = 100)
		{
			if (volume is > 1000 or < 0)
			{
				await RespondAsync("Volume out of range: 0% - 1000%!").ConfigureAwait(false);
				return;
			}

			var player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);

			if (player is null)
			{
				return;
			}

			await player.SetVolumeAsync(volume / 100f).ConfigureAwait(false);
			await RespondAsync($"Volume updated: {volume}%").ConfigureAwait(false);
		}
		#endregion

		#region Skip command
		/// <summary>
		/// Skip to next songs in queues
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		[SlashCommand("skip", description: "Skips the current track", runMode: RunMode.Async)]
		public async Task Skip()
		{
			var player = await GetPlayerAsync(connectToVoiceChannel: false);

			if (player is null)
			{
				return;
			}

			if (player.CurrentItem is null)
			{
				await RespondAsync("Nothing playing!").ConfigureAwait(false);
				return;
			}

			await player.SkipAsync().ConfigureAwait(false);

			var track = player.CurrentItem;

			if (track is not null)
			{
				await RespondAsync($"Skipped. Now playing: {track.Track!.Uri}").ConfigureAwait(false);
			}
			else
			{
				await RespondAsync("Skipped. Stopped playing because the queue is now empty.").ConfigureAwait(false);
			}
		}
		#endregion

		#region Pause command
		/// <summary>
		/// Pause song in playing
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		[SlashCommand("pause", description: "Pauses the player.", runMode: RunMode.Async)]
		public async Task PauseAsync()
		{
			var player = await GetPlayerAsync(connectToVoiceChannel: false);

			if (player is null)
			{
				return;
			}

			if (player.State is PlayerState.Paused)
			{
				await RespondAsync("Player is already paused.").ConfigureAwait(false);
				return;
			}

			await player.PauseAsync().ConfigureAwait(false);
			await RespondAsync("Paused.").ConfigureAwait(false);
		}
		#endregion

		#region Resume command
		/// <summary>
		/// Resume song in playing
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		[SlashCommand("resume", description: "Resumes the player.", runMode: RunMode.Async)]
		public async Task ResumeAsync()
		{
			var player = await GetPlayerAsync(connectToVoiceChannel: false);

			if (player is null)
			{
				return;
			}

			if (player.State is not PlayerState.Paused)
			{
				await RespondAsync("Player is not paused.").ConfigureAwait(false);
				return;
			}

			await player.ResumeAsync().ConfigureAwait(false);
			await RespondAsync("Resumed.").ConfigureAwait(false);
		}
		#endregion

		#region Helper
		/// <summary>
		/// Gets the guild player asynchronously.
		/// </summary>
		/// <param name="connectToVoiceChannel">A value indicating whether to connect to a voice channel</param>
		/// <returns>A task that represents the asynchronous operation. The task result is the lavalink player.</returns>
		private async ValueTask<VoteLavalinkPlayer?> GetPlayerAsync(bool connectToVoiceChannel = true)
		{
			var retrieveOptions = new PlayerRetrieveOptions(
				ChannelBehavior: connectToVoiceChannel ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None);

			var result = await _audioService.Players
				.RetrieveAsync(Context, playerFactory: PlayerFactory.Vote, retrieveOptions)
				.ConfigureAwait(false);

			if (!result.IsSuccess)
			{
				var errorMessage = result.Status switch
				{
					PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel.",
					PlayerRetrieveStatus.BotNotConnected => "The bot is currently not connected.",
					_ => "Unknown error.",
				};

				await FollowupAsync(errorMessage).ConfigureAwait(false);
				return null;
			}

			return result.Player;
		}
		#endregion
	}
}
