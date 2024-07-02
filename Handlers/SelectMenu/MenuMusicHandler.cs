using Discord.Commands;
using Lavalink4NET.DiscordNet;
using Lavalink4NET;
using Lavalink4NET.Players;
using Discord;
using Discord.Rest;
using TLDBot.Utility;
using TLDBot.Structs;

namespace TLDBot.Handlers.SelectMenu
{
	public class MenuMusicHandler : MusicHandler
	{
		private readonly SocketCommandContext _context;

		public MenuMusicHandler(IAudioService audioService, SocketCommandContext context) : base(audioService)
		{
			_context = context;
		}

		public async Task SearchAsync(IReadOnlyCollection<string> collection)
		{
			if (collection is null) return;

			await _context.Message.DeleteAsync().ConfigureAwait(false);
			await SearchAsync(collection, _context.User).ConfigureAwait(false);
		}

		protected override async Task SetPlayerAsync(PlayerRetrieveOptions retrieveOptions)
		{
			if (_context is null) return;
			_playerResult = await _audioService.Players.RetrieveAsync(_context, playerFactory: PlayerFactory.Vote, retrieveOptions).ConfigureAwait(false);
		}

		private async Task<bool> SetGuildPlayer(string? message = null, MessageComponent? components = null, Embed? embed = null)
		{
			GuildPlayerMessage? playerMessage;
			GuildPlayer.TryGetValue(_playerResult.Player!.GuildId, out playerMessage);
			if (playerMessage is not null) return false;

			RestUserMessage replyMessage = await _context.Channel.SendMessageAsync(message, components: components, embed: embed).ConfigureAwait(false);
			GuildPlayer.Add(_context.Guild.Id, new GuildPlayerMessage(_context.Channel, replyMessage.Id, _playerResult.Player, _context.User));
			return true;
		}

		protected override async Task FollowupAsync(string? title = null, string? message = null, MessageComponent? components = null, Embed? embed = null, bool isPlaying = false, bool isUpdateEmbed = false)
		{
			if (_context is null) return;
			if (isPlaying && await SetGuildPlayer(message, components, embed)) return;

			if (isUpdateEmbed)
			{
				await Helper.UpdatePlayingAsync(_playerResult.Player!, _playerResult.Player!.CurrentTrack!, isUpdateEmbed: isUpdateEmbed, isUpdateComponent: true).ConfigureAwait(false);
			}

			RestUserMessage replyMessage = await _context.Channel
				.SendMessageAsync(embed: Embeds.Info(title, isPlaying ? "Playing track: **" + _playerResult.Player!.CurrentTrack!.Title + "**" : message)).ConfigureAwait(false);

			await Task.Delay(TimeSpan.FromSeconds(SECOND_WAIT)).ConfigureAwait(false);
			await replyMessage.DeleteAsync().ConfigureAwait(false);
		}
	}
}
