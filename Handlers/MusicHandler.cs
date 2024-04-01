using Discord.Interactions;
using Lavalink4NET;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Vote;
using Lavalink4NET.Rest.Entities.Tracks;
using System.Reflection;
using Lavalink4NET.DiscordNet;
using TLDBot.Utility;
using Discord.Commands;


namespace TLDBot.Handlers
{
	public class MusicHandler
	{
		public MusicHandler() { }

		/// <summary>
		/// Get command music playing
		/// </summary>
		/// <param name="cmdName">Command name</param>
		/// <returns></returns>
		public async Task ExecuteCommandAsync(string cmdName)
		{
			MethodInfo? method = GetType().GetMethod(cmdName + "Async");
			if (method != null) _ = method.Invoke(this, null) as Task;

			Console.WriteLine("Func not found: " + cmdName + "Async");
			await Task.CompletedTask;
		}

		/// <summary>
		/// Disconnects from the current voice channel connected to asynchronously.
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		public async Task DisconnectAsync(IAudioService audioService, SocketInteractionContext context)
		{
			VoteLavalinkPlayer? player = await GetPlayerAsync(audioService, context).ConfigureAwait(false);
			if (player is null) return;

			await player.DisconnectAsync().ConfigureAwait(false);
			await context.Interaction.RespondAsync("Disconnect.");
		}

		public async Task DisconnectAsync2(IAudioService audioService, CommandContext context)
		{
			VoteLavalinkPlayer? player = await GetPlayerAsync2(audioService, context).ConfigureAwait(false);
			if (player is null) 
			{ 
				await context.Channel.SendMessageAsync("Not playing").ConfigureAwait(false);
				return;
			}

			await player.DisconnectAsync().ConfigureAwait(false);
			await context.Channel.SendMessageAsync("Disconnect.").ConfigureAwait(false);
		}

		/// <summary>
		/// Plays music asynchronously.
		/// </summary>
		/// <param name="query">The search query</param>
		/// <returns>A task that represents the asynchronous operation</returns>
		public async Task PlayAsync(IAudioService audioService, SocketInteractionContext context, string query)
		{
			await context.Interaction.DeferAsync().ConfigureAwait(false);

			VoteLavalinkPlayer? player = await GetPlayerAsync(audioService, context, connectToVoiceChannel: true).ConfigureAwait(false);
			if (player is null) return;

			var track = await audioService.Tracks.LoadTrackAsync(query, TrackSearchMode.YouTube).ConfigureAwait(false);

			if (track is null)
			{
				await context.Interaction.FollowupAsync("😖 No results.").ConfigureAwait(false);
				return;
			}

			var position = await player.PlayAsync(track).ConfigureAwait(false);

			if (position is 0)
			{
				await context.Interaction.FollowupAsync($"🔈 Playing: {track.Uri}",
					components: Helper.CreateButtons([Helper.ACTION_PAUSE, Helper.ACTION_LOOP, Helper.ACTION_SKIP, Helper.ACTION_STOP])).ConfigureAwait(false);
			}
			else
			{
				await context.Interaction.FollowupAsync($"🔈 Added to queue: {track.Uri}").ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Shows the track position asynchronously.
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		public async Task PositionAsync(IAudioService audioService, SocketInteractionContext context)
		{
			VoteLavalinkPlayer? player = await GetPlayerAsync(audioService, context, connectToVoiceChannel: false).ConfigureAwait(false);
			if (player is null) return;

			if (player.CurrentItem is null)
			{
				await context.Interaction.RespondAsync("Nothing playing!").ConfigureAwait(false);
				return;
			}

			await context.Interaction.RespondAsync($"Position: {player.Position?.Position} / {player.CurrentTrack?.Duration}.").ConfigureAwait(false);
		}

		/// <summary>
		/// Stops the current track asynchronously.
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		public async Task StopAsync(IAudioService audioService, SocketInteractionContext context)
		{
			VoteLavalinkPlayer? player = await GetPlayerAsync(audioService, context, connectToVoiceChannel: false);
			if (player is null) return;

			if (player.CurrentItem is null)
			{
				await context.Interaction.RespondAsync("Nothing playing!").ConfigureAwait(false);
				return;
			}

			await player.StopAsync().ConfigureAwait(false);
			await context.Interaction.RespondAsync("Stopped playing.").ConfigureAwait(false);
		}

		/// <summary>
		/// Updates the player volume asynchronously.
		/// </summary>
		/// <param name="volume">The volume (1 - 1000)</param>
		/// <returns>A task that represents the asynchronous operation</returns>
		public async Task VolumeAsync(IAudioService audioService, SocketInteractionContext context, int volume = 100)
		{
			if (volume is > 1000 or < 0)
			{
				await context.Interaction.RespondAsync("Volume out of range: 0% - 1000%!").ConfigureAwait(false);
				return;
			}

			VoteLavalinkPlayer? player = await GetPlayerAsync(audioService, context, connectToVoiceChannel: false).ConfigureAwait(false);
			if (player is null) return;

			await player.SetVolumeAsync(volume / 100f).ConfigureAwait(false);
			await context.Interaction.RespondAsync($"Volume updated: {volume}%").ConfigureAwait(false);
		}

		/// <summary>
		/// Skip to next songs in queues
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		public async Task SkipAsync(IAudioService audioService, SocketInteractionContext context)
		{
			VoteLavalinkPlayer? player = await GetPlayerAsync(audioService, context, connectToVoiceChannel: false);
			if (player is null) return;

			if (player.CurrentItem is null)
			{
				await context.Interaction.RespondAsync("Nothing playing!").ConfigureAwait(false);
				return;
			}

			await player.SkipAsync().ConfigureAwait(false);

			var track = player.CurrentItem;

			if (track is not null)
			{
				await context.Interaction.RespondAsync($"Skipped. Now playing: {track.Track!.Uri}").ConfigureAwait(false);
			}
			else
			{
				await context.Interaction.RespondAsync("Skipped. Stopped playing because the queue is now empty.").ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Pause song in playing
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		public async Task PauseAsync(IAudioService audioService, SocketInteractionContext context)
		{
			VoteLavalinkPlayer? player = await GetPlayerAsync(audioService, context, connectToVoiceChannel: false);
			if (player is null) return;

			if (player.State is PlayerState.Paused)
			{
				await context.Interaction.RespondAsync("Player is already paused.").ConfigureAwait(false);
				return;
			}

			await player.PauseAsync().ConfigureAwait(false);
			await context.Interaction.RespondAsync("Paused.").ConfigureAwait(false);
		}

		/// <summary>
		/// Resume song in playing
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		public async Task ResumeAsync(IAudioService audioService, SocketInteractionContext context)
		{
			VoteLavalinkPlayer? player = await GetPlayerAsync(audioService, context, connectToVoiceChannel: false);
			if (player is null) return;

			if (player.State is not PlayerState.Paused)
			{
				await context.Interaction.RespondAsync("Player is not paused.").ConfigureAwait(false);
				return;
			}

			await player.ResumeAsync().ConfigureAwait(false);
			await context.Interaction.RespondAsync("Resumed.").ConfigureAwait(false);
		}

		/// <summary>
		/// Gets the guild player asynchronously.
		/// </summary>
		/// <param name="connectToVoiceChannel">A value indicating whether to connect to a voice channel</param>
		/// <returns>A task that represents the asynchronous operation. The task result is the lavalink player.</returns>
		private async ValueTask<VoteLavalinkPlayer?> GetPlayerAsync(IAudioService audioService, SocketInteractionContext context,  bool connectToVoiceChannel = true)
		{
			var retrieveOptions = new PlayerRetrieveOptions(
				ChannelBehavior: connectToVoiceChannel ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None);

			var result = await audioService.Players
				.RetrieveAsync(context, playerFactory: PlayerFactory.Vote, retrieveOptions)
				.ConfigureAwait(false);

			if (!result.IsSuccess)
			{
				var errorMessage = result.Status switch
				{
					PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel.",
					PlayerRetrieveStatus.BotNotConnected => "The bot is currently not connected.",
					_ => "Unknown error.",
				};

				await context.Interaction.FollowupAsync(errorMessage).ConfigureAwait(false);
				return null;
			}

			return result.Player;
		}

		private async ValueTask<VoteLavalinkPlayer?> GetPlayerAsync2(IAudioService audioService, CommandContext context, bool connectToVoiceChannel = true)
		{
			var retrieveOptions = new PlayerRetrieveOptions(
				ChannelBehavior: connectToVoiceChannel ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None);

			var result = await audioService.Players
				.RetrieveAsync(context, playerFactory: PlayerFactory.Vote, retrieveOptions)
				.ConfigureAwait(false);

			if (!result.IsSuccess)
			{
				var errorMessage = result.Status switch
				{
					PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel.",
					PlayerRetrieveStatus.BotNotConnected => "The bot is currently not connected.",
					_ => "Unknown error.",
				};

				await context.Channel.SendMessageAsync(errorMessage).ConfigureAwait(false);
				return null;
			}

			return result.Player;
		}
	}
}
