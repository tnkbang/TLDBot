using Discord.Commands;
using Lavalink4NET.DiscordNet;
using Lavalink4NET;
using Lavalink4NET.Players;
using Discord;
using Discord.Rest;
using TLDBot.Utility;
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

		private async Task<bool> SetGuildPlayer(string? message = null, MessageComponent? components = null, Embed? embed = null)
		{
			if (_playerResult.Player is null) return false;

			GuildPlayerMessage? playerMessage;
			GuildPlayer.TryGetValue(_playerResult.Player.GuildId, out playerMessage);
			if (playerMessage is not null) return false;

			RestUserMessage replyMessage = await _commandContext.Channel.SendMessageAsync(message, components: components, embed: embed).ConfigureAwait(false);
			GuildPlayer.Add(_commandContext.Guild.Id, new GuildPlayerMessage(_commandContext.Channel, replyMessage.Id, _playerResult.Player, _messageComponent.User));
			return true;
		}

		protected override async Task FollowupAsync(string? title = null, string? message = null, MessageComponent? components = null, Embed? embed = null, bool isPlaying = false, bool isUpdateEmbed = false)
		{
			if (_commandContext is null) return;
			if (_playerResult.Player is null || _playerResult.Player.CurrentTrack is null) return;
			if (isPlaying && await SetGuildPlayer(message, components, embed)) return;

			if (isUpdateEmbed)
			{
				await Helper.UpdatePlayingAsync(_playerResult.Player, _playerResult.Player.CurrentTrack, isUpdateEmbed: isUpdateEmbed, isUpdateComponent: true).ConfigureAwait(false);
			}

			RestUserMessage replyMessage = await _commandContext.Channel
				.SendMessageAsync(embed: Embeds.Info(title, isPlaying ? Description.Play.GetBody(_playerResult.Player.CurrentTrack.Title) : message)).ConfigureAwait(false);

			await Task.Delay(TimeSpan.FromSeconds(SECOND_WAIT)).ConfigureAwait(false);
			await replyMessage.DeleteAsync().ConfigureAwait(false);
		}
	}
}
