using Discord.Interactions;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Rest;
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

		public SocketUser _currentUser
		{
			get
			{
				return _commandContext is not null ? _commandContext.User : _interactionContext!.User;
			}
		}

		/// <summary>
		/// Disconnects from the current voice channel connected to asynchronously.
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		public async Task DisconnectAsync()
		{
			await DeferAsync().ConfigureAwait(false);

			VoteLavalinkPlayer? player = await GetPlayerAsync().ConfigureAwait(false);
			if (player is null) return;

			await player.DisconnectAsync().ConfigureAwait(false);
			await FollowupAsync(message: "Disconnect.").ConfigureAwait(false);
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
				await FollowupAsync(title: "Search Playing", message: "No results.").ConfigureAwait(false);
				return;
			}

			var position = await player.PlayAsync(track).ConfigureAwait(false);

			if (position is 0)
			{
				await FollowupAsync(components: Helper.CreateButtonsMusicPlaying(isPause: false), embed: UtilEmbed.Playing(player, track, _interactionContext!.User), isPlaying: true, isUpdateEmbed: true).ConfigureAwait(false);
			}
			else
			{
				await FollowupAsync(title: "Playing", message: $"Added to queue: **{track.Title}**", isUpdateEmbed: true).ConfigureAwait(false);
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
			VoteLavalinkPlayer? player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);
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

			VoteLavalinkPlayer? player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);
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
			VoteLavalinkPlayer? player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);
			if (player is null) return;

			if (player.CurrentItem is null)
			{
				await RespondAsync(message: "Nothing playing!").ConfigureAwait(false);
				return;
			}

			await player.SkipAsync().ConfigureAwait(false);

			var track = player.CurrentItem;

			if (track is not null)
			{
				await RespondAsync(title: "Skipped", message: $"Now playing: **{track.Track!.Title}**").ConfigureAwait(false);
			}
			else
			{
				await RespondAsync(title: "Skipped", message: "Stopped playing because the queue is now empty.").ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Loop or unloop the current track and queue.
		/// </summary>
		/// <returns>A task that represents the asynchronous operation</returns>
		public async Task LoopAsync()
		{
			VoteLavalinkPlayer? player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);
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
			VoteLavalinkPlayer? player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);
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
			VoteLavalinkPlayer? player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);
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
			VoteLavalinkPlayer? player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);
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
			VoteLavalinkPlayer? player = await GetPlayerAsync(connectToVoiceChannel: false).ConfigureAwait(false);
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

				if(_interactionContext is not null) await FollowupAsync(errorMessage).ConfigureAwait(false);
				else await RespondAsync(errorMessage).ConfigureAwait(false);
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

		private async Task FollowupAsync(string? title = null, string? message = null, MessageComponent? components = null, Embed? embed = null, bool isPlaying = false, bool isUpdateEmbed = false)
		{
			if(_interactionContext is not null)
			{
				RestFollowupMessage followupMessage;
				if (isPlaying)
				{
					GuildPlayerMessage? playerMessage;
					Helper.GuildPlayer.TryGetValue(_playerResult.Player!.GuildId, out playerMessage);

					if (playerMessage is null)
					{
						followupMessage = await _interactionContext.Interaction.FollowupAsync(message, components: components, embed: embed).ConfigureAwait(false);
						Helper.GuildPlayer.Add(_interactionContext.Guild.Id, new GuildPlayerMessage(_interactionContext.Channel, followupMessage.Id, _playerResult.Player!, _interactionContext.User));
						return;
					}
				}

				if (isUpdateEmbed)
				{
					await Helper.UpdatePlayingAsync(_playerResult.Player!, _playerResult.Player!.CurrentTrack!, isUpdateEmbed: isUpdateEmbed, isUpdateComponent: true).ConfigureAwait(false);
				}

				followupMessage = await _interactionContext.Interaction
					.FollowupAsync(embed: UtilEmbed.Info(title, isPlaying ? "Playing track: **" + _playerResult.Player!.CurrentTrack!.Title + "**" : message)).ConfigureAwait(false);

				await Task.Delay(TimeSpan.FromSeconds(Helper.SECOND_WAIT)).ConfigureAwait(false);
				await followupMessage.DeleteAsync().ConfigureAwait(false);
			}
		}
		
		private async Task RespondAsync(string? title = null, string? message = null, bool isUpdateEmbed = false, bool isUpdateComponent = false)
		{
			if (isUpdateEmbed || isUpdateComponent)
			{
				await Helper.UpdatePlayingAsync(_playerResult.Player!, _playerResult.Player!.CurrentTrack!, isUpdateEmbed, isUpdateComponent).ConfigureAwait(false);
			}

			if (_interactionContext is not null)
			{
				await _interactionContext.Interaction.RespondAsync(embed: UtilEmbed.Info(title, message)).ConfigureAwait(false);

				await Task.Delay(TimeSpan.FromSeconds(Helper.SECOND_WAIT)).ConfigureAwait(false);
				await _interactionContext.Interaction.DeleteOriginalResponseAsync().ConfigureAwait(false);
			}

			if(_messageComponent is not null)
			{
				await _messageComponent.RespondAsync(embed: UtilEmbed.Info(title, message)).ConfigureAwait(false);

				await Task.Delay(TimeSpan.FromSeconds(Helper.SECOND_WAIT)).ConfigureAwait(false);
				await _messageComponent.DeleteOriginalResponseAsync().ConfigureAwait(false);
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
