using Discord.Interactions;
using Discord.Rest;
using Discord;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Players;
using TLDBot.Structs;
using TLDBot.Utility;

namespace TLDBot.Handlers.Slash
{
	public class SlashMusicHandler : MusicHandler
	{
		private readonly SocketInteractionContext _interactionContext;

		public SlashMusicHandler(IAudioService audioService, SocketInteractionContext interactionContext) : base(audioService)
		{
			_interactionContext = interactionContext;
		}

		public Task PlayAsync(string query)
		{
			return base.PlayAsync(query, _interactionContext.User);
		}

		protected override async Task SetPlayerAsync(PlayerRetrieveOptions retrieveOptions)
		{
			if (_interactionContext is null) return;
			_playerResult = await _audioService.Players.RetrieveAsync(_interactionContext, playerFactory: PlayerFactory.Vote, retrieveOptions).ConfigureAwait(false);
		}

		private async Task<bool> SetGuildPlayer(string? message = null, MessageComponent? components = null, Embed? embed = null)
		{
			if (_playerResult.Player is null) return false;

			GuildPlayerMessage? playerMessage;
			GuildPlayer.TryGetValue(_playerResult.Player.GuildId, out playerMessage);
			if (playerMessage is not null) return false;
			
			RestFollowupMessage followupMessage = await _interactionContext.Interaction.FollowupAsync(message, components: components, embed: embed).ConfigureAwait(false);
			GuildPlayer.Add(_interactionContext.Guild.Id, new GuildPlayerMessage(_interactionContext.Channel, followupMessage.Id, _playerResult.Player, _interactionContext.User));
			return true;
		}

		protected override async Task FollowupAsync(string? title = null, string? message = null, MessageComponent? components = null, Embed? embed = null, bool isPlaying = false, bool isUpdateEmbed = false)
		{
			if (_interactionContext is null) return;
			if (_playerResult.Player is null || _playerResult.Player.CurrentTrack is null) return;
			if (isPlaying && await SetGuildPlayer(message, components, embed)) return;

			if (isUpdateEmbed)
			{
				await Helper.UpdatePlayingAsync(_playerResult.Player, _playerResult.Player.CurrentTrack, isUpdateEmbed: isUpdateEmbed, isUpdateComponent: true).ConfigureAwait(false);
			}

			RestFollowupMessage followupMessage = await _interactionContext.Interaction
				.FollowupAsync(embed: Embeds.Info(title, isPlaying ? "Playing track: **" + _playerResult.Player.CurrentTrack.Title + "**" : message)).ConfigureAwait(false);

			await Task.Delay(TimeSpan.FromSeconds(SECOND_WAIT)).ConfigureAwait(false);
			await followupMessage.DeleteAsync().ConfigureAwait(false);
		}

		protected override async Task FollowupAsync(MessageComponent component)
		{
			await _interactionContext.Interaction.FollowupAsync(components: component).ConfigureAwait(false);
		}

		protected override async Task RespondAsync(int wait, Embed? embed = null, MessageComponent? components = null)
		{
			if (embed is null && components is null) return;
			if (_interactionContext is null) return;

			await _interactionContext.Interaction.RespondAsync(embed: embed, components: components).ConfigureAwait(false);

			await Task.Delay(TimeSpan.FromSeconds(wait)).ConfigureAwait(false);
			await _interactionContext.Interaction.DeleteOriginalResponseAsync().ConfigureAwait(false);
		}

		protected override async Task DeferAsync()
		{
			if (_interactionContext is null) return;
			await _interactionContext.Interaction.DeferAsync().ConfigureAwait(false);
		}
	}
}