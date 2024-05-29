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

		public override async Task RespondAsync()
		{
			if (_userMessage is null) return;

			await _userMessage.Channel.SendMessageAsync(embed: Embeds.T3Start(_userMessage.Author, Description), components: Component).ConfigureAwait(false);
			return;
		}
	}
}
