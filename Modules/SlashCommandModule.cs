﻿using Discord.Interactions;
using Lavalink4NET;
using TLDBot.Handlers;

namespace TLDBot.Modules
{
	public sealed class SlashCommandModule : InteractionModuleBase<SocketInteractionContext>
	{
		private readonly IAudioService _audioService;
		private MusicHandler? _musicHandler;

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

			_musicHandler = new MusicHandler(_audioService, interactionContext: Context);
			return Task.CompletedTask;
		}

		[SlashCommand("disconnect", "Disconnects from the current voice channel connected to", runMode: RunMode.Async)]
		public async Task DisconnectAsync() => await _musicHandler!.DisconnectAsync().ConfigureAwait(false);

		[SlashCommand("play", description: "Plays music", runMode: RunMode.Async)]
		public async Task PlayAsync(string query) => await _musicHandler!.PlayAsync(query).ConfigureAwait(false);

		[SlashCommand("position", description: "Shows the track position", runMode: RunMode.Async)]
		public async Task PositionAsync() => await _musicHandler!.PositionAsync().ConfigureAwait(false);

		[SlashCommand("stop", description: "Stops the current track", runMode: RunMode.Async)]
		public async Task StopAsync() => await _musicHandler!.StopAsync().ConfigureAwait(false);

		[SlashCommand("volume", description: "Sets the player volume (0 - 1000%)", runMode: RunMode.Async)]
		public async Task VolumeAsync(int volume = 100) => await _musicHandler!.VolumeAsync(volume).ConfigureAwait(false);

		[SlashCommand("skip", description: "Skips the current track", runMode: RunMode.Async)]
		public async Task SkipAsync() => await _musicHandler!.SkipAsync().ConfigureAwait(false);

		[SlashCommand("loop", description: "Loop/Unloop the current track/queue", runMode: RunMode.Async)]
		public async Task LoopAsync() => await _musicHandler!.LoopAsync().ConfigureAwait(false);

		[SlashCommand("shuffle", description: "Shuffle/Un shuffle the current queue", runMode: RunMode.Async)]
		public async Task ShuffleAsync() => await _musicHandler!.ShuffleAsync().ConfigureAwait(false);

		[SlashCommand("pause", description: "Pauses the player.", runMode: RunMode.Async)]
		public async Task PauseAsync() => await _musicHandler!.PauseAsync().ConfigureAwait(false);

		[SlashCommand("resume", description: "Resumes the player.", runMode: RunMode.Async)]
		public async Task ResumeAsync() => await _musicHandler!.ResumeAsync().ConfigureAwait(false);
	}
}
