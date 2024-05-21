using Discord.Interactions;
using TLDBot.Utility;

namespace TLDBot.Handlers.Slash
{
	public class SlashH3Handler : HooHeyHowHandler
	{
		private readonly SocketInteractionContext _interactionContext;

		public SlashH3Handler(SocketInteractionContext interactionContext) : base(interactionContext.User)
		{
			_interactionContext = interactionContext;
		}

		public override async Task RespondAsync(string choice)
		{
			if (_interactionContext is null) return;

			if (string.IsNullOrEmpty(choice))
			{
				await _interactionContext.Interaction.RespondAsync(embed: Embeds.H3Start(_interactionContext.User, StartDes),
					components: Component).ConfigureAwait(false);
				return;
			}

			choice = choice.ToLower();
			if (await IsCorrectChoice(choice) is false) return;
			await _interactionContext.Interaction.RespondAsync(embed: GetEmbedProcess(strKey[choice], _interactionContext.User));
		}

		private async Task<bool> IsCorrectChoice(string choice)
		{
			bool check = strKey.TryGetValue(choice, out var value);
			if (check) return true;

			await _interactionContext.Interaction.RespondAsync("Vui lòng chọn 1 trong 6 linh vật của trò chơi bầu cua!").ConfigureAwait(false);
			return false;
		}
	}
}
