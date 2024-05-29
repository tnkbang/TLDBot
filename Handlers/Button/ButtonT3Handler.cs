using Discord;
using Discord.WebSocket;
using TLDBot.Utility;

namespace TLDBot.Handlers.Button
{
	public class ButtonT3Handler : TicTacToeHandler
	{
		private readonly SocketMessageComponent _messageComponent;

		public ButtonT3Handler(SocketMessageComponent message) : base(message.User)
		{
			_messageComponent = message;
		}

		public async Task RespondAsync(int row, int col)
		{
			if (_messageComponent is null) return;
			if (await IsUserStart() is false) return;

			string state = GetStatePlay(row, col);
			bool isOver = IsGameOver();

			await _messageComponent.UpdateAsync(msg =>
			{
				msg.Content = state;
				msg.Embed = Embeds.T3Start(_messageComponent.User, Description);
				msg.Components = isOver ? new ComponentBuilder().Build() : Component;
			});
			InitializeBoard(isOver);
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
