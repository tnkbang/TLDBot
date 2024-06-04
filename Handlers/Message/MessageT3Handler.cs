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
			SetMode(GetUserMention());
		}

		private SocketUser? GetUserMention()
		{
			foreach(SocketUser itm in _userMessage.MentionedUsers)
			{
				if (itm.IsBot) continue;
				if (itm.Id == _userMessage.Author.Id) continue;

				return itm;
			}
			return null;
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

			RestUserMessage message;
			SocketUser? duet = GetUserMention();
			if(duet is not null)
			{
				message = await RespondDuetAsync(duet).ConfigureAwait(false);
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
