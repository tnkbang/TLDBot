using Discord.Commands;
using Discord.WebSocket;
using Discord;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Players;
using Lavalink4NET;

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
		}

		protected override async Task SetPlayerAsync(PlayerRetrieveOptions retrieveOptions)
		{
			if (_commandContext is null) return;
			await DeferAsync().ConfigureAwait(false);
			_playerResult = await _audioService.Players.RetrieveAsync(_commandContext, playerFactory: PlayerFactory.Vote, retrieveOptions).ConfigureAwait(false);
		}

		protected override async Task RespondAsync(int wait, Embed? embed = null, MessageComponent? components = null)
		{
			if (embed is null && components is null) return;
			if (_messageComponent is null) return;

			await _messageComponent.FollowupAsync(embed: embed, components: components).ConfigureAwait(false);

			await Task.Delay(TimeSpan.FromSeconds(SECOND_WAIT)).ConfigureAwait(false);
			await _messageComponent.DeleteOriginalResponseAsync().ConfigureAwait(false);
		}

		protected override async Task DeferAsync()
		{
			if (_messageComponent is null) return;
			await _messageComponent.DeferLoadingAsync().ConfigureAwait(false);
		}
	}
}