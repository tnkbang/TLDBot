using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using Lavalink4NET;
using TLDBot.Handlers;

namespace TLDBot.Modules
{
	public class ButtonModule
	{
		private readonly MusicHandler _musicHandler;

		public ButtonModule(IAudioService audioService, SocketMessageComponent messageComponent, SocketCommandContext context)
		{
			ArgumentNullException.ThrowIfNull(audioService);
			ArgumentNullException.ThrowIfNull(messageComponent);
			ArgumentNullException.ThrowIfNull(context);

			_musicHandler = new MusicHandler(audioService, messageComponent: messageComponent, commandContext: context);
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

			Console.WriteLine("Func not found: " + cmdName + "Async");
			await Task.CompletedTask;
		}

		public async Task DisconnectAsync() => await _musicHandler.DisconnectAsync().ConfigureAwait(false);

		public async Task PositionAsync() => await _musicHandler.PositionAsync().ConfigureAwait(false);

		public async Task StopAsync() => await _musicHandler.StopAsync().ConfigureAwait(false);

		public async Task SkipAsync() => await _musicHandler.SkipAsync().ConfigureAwait(false);

		public async Task LoopAsync() => await _musicHandler.LoopAsync().ConfigureAwait(false);

		public async Task PauseAsync() => await _musicHandler.PauseAsync().ConfigureAwait(false);

		public async Task ResumeAsync() => await _musicHandler.ResumeAsync().ConfigureAwait(false);
	}
}
