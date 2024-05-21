using Discord;
using Discord.WebSocket;
using TLDBot.Utility;

namespace TLDBot.Handlers.Message
{
	public class MessageH3Handler : HooHeyHowHandler
	{
		private readonly SocketUserMessage _userMessage;

		public MessageH3Handler(SocketUserMessage userMessage) : base(userMessage.Author)
		{
			_userMessage = userMessage;
		}

		public override async Task RespondAsync(string choice)
		{
			if (_userMessage is null) return;

			if (string.IsNullOrEmpty(choice))
			{
				await _userMessage.Channel.SendMessageAsync(embed: Embeds.H3Start(_userMessage.Author, StartDes),
					components: Component).ConfigureAwait(false);
				return;
			}

			choice = choice.ToLower();
			if (await IsCorrectChoice(choice) is false) return;

			await _userMessage.Channel.SendMessageAsync(embed: GetEmbedProcess(strKey[choice], _userMessage.Author));
		}

		private async Task<bool> IsCorrectChoice(string choice)
		{
			bool check = strKey.TryGetValue(choice, out var value);
			if (check) return true;

			await _userMessage.ReplyAsync("Vui lòng chọn 1 trong 6 linh vật của trò chơi bầu cua!").ConfigureAwait(false);
			return false;
		}
	}
}
