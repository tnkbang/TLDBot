using Discord.Interactions;
using Discord.Rest;
using Discord;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Players;
using TLDBot.Structs;
using Discord.WebSocket;

namespace TLDBot.Handlers.Slash
{
	public class SlashMusicHandler : MusicHandler
	{
		private readonly SocketInteractionContext _interactionContext;

		public SlashMusicHandler(IAudioService audioService, SocketInteractionContext interactionContext) : base(audioService)
		{
			_interactionContext = interactionContext;

			SetVoiceMembers();
		}

		private void SetVoiceMembers()
		{
			Bot = _interactionContext.Guild.CurrentUser as SocketGuildUser;
			User = _interactionContext.User as SocketGuildUser;
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

		protected override async Task<bool> SetGuildPlayer(MessageComponent? components = null, Embed? embed = null)
		{
			if (_playerResult.Player is null) return false;

			GuildPlayerMessage? playerMessage;
			GuildPlayer.TryGetValue(_playerResult.Player.GuildId, out playerMessage);
			if (playerMessage is not null) return false;
			
			RestFollowupMessage followupMessage = await FollowupAsync(components: components, embed: embed).ConfigureAwait(false);
			GuildPlayer.Add(_interactionContext.Guild.Id, new GuildPlayerMessage(_interactionContext.Channel, followupMessage.Id, _playerResult.Player, _interactionContext.User));
			return true;
		}

		protected override async Task SendMessageAsync(int wait = 0, string? text = null, bool ephemeral = false, MessageComponent? components = null, Embed? embed = null)
		{
			if (_interactionContext.Interaction.HasResponded)
			{
				await FollowupAsync(wait, text, ephemeral, components, embed).ConfigureAwait(false);
				return;
			}

			await RespondAsync(wait, text, ephemeral, components, embed).ConfigureAwait(false);
		}

		private async Task<RestFollowupMessage> FollowupAsync(int wait = 0, string? text = null, bool ephemeral = false, MessageComponent? components = null, Embed? embed = null)
		{
			RestFollowupMessage followupMessage = await _interactionContext.Interaction.FollowupAsync(text: text, ephemeral: ephemeral, components: components, embed: embed).ConfigureAwait(false);
			if(wait is 0) return followupMessage;

			await Task.Delay(TimeSpan.FromSeconds(wait)).ConfigureAwait(false);
			await followupMessage.DeleteAsync().ConfigureAwait(false);
			return followupMessage;
		}

		private async Task RespondAsync(int wait = 0, string? text = null, bool ephemeral = false, MessageComponent? components = null, Embed? embed = null)
		{
			await _interactionContext.Interaction.RespondAsync(text: text, ephemeral: ephemeral, components: components, embed: embed).ConfigureAwait(false);
			if (wait is 0) return;

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