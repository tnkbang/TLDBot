using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using TLDBot.Utility;

namespace TLDBot.Handlers.Slash
{
	public class SlashT3Handler : TicTacToeHandler
	{
		private readonly SocketInteractionContext _interactionContext;

		public SlashT3Handler(SocketInteractionContext interactionContext) : base(interactionContext.User)
		{
			_interactionContext = interactionContext;
		}

		private async Task<RestFollowupMessage> RespondDuetAsync(SocketUser user)
		{
			string duet = _interactionContext.User.Mention + ": " + GetStringDuetChoice(_interactionContext.User.Id) + Environment.NewLine;
			duet += user.Mention + ": " + GetStringDuetChoice(user.Id) + Environment.NewLine;
			return await _interactionContext.Interaction.FollowupAsync(
				embed: Embeds.T3StartDuet(_interactionContext.User, DescriptionDuet, duet, IsSetTitle), components: ComponentChooseXO).ConfigureAwait(false);
		}

		private async Task<bool> IsMentionCorrect(SocketUser user)
		{
			if (user.IsBot)
			{
				await _interactionContext.Interaction.FollowupAsync(Description.Permission.NotMentionBot, ephemeral: true).ConfigureAwait(false);
				return false;
			}
			if (user.Id.Equals(_interactionContext.User.Id))
			{
				await _interactionContext.Interaction.FollowupAsync(Description.Permission.NotMention, ephemeral: true).ConfigureAwait(false);
				return false;
			}

			return true;
		}

		private async Task<bool> HasAlreadyGame()
		{
			if (_player.MessageId is 0) return false;

			await _interactionContext.Interaction.FollowupAsync(Description.State.Already, ephemeral: true).ConfigureAwait(false);
			return true;
		}

		public async Task RespondAsync(SocketUser? user)
		{
			if (_interactionContext is null) return;
			await _interactionContext.Interaction.DeferAsync(true);

			if (await HasAlreadyGame()) return;
			SetMode(user);

			RestFollowupMessage message;
			if (user is not null)
			{
				if (await IsMentionCorrect(user) is false) return;
				message = await RespondDuetAsync(user).ConfigureAwait(false);
			}
			else
			{
				message = await _interactionContext.Interaction.FollowupAsync(
					embed: Embeds.T3Start(_interactionContext.User, DescriptionWithBot, IsSetTitle), components: ComponentChooseXO).ConfigureAwait(false);
			}

			SetMessageBoard(message.Id);
		}
	}
}
