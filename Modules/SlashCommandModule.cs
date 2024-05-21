using Discord.Interactions;
using Lavalink4NET;
using System.Globalization;
using TLDBot.Handlers.Slash;

namespace TLDBot.Modules
{
	public sealed class SlashCommandModule : InteractionModuleBase<SocketInteractionContext>
	{
		private readonly IAudioService _audioService;
		private SlashMusicHandler? _musicHandler;
		private SlashH3Handler? _hooheyhowHandler;

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

		public override Task BeforeExecuteAsync(ICommandInfo command)
		{
			base.BeforeExecuteAsync(command);

			_musicHandler = new SlashMusicHandler(_audioService, interactionContext: Context);
			_hooheyhowHandler = new SlashH3Handler(Context);

			return Task.CompletedTask;
		}

		#region Music
		[SlashCommand("disconnect", description: "Disconnects from the current voice channel connected to", runMode: RunMode.Async)]
		public async Task DisconnectAsync() => await _musicHandler!.DisconnectAsync().ConfigureAwait(false);

		[SlashCommand("play", description: "Plays music", runMode: RunMode.Async)]
		public async Task PlayAsync([Summary(description: "Name or url track")] string query) 
			=> await _musicHandler!.PlayAsync(query).ConfigureAwait(false);

		[SlashCommand("position", description: "Shows the track position", runMode: RunMode.Async)]
		public async Task PositionAsync() => await _musicHandler!.PositionAsync().ConfigureAwait(false);

		[SlashCommand("stop", description: "Stops the current track", runMode: RunMode.Async)]
		public async Task StopAsync() => await _musicHandler!.StopAsync().ConfigureAwait(false);

		[SlashCommand("volume", description: "Sets the player volume (0 - 1000%)", runMode: RunMode.Async)]
		public async Task VolumeAsync([Summary(description: "Suggested volume from 0 to 100")] int volume = 100) 
			=> await _musicHandler!.VolumeAsync(volume).ConfigureAwait(false);

		[SlashCommand("skip", description: "Skips the current track", runMode: RunMode.Async)]
		public async Task SkipAsync() => await _musicHandler!.SkipAsync().ConfigureAwait(false);

		[SlashCommand("loop", description: "Loop/Unloop the current track/queue", runMode: RunMode.Async)]
		public async Task LoopAsync() => await _musicHandler!.LoopAsync().ConfigureAwait(false);

		[SlashCommand("shuffle", description: "Shuffle/Un shuffle the current queue", runMode: RunMode.Async)]
		public async Task ShuffleAsync() => await _musicHandler!.ShuffleAsync().ConfigureAwait(false);

		[SlashCommand("seek", description: "Seek the current track (hh:mm:ss)", runMode: RunMode.Async)]
		public async Task SeekAsync([Summary(description: "Only receive hh:mm:ss (Ex: 00:01:02)")] string time) 
			=> await _musicHandler!.SeekAsync(TimeSpan.ParseExact(time, @"hh\:mm\:ss", CultureInfo.InvariantCulture), true).ConfigureAwait(false);

		[SlashCommand("pause", description: "Pauses the player.", runMode: RunMode.Async)]
		public async Task PauseAsync() => await _musicHandler!.PauseAsync().ConfigureAwait(false);

		[SlashCommand("resume", description: "Resumes the player.", runMode: RunMode.Async)]
		public async Task ResumeAsync() => await _musicHandler!.ResumeAsync().ConfigureAwait(false);

		[SlashCommand("queue", description: "Queue in the player.", runMode: RunMode.Async)]
		public async Task QueueAsync() => await _musicHandler!.QueueAsync().ConfigureAwait(false);
		#endregion

		#region Game HooHeyHow
		[SlashCommand("hooheyhow", description: "The hoo hey how game", runMode: RunMode.Async)]
		public async Task HooHeyHowAsync([Summary(description: "Choose 1 of 6 mascots")] string choice = "")
			=> await _hooheyhowHandler!.RespondAsync(choice).ConfigureAwait(false);

		[SlashCommand("baucua", description: "The hoo hey how game", runMode: RunMode.Async)]
		public async Task BaucuaAsync([Summary(description: "Choose 1 of 6 mascots")] string choice = "")
			=> await _hooheyhowHandler!.RespondAsync(choice).ConfigureAwait(false);
		#endregion
	}
}
