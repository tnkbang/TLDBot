using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using TLDBot.Utility;

namespace TLDBot.Handlers.Message
{
	public class MessageT3Handler : TicTacToeHandler
	{
		private readonly SocketCommandContext _commandContext;
		private readonly SocketUserMessage _userMessage;

		public MessageT3Handler(SocketCommandContext context) : base(context.Message.Author)
		{
			_commandContext = context;
			_userMessage = context.Message;
		}

		private async Task<bool> IsMentionCorrect(SocketUser user)
		{
			if (user.IsBot)
			{
				await _userMessage.ReplyAsync(Description.Permission.NotMentionBot).ConfigureAwait(false);
				return false;
			}
			if (user.Id.Equals(_userMessage.Author.Id))
			{
				await _userMessage.ReplyAsync(Description.Permission.NotMention).ConfigureAwait(false);
				return false;
			}

			return true;
		}

		private async Task<bool> HasAlreadyGame()
		{
			if (_player.MessageId is 0) return false;

			IUserMessage reply = await _userMessage.ReplyAsync(Description.State.Already).ConfigureAwait(false);
			await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);

			await _userMessage.DeleteAsync().ConfigureAwait(false);
			await reply.DeleteAsync().ConfigureAwait(false);

			return true;
		}

		private async Task<RestUserMessage> RespondDuetAsync(SocketUser user)
		{
			string duet = _userMessage.Author.Mention + ": " + GetStringDuetChoice(_userMessage.Author.Id) + Environment.NewLine;
			duet += user.Mention + ": " + GetStringDuetChoice(user.Id) + Environment.NewLine;
			return await _userMessage.Channel.SendMessageAsync(
				embed: Embeds.T3StartDuet(_userMessage.Author, DescriptionDuet, duet, IsSetTitle), components: ComponentChooseXO).ConfigureAwait(false);
		}

		private SocketUser? GetUserMention()
		{
			foreach(SocketUser user in _userMessage.MentionedUsers)
			{
				if (user.Id == _commandContext.Client.CurrentUser.Id) continue;
				return user;
			}
			return null;
		}

		public override async Task RespondAsync()
		{
			if (_userMessage is null) return;
			if (await HasAlreadyGame()) return;

			SocketUser? user = GetUserMention();
			SetMode(user);

			RestUserMessage message;
			if(user is not null)
			{
				if (await IsMentionCorrect(user) is false) return;
				message = await RespondDuetAsync(user).ConfigureAwait(false);
			}
			else
			{
				message = await _userMessage.Channel.SendMessageAsync(
					embed: Embeds.T3Start(_userMessage.Author, DescriptionWithBot, IsSetTitle), components: ComponentChooseXO).ConfigureAwait(false);
			}

			SetMessageBoard(message.Id);
		}
	}
}
