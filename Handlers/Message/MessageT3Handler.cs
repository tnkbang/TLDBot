using Discord.Rest;
using Discord.WebSocket;
using TLDBot.Utility;

namespace TLDBot.Handlers.Message
{
	public class MessageT3Handler : TicTacToeHandler
	{
		private readonly SocketUserMessage _userMessage;

		public MessageT3Handler(SocketUserMessage userMessage) : base(userMessage.Author)
		{
			_userMessage = userMessage;
		}

		private async Task<bool> IsMentionCorrect(SocketUser user)
		{
			if (user.IsBot)
			{
				await _userMessage.Channel.SendMessageAsync(Description.Permission.NotMentionBot).ConfigureAwait(false);
				return false;
			}
			if (user.Id.Equals(_userMessage.Author.Id))
			{
				await _userMessage.Channel.SendMessageAsync(Description.Permission.NotMention).ConfigureAwait(false);
				return false;
			}

			return true;
		}

		private async Task<RestUserMessage> RespondDuetAsync(SocketUser user)
		{
			string duet = _userMessage.Author.Mention + ": " + GetStringDuetChoice(_userMessage.Author.Id) + Environment.NewLine;
			duet += user.Mention + ": " + GetStringDuetChoice(user.Id) + Environment.NewLine;
			return await _userMessage.Channel.SendMessageAsync(
				embed: Embeds.T3StartDuet(_userMessage.Author, DescriptionDuet, duet), components: ComponentChooseXO).ConfigureAwait(false);
		}

		public override async Task RespondAsync()
		{
			if (_userMessage is null) return;
			SocketUser? user = _userMessage.MentionedUsers.FirstOrDefault();
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
					embed: Embeds.T3Start(_userMessage.Author, DescriptionWithBot), components: ComponentChooseXO).ConfigureAwait(false);
			}

			SetMessageBoard(message.Id);
		}
	}
}
