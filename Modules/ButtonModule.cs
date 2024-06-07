using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using Lavalink4NET;
using TLDBot.Handlers;
using TLDBot.Handlers.Button;
using TLDBot.Structs;

namespace TLDBot.Modules
{
	public sealed class ButtonModule
	{
		private readonly ButtonMusicHandler _musicHandler;
		private readonly ButtonH3Handler _heyHowHandler;
		private readonly ButtonT3Handler _t3Handler;

		public ButtonModule(IAudioService audioService, SocketMessageComponent messageComponent, SocketCommandContext context)
		{
			ArgumentNullException.ThrowIfNull(audioService);
			ArgumentNullException.ThrowIfNull(messageComponent);
			ArgumentNullException.ThrowIfNull(context);

			_musicHandler = new ButtonMusicHandler(audioService, messageComponent: messageComponent, commandContext: context);
			_heyHowHandler = new ButtonH3Handler(messageComponent);
			_t3Handler = new ButtonT3Handler(messageComponent);
		}

		public async Task ExecuteCommandAsync(string cmdName)
		{
			ArgumentNullException.ThrowIfNull(cmdName);

			MethodInfo? method = GetType().GetMethod(cmdName + "Async");
			if (method is not null)
			{
				_ = method.Invoke(this, null) as Task;
				return;
			}

			//Call Tic Tac Toe game
			if (cmdName.StartsWith("T3"))
			{
				int row = int.Parse(cmdName.Substring(2, 1));
				int col = int.Parse(cmdName.Substring(3, 1));
				await CaroAsync(row, col).ConfigureAwait(false);
				return;
			}

			Console.WriteLine("Func not found: " + cmdName + "Async");
			await Task.CompletedTask;
		}

		#region Music
		public async Task DisconnectAsync() => await _musicHandler.DisconnectAsync().ConfigureAwait(false);

		public async Task PositionAsync() => await _musicHandler.PositionAsync().ConfigureAwait(false);

		public async Task StopAsync() => await _musicHandler.StopAsync().ConfigureAwait(false);

		public async Task SkipAsync() => await _musicHandler.SkipAsync().ConfigureAwait(false);

		public async Task LoopAsync() => await _musicHandler.LoopAsync().ConfigureAwait(false);

		public async Task ShuffleAsync() => await _musicHandler.ShuffleAsync().ConfigureAwait(false);

		public async Task SeekPrev10SAsync() => await _musicHandler.SeekAsync(new TimeSpan(00, 00, -10)).ConfigureAwait(false);

		public async Task SeekNext10SAsync() => await _musicHandler.SeekAsync(new TimeSpan(00, 00, 10)).ConfigureAwait(false);

		public async Task PauseAsync() => await _musicHandler.PauseAsync().ConfigureAwait(false);

		public async Task ResumeAsync() => await _musicHandler.ResumeAsync().ConfigureAwait(false);

		public async Task QueueAsync() => await _musicHandler.QueueAsync().ConfigureAwait(false);
		#endregion

		#region Game HooHeyHow
		public async Task DeerAsync() => await _heyHowHandler.RespondAsync(HooHeyHowHandler.DEER).ConfigureAwait(false);

		public async Task CalabashAsync() => await _heyHowHandler.RespondAsync(HooHeyHowHandler.CALABASH).ConfigureAwait(false);

		public async Task ChickenAsync() => await _heyHowHandler.RespondAsync(HooHeyHowHandler.CHICKEN).ConfigureAwait(false);

		public async Task FishAsync() => await _heyHowHandler.RespondAsync(HooHeyHowHandler.FISH).ConfigureAwait(false);

		public async Task CrabAsync() => await _heyHowHandler.RespondAsync(HooHeyHowHandler.CRAB).ConfigureAwait(false);

		public async Task LobsterAsync() => await _heyHowHandler.RespondAsync(HooHeyHowHandler.LOBSTER).ConfigureAwait(false);
		#endregion

		#region Game TicTacToe
		public async Task CaroAsync(int row, int col) => await _t3Handler.RespondAsync(row, col).ConfigureAwait(false);

		public async Task CaroXAsync() => await _t3Handler.SetChoice(TicTacToe.PLAYER_X).ConfigureAwait(false);

		public async Task CaroOAsync() => await _t3Handler.SetChoice(TicTacToe.PLAYER_O).ConfigureAwait(false);
		#endregion
	}
}
