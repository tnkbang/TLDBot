using Discord.Interactions;
using Lavalink4NET;
using TLDBot.Handlers;

namespace TLDBot.Modules
{
	public sealed class SlashCommandModule : InteractionModuleBase<SocketInteractionContext>
	{
		private readonly IAudioService _audioService;
		private readonly MusicHandler _musicHandler = new MusicHandler();

		/// <summary>
		/// Initializes a new instance of the <see cref="SlashCommandModule"/> class.
		/// </summary>
		/// <param name="audioService">The audio service</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if the specified <paramref name="audioService"/> is <see langword="null"/>.
		/// </exception>
		public SlashCommandModule(IAudioService audioService)
		{
			ArgumentNullException.ThrowIfNull(audioService);

			_audioService = audioService;
		}

		[SlashCommand("disconnect", "Disconnects from the current voice channel connected to", runMode: RunMode.Async)]
		public async Task DisconnectAsync() => await _musicHandler.DisconnectAsync(_audioService, Context).ConfigureAwait(false);

		[SlashCommand("play", description: "Plays music", runMode: RunMode.Async)]
		public async Task PlayAsync(string query) => await _musicHandler.PlayAsync(_audioService, Context, query).ConfigureAwait(false);

		[SlashCommand("position", description: "Shows the track position", runMode: RunMode.Async)]
		public async Task PositionAsync() => await _musicHandler.PositionAsync(_audioService, Context).ConfigureAwait(false);

		[SlashCommand("stop", description: "Stops the current track", runMode: RunMode.Async)]
		public async Task StopAsync() => await _musicHandler.StopAsync(_audioService, Context).ConfigureAwait(false);

		[SlashCommand("volume", description: "Sets the player volume (0 - 1000%)", runMode: RunMode.Async)]
		public async Task VolumeAsync(int volume = 100) => await _musicHandler.VolumeAsync(_audioService, Context).ConfigureAwait(false);

		[SlashCommand("skip", description: "Skips the current track", runMode: RunMode.Async)]
		public async Task SkipAsync() => await _musicHandler.StopAsync(_audioService, Context).ConfigureAwait(false);

		[SlashCommand("pause", description: "Pauses the player.", runMode: RunMode.Async)]
		public async Task PauseAsync() => await _musicHandler.PauseAsync(_audioService, Context).ConfigureAwait(false);

		[SlashCommand("resume", description: "Resumes the player.", runMode: RunMode.Async)]
		public async Task ResumeAsync() => await _musicHandler.ResumeAsync(_audioService, Context).ConfigureAwait(false);
	}
}
