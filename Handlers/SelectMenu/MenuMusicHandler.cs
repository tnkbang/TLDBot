using Discord.Commands;
using Lavalink4NET.DiscordNet;
using Lavalink4NET;
using Lavalink4NET.Players;
using Discord;
using Discord.Rest;
using TLDBot.Structs;
using Discord.WebSocket;

namespace TLDBot.Handlers.SelectMenu
{
	public class MenuMusicHandler : MusicHandler
	{
		private readonly SocketMessageComponent _messageComponent;
		private readonly SocketCommandContext _commandContext;

		public MenuMusicHandler(IAudioService audioService, SocketMessageComponent messageComponent, SocketCommandContext context) : base(audioService)
		{
			_messageComponent = messageComponent;
			_commandContext = context;

			SetVoiceMembers();
		}

		private void SetVoiceMembers()
		{
			Bot = _commandContext.Guild.CurrentUser as SocketGuildUser;
			User = _messageComponent.User as SocketGuildUser;
		}

		public async Task SearchAsync(IReadOnlyCollection<string> collection)
		{
			if (collection is null) return;

			await _commandContext.Message.DeleteAsync().ConfigureAwait(false);
			await SearchAsync(collection, _messageComponent.User).ConfigureAwait(false);
		}

		protected override async Task SetPlayerAsync(PlayerRetrieveOptions retrieveOptions)
		{
			if (_commandContext is null) return;
			_playerResult = await _audioService.Players.RetrieveAsync(_commandContext, playerFactory: PlayerFactory.Vote, retrieveOptions).ConfigureAwait(false);
		}

		protected override async Task<bool> SetGuildPlayer(MessageComponent? components = null, Embed? embed = null)
		{
			if (_playerResult.Player is null) return false;

			GuildPlayerMessage? playerMessage;
			GuildPlayer.TryGetValue(_playerResult.Player.GuildId, out playerMessage);
			if (playerMessage is not null) return false;

			RestFollowupMessage followupMessage = await FollowupAsync(components: components, embed: embed).ConfigureAwait(false);
			GuildPlayer.Add(_commandContext.Guild.Id, new GuildPlayerMessage(_commandContext.Channel, followupMessage.Id, _playerResult.Player, _messageComponent.User));
			return true;
		}

		protected override async Task SendMessageAsync(int wait = 0, string? text = null, bool ephemeral = false, MessageComponent? components = null, Embed? embed = null)
		{
			if (_messageComponent.HasResponded)
			{
				await FollowupAsync(wait, text, ephemeral, components, embed).ConfigureAwait(false);
				return;
			}

			await RespondAsync(wait, text, ephemeral, components, embed).ConfigureAwait(false);
		}

		private async Task<RestFollowupMessage> FollowupAsync(int wait = 0, string? text = null, bool ephemeral = false, MessageComponent? components = null, Embed? embed = null)
		{
			RestFollowupMessage followupMessage = await _messageComponent.FollowupAsync(text: text, ephemeral: ephemeral, components: components, embed: embed).ConfigureAwait(false);
			if (wait is 0) return followupMessage;

			await Task.Delay(TimeSpan.FromSeconds(wait)).ConfigureAwait(false);
			await _messageComponent.DeleteOriginalResponseAsync().ConfigureAwait(false);
			return followupMessage;
		}

		private async Task RespondAsync(int wait = 0, string? text = null, bool ephemeral = false, MessageComponent? components = null, Embed? embed = null)
		{
			await _messageComponent.RespondAsync(text: text, ephemeral: ephemeral, components: components, embed: embed).ConfigureAwait(false);
			if (wait is 0) return;

			await Task.Delay(TimeSpan.FromSeconds(wait)).ConfigureAwait(false);
			await _messageComponent.DeleteOriginalResponseAsync().ConfigureAwait(false);
		}

		protected override async Task DeferAsync()
		{
			if (_messageComponent is null || _commandContext is null) return;
			await _messageComponent.DeferAsync().ConfigureAwait(false);
		}
	}
}
