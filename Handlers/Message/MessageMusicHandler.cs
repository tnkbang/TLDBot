using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.Players;
using Lavalink4NET.DiscordNet;
using Discord.Rest;
using TLDBot.Structs;

namespace TLDBot.Handlers.Message
{
	public class MessageMusicHandler : MusicHandler
	{
		private readonly SocketUserMessage _userMessage;
		private readonly SocketCommandContext _commandContext;

		public MessageMusicHandler(IAudioService audioService, SocketUserMessage userMessage, SocketCommandContext context) : base(audioService)
		{
			_userMessage = userMessage;
			_commandContext = context;

			SetVoiceMembers();
		}

		private void SetVoiceMembers()
		{
			Bot = _commandContext.Guild.CurrentUser as SocketGuildUser;
			User = _commandContext.User as SocketGuildUser;
		}

		public Task PlayAsync(string query)
		{
			return base.PlayAsync(query, _userMessage.Author);
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

			RestUserMessage replyMessage = await _commandContext.Channel.SendMessageAsync(components: components, embed: embed).ConfigureAwait(false);
			GuildPlayer.Add(_commandContext.Guild.Id, new GuildPlayerMessage(_commandContext.Channel, replyMessage.Id, _playerResult.Player, _commandContext.User));
			return true;
		}

		protected override async Task SendMessageAsync(int wait = 0, string? text = null, bool ephemeral = false, MessageComponent? components = null, Embed? embed = null)
		{
			await _userMessage.DeleteAsync().ConfigureAwait(false);
			await SendMessageAsync(wait: wait, text: text, components: components, embed: embed).ConfigureAwait(false);
		}

		private async Task<RestUserMessage> SendMessageAsync(int wait = 0, string? text = null, MessageComponent? components = null, Embed? embed = null)
		{
			RestUserMessage userMessage = await _commandContext.Channel.SendMessageAsync(text: text, components: components, embed: embed).ConfigureAwait(false);
			if (wait is 0) return userMessage;

			await Task.Delay(TimeSpan.FromSeconds(wait)).ConfigureAwait(false);
			await userMessage.DeleteAsync().ConfigureAwait(false);
			return userMessage;
		}

		protected override async Task DeferAsync()
		{
			if (_commandContext is null || _userMessage is null) return;
			await _commandContext.Channel.TriggerTypingAsync().ConfigureAwait(false);
		}
	}
}
