using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.Players;
using Lavalink4NET.DiscordNet;
using Discord.Rest;
using TLDBot.Structs;
using TLDBot.Utility;

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
		}

		public Task PlayAsync(string query)
		{
			return base.PlayAsync(query, _userMessage.Author);
		}

		private async Task<bool> SetGuildPlayer(string? message = null, MessageComponent? components = null, Embed? embed = null)
		{
			if (_playerResult.Player is null) return false;

			GuildPlayerMessage? playerMessage;
			GuildPlayer.TryGetValue(_playerResult.Player.GuildId, out playerMessage);
			if (playerMessage is not null) return false;

			RestUserMessage replyMessage = await _commandContext.Channel.SendMessageAsync(message, components: components, embed: embed).ConfigureAwait(false);
			GuildPlayer.Add(_commandContext.Guild.Id, new GuildPlayerMessage(_commandContext.Channel, replyMessage.Id, _playerResult.Player, _commandContext.User));
			return true;
		}

		protected override async Task FollowupAsync(string? title = null, string? message = null, MessageComponent? components = null, Embed? embed = null, bool isPlaying = false, bool isUpdateEmbed = false)
		{
			if (_commandContext is null) return;
			if (_playerResult.Player is null || _playerResult.Player.CurrentTrack is null) return;

			await _commandContext.Message.DeleteAsync().ConfigureAwait(false);
			if (isPlaying && await SetGuildPlayer(message, components, embed)) return;

			if (isUpdateEmbed)
			{
				await Helper.UpdatePlayingAsync(_playerResult.Player, _playerResult.Player.CurrentTrack, isUpdateEmbed: isUpdateEmbed, isUpdateComponent: true).ConfigureAwait(false);
			}

			RestUserMessage replyMessage = await _commandContext.Channel
				.SendMessageAsync(embed: Embeds.Info(title, isPlaying ? "Playing track: **" + _playerResult.Player.CurrentTrack.Title + "**" : message)).ConfigureAwait(false);

			await Task.Delay(TimeSpan.FromSeconds(SECOND_WAIT)).ConfigureAwait(false);
			await replyMessage.DeleteAsync().ConfigureAwait(false);
		}

		protected override async Task FollowupAsync(MessageComponent component)
		{
			await _commandContext.Channel.SendMessageAsync(components: component).ConfigureAwait(false);
			await _userMessage.DeleteAsync().ConfigureAwait(false);
		}

		protected override async Task SetPlayerAsync(PlayerRetrieveOptions retrieveOptions)
		{
			if (_commandContext is null) return;
			_playerResult = await _audioService.Players.RetrieveAsync(_commandContext, playerFactory: PlayerFactory.Vote, retrieveOptions).ConfigureAwait(false);
		}

		protected override async Task RespondAsync(int wait, Embed? embed = null, MessageComponent? components = null)
		{
			if (embed is null && components is null) return;
			if (_userMessage is null) return;

			await _userMessage.DeleteAsync().ConfigureAwait(false);
			RestUserMessage reply = await _userMessage.Channel.SendMessageAsync(embed: embed, components: components).ConfigureAwait(false);

			await Task.Delay(TimeSpan.FromSeconds(wait)).ConfigureAwait(false);
			await reply.DeleteAsync().ConfigureAwait(false);
		}
	}
}
