using Discord.Commands;
using Lavalink4NET;
using System.Globalization;
using TLDBot.Handlers;
using TLDBot.Handlers.Message;

namespace TLDBot.Modules
{
	public sealed class MessageCommandModule : ModuleBase<SocketCommandContext>
	{
		private readonly IAudioService _audioService;

		private AIChatHandler? chatHandler;
		private MessageMusicHandler? musicHandler;
		private MessageH3Handler? hooheyhowHandler;
		private MessageT3Handler? tictactoeHandler;

		public MessageCommandModule(IAudioService audioService)
		{
			ArgumentNullException.ThrowIfNull(audioService);

			_audioService = audioService;
		}

		protected override async Task BeforeExecuteAsync(CommandInfo command)
		{
			await Context.Channel.TriggerTypingAsync();
			await base.BeforeExecuteAsync(command);

			chatHandler = new AIChatHandler();
			musicHandler = new MessageMusicHandler(_audioService, Context.Message, Context);
			hooheyhowHandler = new MessageH3Handler(Context.Message);
			tictactoeHandler = new MessageT3Handler(Context);

			await Task.CompletedTask;
		}

		#region AI Chat
		[Command(text: "chat", Summary = "AI chat bot", RunMode = RunMode.Async)]
		public async Task ChatAsync([Remainder] string prompt = "Xin chào") 
			=> await Context.Channel.SendMessageAsync(text: await chatHandler!.GenerateContent(prompt)).ConfigureAwait(false);
		#endregion

		#region Music
		[Command(text: "disconnect", Summary = "Disconnects from the current voice channel connected to", RunMode = RunMode.Async)]
		public async Task DisconnectAsync() => await musicHandler!.DisconnectAsync().ConfigureAwait(false);

		[Command(text: "play", Summary = "Plays music", RunMode = RunMode.Async)]
		public async Task PlayAsync([Remainder] string query = "") => await musicHandler!.PlayAsync(query).ConfigureAwait(false);

		[Alias("search", "s")]
		[Command(text: "search", Summary = "Plays music", RunMode = RunMode.Async)]
		public async Task SearchAsync([Remainder] string query = "") => await musicHandler!.SearchAsync(query).ConfigureAwait(false);

		[Command(text: "position", Summary = "Shows the track position", RunMode = RunMode.Async)]
		public async Task PositionAsync() => await musicHandler!.PositionAsync().ConfigureAwait(false);

		[Command(text: "stop", Summary = "Stops the current track", RunMode = RunMode.Async)]
		public async Task StopAsync() => await musicHandler!.StopAsync().ConfigureAwait(false);

		[Command(text: "volume", Summary = "Sets the player volume (0 - 1000%)", RunMode = RunMode.Async)]
		public async Task VolumeAsync([Remainder] int volume = 100) => await musicHandler!.VolumeAsync(volume).ConfigureAwait(false);

		[Command(text: "skip", Summary = "Skips the current track", RunMode = RunMode.Async)]
		public async Task SkipAsync() => await musicHandler!.SkipAsync().ConfigureAwait(false);

		[Command(text: "loop", Summary = "Loop/Unloop the current track/queue", RunMode = RunMode.Async)]
		public async Task LoopAsync() => await musicHandler!.LoopAsync().ConfigureAwait(false);

		[Command(text: "shuffle", Summary = "Shuffle/Un shuffle the current queue", RunMode = RunMode.Async)]
		public async Task ShuffleAsync() => await musicHandler!.ShuffleAsync().ConfigureAwait(false);

		[Command(text: "seek", Summary = "Seek the current track (hh:mm:ss)", RunMode = RunMode.Async)]
		public async Task SeekAsync([Remainder] string time)
			=> await musicHandler!.SeekAsync(TimeSpan.ParseExact(time, @"hh\:mm\:ss", CultureInfo.InvariantCulture), true).ConfigureAwait(false);

		[Command(text: "pause", Summary = "Pauses the player.", RunMode = RunMode.Async)]
		public async Task PauseAsync() => await musicHandler!.PauseAsync().ConfigureAwait(false);

		[Command(text: "resume", Summary = "Resumes the player.", RunMode = RunMode.Async)]
		public async Task ResumeAsync() => await musicHandler!.ResumeAsync().ConfigureAwait(false);

		[Command(text: "queue", Summary = "Queue in the player.", RunMode = RunMode.Async)]
		public async Task QueueAsync() => await musicHandler!.QueueAsync().ConfigureAwait(false);
		#endregion

		#region HooHeyHow
		[Command(text: "hooheyhow", Summary = "The hoo hey how game", RunMode = RunMode.Async)]
		[Alias("bc", "baucua")]
		public async Task HooheyhowAsync([Remainder] string choice = "") => await hooheyhowHandler!.RespondAsync(choice).ConfigureAwait(false);
		#endregion

		#region TicTacToe
		[Command(text: "tictactoe", Summary = "The tic tac toe game", RunMode = RunMode.Async)]
		[Alias("ttt", "caro")]
		public async Task TicTacToeAsync(string? input = null) => await tictactoeHandler!.RespondAsync().ConfigureAwait(false);
		#endregion
	}
}
