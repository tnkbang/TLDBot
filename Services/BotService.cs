using Discord.WebSocket;
using Discord;
using Discord.Interactions;
using System.Reflection;
using TLDBot.Utility;
using TLDBot.Modules;
using Lavalink4NET;
using Discord.Commands;

namespace TLDBot.Services
{
	internal sealed class BotService : IHostedService
	{
		private readonly DiscordSocketClient _Client;
		private readonly InteractionService _Service;
		private readonly IServiceProvider _Provider;
		private readonly IConfiguration _Config;

		public BotService(DiscordSocketClient Client, InteractionService Service, IServiceProvider Provider, IConfiguration Config)
		{
			ArgumentNullException.ThrowIfNull(Client);
			ArgumentNullException.ThrowIfNull(Service);
			ArgumentNullException.ThrowIfNull(Provider);
			ArgumentNullException.ThrowIfNull(Config);

			_Client = Client;
			_Service = Service;
			_Provider = Provider;
			_Config = Config;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			_Client.InteractionCreated += InteractionCreated;
			_Client.MessageReceived += MessageReceived;
			_Client.Ready += ClientReady;
			_Client.Log += Log;

			await _Client.LoginAsync(TokenType.Bot, _Config["DiscordToken"]).ConfigureAwait(false);
			await _Client.StartAsync().ConfigureAwait(false);
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			_Client.InteractionCreated -= InteractionCreated;
			_Client.MessageReceived -= MessageReceived;
			_Client.Ready -= ClientReady;
			_Client.Log -= Log;
			
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
			var message = messageParam as SocketUserMessage;
			if (message == null) return;
			if(message.Interaction != null) return;

			if (message.Content.Contains($"<@{_Client.CurrentUser.Id}>"))
			{
				await message.Channel.SendMessageAsync("Please using slash commands. Prefix commands is development....");
			}
		}

		private Task InteractionCreated(SocketInteraction interaction)
		{
			//Get interaction
			if(interaction is SocketSlashCommand)
			{
				SocketInteractionContext interactionContext = new SocketInteractionContext(_Client, interaction);
				return _Service!.ExecuteCommandAsync(interactionContext, _Provider);
			}

			//Get button click
			if(interaction is SocketMessageComponent)
			{
				SocketMessageComponent cbn = (SocketMessageComponent)interaction;
				if (cbn.Data.CustomId == ButtonComponents.PREFIX_ID + Helper.ACTION_PAUSE)
				{
					MessageComponent btn = Helper.CreateButtons([Helper.ACTION_RESUME, Helper.ACTION_SKIP, Helper.ACTION_STOP]);
					cbn.UpdateAsync(msg => msg.Components = btn);
				}
				IAudioService? audioService = _Provider.GetService<IAudioService>();
				if(audioService is not null)
				{
					CommandContext context = new CommandContext(_Client, cbn.Message);
					new ButtonModule(audioService, context).DisconnectAsync().ConfigureAwait(true);
				}
			}

			return Task.CompletedTask;
		}

		private async Task ClientReady()
		{
			await _Service.AddModulesAsync(Assembly.GetExecutingAssembly(), _Provider).ConfigureAwait(false);
			await _Service.RegisterCommandsGloballyAsync(true).ConfigureAwait(false);
		}
	}
}
