using Discord.WebSocket;
using Discord;
using Discord.Interactions;
using System.Reflection;
using TLDBot.Utility;
using TLDBot.Modules;
using Lavalink4NET;
using Discord.Commands;
using TLDBot.Handlers;

namespace TLDBot.Services
{
	internal sealed class BotService : IHostedService
	{
		private readonly DiscordSocketClient _Client;
		private readonly InteractionService _interactionService;
		private readonly CommandService _commandService;
		private readonly IServiceProvider _Provider;
		private readonly IConfiguration _Config;

		public BotService(DiscordSocketClient Client, InteractionService Interaction, CommandService Command, IServiceProvider Provider, IConfiguration Config)
		{
			ArgumentNullException.ThrowIfNull(Client);
			ArgumentNullException.ThrowIfNull(Interaction);
			ArgumentNullException.ThrowIfNull(Command);
			ArgumentNullException.ThrowIfNull(Provider);
			ArgumentNullException.ThrowIfNull(Config);

			_Client = Client;
			_interactionService = Interaction;
			_commandService = Command;
			_Provider = Provider;
			_Config = Config;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			_Client.InteractionCreated += InteractionCreated;
			_Client.MessageReceived += MessageReceived;
			_Client.ButtonExecuted += ButtonExecuted;
			_Client.SelectMenuExecuted += SelectMenuExecuted;
			_Client.Ready += ClientReady;
			_Client.Log += Log;

			_Provider.GetService<IAudioService>()!.TrackStarted += Helper.TrackStartedAsync;
			_Provider.GetService<IAudioService>()!.TrackEnded += Helper.TrackEndedAsync;
			_Provider.GetService<IAudioService>()!.Players.PlayerDestroyed += Helper.PlayerDestroyedAsync;

			await _Client.LoginAsync(TokenType.Bot, _Config["DiscordToken"]).ConfigureAwait(false);
			await _Client.StartAsync().ConfigureAwait(false);
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			_Client.InteractionCreated -= InteractionCreated;
			_Client.MessageReceived -= MessageReceived;
			_Client.ButtonExecuted -= ButtonExecuted;
			_Client.SelectMenuExecuted -= SelectMenuExecuted;
			_Client.Ready -= ClientReady;
			_Client.Log -= Log;

			_Provider.GetService<IAudioService>()!.TrackStarted -= Helper.TrackStartedAsync;
			_Provider.GetService<IAudioService>()!.TrackEnded -= Helper.TrackEndedAsync;
			_Provider.GetService<IAudioService>()!.Players.PlayerDestroyed -= Helper.PlayerDestroyedAsync;

			Lavalink.Stop();
			await _Client.StopAsync().ConfigureAwait(false);
		}

		private Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}

		private async Task MessageReceived(SocketMessage messageParam)
		{
			SocketUserMessage? message = messageParam as SocketUserMessage;
			if (message is null) return;

			int argPos = 0;
			if (!(message.HasStringPrefix(_Config["Prefix"], ref argPos) || 
				message.HasMentionPrefix(_Client.CurrentUser, ref argPos)) || message.Author.IsBot) return;

			SocketCommandContext context = new SocketCommandContext(_Client, message);
			SearchResult command = _commandService.Search(context, argPos);
			if(command.Text is null)
			{
				await _commandService.ExecuteAsync(context, "chat " + context.Message.Content.Substring(argPos), _Provider).ConfigureAwait(false);
				return;
			}

			await _commandService.ExecuteAsync(context, argPos, _Provider).ConfigureAwait(false);
		}

		private async Task InteractionCreated(SocketInteraction interaction)
		{
			//Get interaction
			if(interaction is SocketSlashCommand)
			{
				SocketInteractionContext interactionContext = new SocketInteractionContext(_Client, interaction);
				await _interactionService.ExecuteCommandAsync(interactionContext, _Provider).ConfigureAwait(false);
			}
		}

		private async Task SelectMenuExecuted(SocketMessageComponent component)
		{
			SocketCommandContext commandContext = new SocketCommandContext(_Client, component.Message);

			SelectMenuModule menuModule = new SelectMenuModule(_Provider.GetService<IAudioService>()!, component, commandContext);
			await menuModule.ExecuteCommandAsync(component.Data.CustomId).ConfigureAwait(false);
		}

		private async Task ButtonExecuted(SocketMessageComponent component)
		{
			SocketCommandContext commandContext = new SocketCommandContext(_Client, component.Message);

			ButtonModule buttonModule = new ButtonModule(_Provider.GetService<IAudioService>()!, component, commandContext);
			await buttonModule.ExecuteCommandAsync(component.Data.CustomId).ConfigureAwait(false);
		}

		private async Task ClientReady()
		{
			await _interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), _Provider).ConfigureAwait(false);
			//await _interactionService.RegisterCommandsGloballyAsync(true).ConfigureAwait(false);
			await _interactionService.RegisterCommandsToGuildAsync(1082505225098756127, true).ConfigureAwait(false);

			await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _Provider).ConfigureAwait(false);

			if (string.IsNullOrEmpty(_Config["Gemini:Credentials:ApiKey"]) is true) return;
			AIChatHandler.GenerateGoogleAI(_Config["Gemini:Credentials:ApiKey"]!);
		}
	}
}
