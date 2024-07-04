using Discord.Commands;
using Discord.WebSocket;
using Lavalink4NET;
using System.Reflection;
using TLDBot.Handlers.SelectMenu;

namespace TLDBot.Modules
{
	public class SelectMenuModule
	{
		private readonly MenuMusicHandler _musicHandler;
		private readonly SocketMessageComponent _component;

		public SelectMenuModule(IAudioService audioService, SocketMessageComponent messageComponent, SocketCommandContext context)
		{
			ArgumentNullException.ThrowIfNull(audioService);
			ArgumentNullException.ThrowIfNull(messageComponent);
			ArgumentNullException.ThrowIfNull(context);

			_musicHandler = new MenuMusicHandler(audioService, messageComponent, context);
			_component = messageComponent;
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

		public async Task SearchAsync() => await _musicHandler.SearchAsync(_component.Data.Values).ConfigureAwait(false);
	}
}
