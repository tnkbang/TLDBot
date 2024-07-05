using Discord.Commands;
using Discord.WebSocket;
using Discord;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Players;
using Lavalink4NET;
using Discord.Rest;

namespace TLDBot.Handlers.Button
{
	public class ButtonMusicHandler : MusicHandler
	{
		private readonly SocketMessageComponent _messageComponent;
		private readonly SocketCommandContext _commandContext;

		public ButtonMusicHandler(IAudioService audioService, SocketMessageComponent messageComponent, SocketCommandContext commandContext) : base(audioService)
		{
			_messageComponent = messageComponent;
			_commandContext = commandContext;

			SetVoiceMembers();
		}

		private void SetVoiceMembers()
		{
			Bot = _commandContext.Guild.CurrentUser as SocketGuildUser;
			User = _messageComponent.User as SocketGuildUser;
		}

		protected override async Task SetPlayerAsync(PlayerRetrieveOptions retrieveOptions)
		{
			if (_commandContext is null) return;
			_playerResult = await _audioService.Players.RetrieveAsync(_commandContext, playerFactory: PlayerFactory.Vote, retrieveOptions).ConfigureAwait(false);
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