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
			if (await IsPermission() is false) return;
			if (await IsYouTurn() is false) return;

			string state = GetStatePlay(row, col);
			bool isOver = IsGameOver();

			await _messageComponent.UpdateAsync(msg =>
			{
				msg.Content = state;
				msg.Embed = Embeds.T3Start(_messageComponent.User, BodyBoardPorcess);
				msg.Components = isOver ? new ComponentBuilder().Build() : Component;
			});
			if (isOver) ResetBase();
		}

		public async Task SetChoice(char name)
		{
			if (await IsPermission() is false) return;

			SetChooseXO(name);
			if (_player.IsDuet && GetStringDuetChoice(_player.UserDuet!.Id).Equals(Description.State.NotSelect))
			{
				string duet = _messageComponent.User.Mention + ": " + GetStringDuetChoice(_messageComponent.User.Id) + Environment.NewLine;
				duet += _player.UserDuet.Mention + ": " + GetStringDuetChoice(_player.UserDuet.Id) + Environment.NewLine;

				await _messageComponent.UpdateAsync(msg =>
				{
					msg.Embed = Embeds.T3StartDuet(_messageComponent.User, DescriptionDuet, duet);
					msg.Components = ComponentChooseXO;
				}).ConfigureAwait(false);
				return;
			}

			await _messageComponent.UpdateAsync(msg =>
			{
				msg.Embed = Embeds.T3Start(_messageComponent.User, BodyBoardPorcess);
				msg.Components = ComponentFirst;
			}).ConfigureAwait(false);
		}

		private async Task<bool> IsPermission()
		{
			if (_messageComponent is null) return false;
			if (_player is null) return false;

			if (_messageComponent.Message.Id.Equals(_player.MessageId)) return true;

			await _messageComponent.RespondAsync(text: Description.Permission.NotAllow, ephemeral: true).ConfigureAwait(false);
			return false;
		}

		private async Task<bool> IsYouTurn()
		{
			if (_messageComponent is null) return false;
			if (_messageComponent.Message.Components.Count == 0) return false;

			ButtonComponent btn = (ButtonComponent)_messageComponent.Message.Components.First().Components.First();
			if (btn.Emote.Name.Contains(_player.SelectChar)) return true;

			await _messageComponent.RespondAsync(text: Description.Permission.NotTurn, ephemeral: true).ConfigureAwait(false);
			return false;
		}
	}
}
