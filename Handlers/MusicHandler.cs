using Discord.Interactions;
using Discord.WebSocket;
using Discord.Commands;
using Discord;
using Lavalink4NET;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Vote;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Rest.Entities.Tracks;
using Lavalink4NET.DiscordNet;
using TLDBot.Utility;

namespace TLDBot.Handlers
{
	public class MusicHandler
	{
		private PlayerResult<VoteLavalinkPlayer> _playerResult;
		private readonly IAudioService _audioService;
		private readonly SocketInteractionContext? _interactionContext = null;
		private readonly SocketMessageComponent? _messageComponent = null;
		private readonly SocketCommandContext? _commandContext = null;

		public MusicHandler(IAudioService audioService, SocketInteractionContext? interactionContext = null, SocketMessageComponent? messageComponent = null, SocketCommandContext? commandContext = null)
		{
			_audioService = audioService;
			_interactionContext = interactionContext;
			_messageComponent = messageComponent;
			_commandContext = commandContext;
		}

		/// <summary>
		/// Disconnects from the current voice channel connected to asynchronously.
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		public async Task DisconnectAsync()
		{
			VoteLavalinkPlayer? player = await GetPlayerAsync().ConfigureAwait(false);
			if (player is null) return;

			await player.DisconnectAsync().ConfigureAwait(false);
			await RespondAsync("Disconnect.").ConfigureAwait(false);
		}

		/// <summary>
		/// Plays music asynchronously.
		/// </summary>
		/// <param name="query">The search query</param>
		/// <returns>A task that represents the asynchronous operation</returns>
		public async Task PlayAsync(string query)
		{
			await DeferAsync().ConfigureAwait(false);

			VoteLavalinkPlayer? player = await GetPlayerAsync(connectToVoiceChannel: true).ConfigureAwait(false);
			if (player is null) return;

			var track = await _audioService.Tracks.LoadTrackAsync(query, TrackSearchMode.YouTube).ConfigureAwait(false);

			if (track is null)
			{
				await FollowupAsync("😖 No results.").ConfigureAwait(false);
				return;
			}

			var position = await player.PlayAsync(track).ConfigureAwait(false);

			if (position is 0)
			{
				await FollowupAsync($"🔈 Playing: {track.Uri}", components: Helper.CreateButtonsMusicPlaying(isPause: false)).ConfigureAwait(false);
			}
			else
			{
				await FollowupAsync($"🔈 Added to queue: {track.Uri}").ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Shows the track position asynchronously.
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		public async Task PositionAsync()
		{
			VoteLavalinkPlayer? player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);
			if (player is null) return;

			if (player.CurrentItem is null)
			{
				await RespondAsync("Nothing playing!").ConfigureAwait(false);
				return;
			}

			await RespondAsync($"Position: {player.Position?.Position} / {player.CurrentTrack?.Duration}.").ConfigureAwait(false);
		}

		/// <summary>
		/// Stops the current track asynchronously.
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		public async Task StopAsync()
		{
			VoteLavalinkPlayer? player = await GetPlayerAsync(connectToVoiceChannel: false);
			if (player is null) return;

			if (player.CurrentItem is null)
			{
				await RespondAsync("Nothing playing!").ConfigureAwait(false);
				return;
			}

			await player.StopAsync().ConfigureAwait(false);
			await RespondAsync("Stopped playing.").ConfigureAwait(false);
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
				await RespondAsync("Volume out of range: 0% - 1000%!").ConfigureAwait(false);
				return;
			}

			VoteLavalinkPlayer? player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);
			if (player is null) return;

			await player.SetVolumeAsync(volume / 100f).ConfigureAwait(false);
			await RespondAsync($"Volume updated: {volume}%").ConfigureAwait(false);
		}

		/// <summary>
		/// Skip to next songs in queues
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		public async Task SkipAsync()
		{
			VoteLavalinkPlayer? player = await GetPlayerAsync(connectToVoiceChannel: false);
			if (player is null) return;

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

		/// <summary>
		/// Loop or unloop the current track and queue.
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		public async Task LoopAsync()
		{
			VoteLavalinkPlayer? player = await GetPlayerAsync(connectToVoiceChannel: false);
			if (player is null) return;

			switch (player.RepeatMode)
			{
				case TrackRepeatMode.None:
					player.RepeatMode = TrackRepeatMode.Track;
					await RespondAsync("Loop the current track.").ConfigureAwait(false);
					break;
				case TrackRepeatMode.Track:
					player.RepeatMode = TrackRepeatMode.Queue;
					await RespondAsync("Loop the current queue.").ConfigureAwait(false);
					break;
				case TrackRepeatMode.Queue:
					player.RepeatMode = TrackRepeatMode.None;
					await RespondAsync("Unloop the current queue.").ConfigureAwait(false);
					break;
			}
		}

		/// <summary>
		/// Pause song in playing
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		public async Task PauseAsync()
		{
			VoteLavalinkPlayer? player = await GetPlayerAsync(connectToVoiceChannel: false);
			if (player is null) return;

			if (player.State is PlayerState.Paused)
			{
				await RespondAsync("Player is already paused.").ConfigureAwait(false);
				return;
			}

			await player.PauseAsync().ConfigureAwait(false);

			if (_messageComponent is not null) await _messageComponent.UpdateAsync(msg => msg.Components = Helper.CreateButtonsMusicPlaying(isPause: true)).ConfigureAwait(false);
			else await RespondAsync("Paused.").ConfigureAwait(false);
		}

		/// <summary>
		/// Resume song in playing
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		public async Task ResumeAsync()
		{
			VoteLavalinkPlayer? player = await GetPlayerAsync(connectToVoiceChannel: false);
			if (player is null) return;

			if (player.State is not PlayerState.Paused)
			{
				await RespondAsync("Player is not paused.").ConfigureAwait(false);
				return;
			}

			await player.ResumeAsync().ConfigureAwait(false);

			if (_messageComponent is not null) await _messageComponent.UpdateAsync(msg => msg.Components = Helper.CreateButtonsMusicPlaying(isPause: false)).ConfigureAwait(false);
			else await RespondAsync("Resumed.").ConfigureAwait(false);
		}

		/// <summary>
		/// Gets the guild player asynchronously.
		/// </summary>
		/// <param name="connectToVoiceChannel">A value indicating whether to connect to a voice channel</param>
		/// <returns>A task that represents the asynchronous operation. The task result is the lavalink player.</returns>
		private async ValueTask<VoteLavalinkPlayer?> GetPlayerAsync(bool connectToVoiceChannel = true)
		{
			if (_interactionContext is null && _commandContext is null) return null;

			var retrieveOptions = new PlayerRetrieveOptions(
				ChannelBehavior: connectToVoiceChannel ? PlayerChannelBehavior.Join : PlayerChannelBehavior.None);

			await SetPlayerAsync(retrieveOptions).ConfigureAwait(false);

			if (!_playerResult.IsSuccess)
			{
				var errorMessage = _playerResult.Status switch
				{
					PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel.",
					PlayerRetrieveStatus.BotNotConnected => "The bot is currently not connected.",
					_ => "Unknown error.",
				};

				await FollowupAsync(errorMessage).ConfigureAwait(false);
				return null;
			}

			return _playerResult.Player;
		}

		private async Task SetPlayerAsync(PlayerRetrieveOptions retrieveOptions)
		{
			if (_interactionContext is not null)
			{
				_playerResult = await _audioService.Players.RetrieveAsync(_interactionContext, playerFactory: PlayerFactory.Vote, retrieveOptions).ConfigureAwait(false);
			}

			if (_commandContext is not null)
			{
				_playerResult = await _audioService.Players.RetrieveAsync(_commandContext, playerFactory: PlayerFactory.Vote, retrieveOptions).ConfigureAwait(false);
			}
		}

		private async Task FollowupAsync(string message, MessageComponent? components = null)
		{
			if(_interactionContext is not null)
			{
				await _interactionContext.Interaction.FollowupAsync(message, components: components).ConfigureAwait(false);
			}

			if(_messageComponent is not null)
			{
				await _messageComponent.RespondAsync(message).ConfigureAwait(false);
			}
		}
		
		private async Task RespondAsync(string message)
		{
			if(_interactionContext is not null)
			{
				await _interactionContext.Interaction.RespondAsync(message).ConfigureAwait(false);
			}

			if(_messageComponent is not null)
			{
				await _messageComponent.RespondAsync(message).ConfigureAwait(false);
			}
		}
		
		private async Task DeferAsync()
		{
			if(_interactionContext is not null)
			{
				await _interactionContext.Interaction.DeferAsync().ConfigureAwait(false);
			}

			if(_messageComponent is not null)
			{
				await _messageComponent.DeferAsync().ConfigureAwait(false);
			}
		}
	}
}
