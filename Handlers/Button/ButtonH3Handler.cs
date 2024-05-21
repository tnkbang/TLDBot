using Discord;
using Discord.WebSocket;

namespace TLDBot.Handlers.Button
{
	public class ButtonH3Handler : HooHeyHowHandler
	{
		private readonly SocketMessageComponent _messageComponent;

		public ButtonH3Handler(SocketMessageComponent messageComponent) : base(messageComponent.User)
		{
			_messageComponent = messageComponent;
		}

		public override async Task RespondAsync(string choice)
		{
			if (_messageComponent is null) return;
			if (await IsUserStart() is false) return;

			await _messageComponent.UpdateAsync(msg =>
			{
				msg.Embed = GetEmbedProcess(choice, _messageComponent.User);
				msg.Components = new ComponentBuilder().Build();
			});
		}

		private async Task<bool> IsUserStart()
		{
			if (_messageComponent is null) return false;

			string userStart = _messageComponent.Message.Embeds.First().Footer!.Value.Text;
			if (_messageComponent.User.GlobalName.Equals(userStart) is true) return true;

			await _messageComponent.RespondAsync(text: "You aren't author this game.", ephemeral: true).ConfigureAwait(false);
			return false;
		}
	}
}
