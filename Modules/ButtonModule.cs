using Discord.Commands;
using Lavalink4NET;
using TLDBot.Handlers;

namespace TLDBot.Modules
{
	public class ButtonModule
	{
		private readonly IAudioService _audioService;
		private readonly CommandContext _Context;
		private readonly MusicHandler _musicHandler = new MusicHandler();

		public ButtonModule(IAudioService audioService, CommandContext context)
		{
			ArgumentNullException.ThrowIfNull(audioService);
			ArgumentNullException.ThrowIfNull(context);

			_audioService = audioService;
			_Context = context;
		}

		public async Task DisconnectAsync() => await _musicHandler.DisconnectAsync2(_audioService, _Context).ConfigureAwait(false);
	}
}
