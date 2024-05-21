using Discord.Commands;
using Discord.WebSocket;
using Lavalink4NET;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using TLDBot.Handlers;
using TLDBot.Handlers.Message;

namespace TLDBot.Modules
{
	public class MessageCommandModule
	{
		private readonly SocketUserMessage _userMessage;
		private readonly DiscordSocketClient _client;
		private readonly IConfiguration _config;

		private readonly AIChatHandler chatHandler;
		private readonly MessageMusicHandler musicHandler;
		private readonly MessageH3Handler hooheyhowHandler;

		private string _prefix
		{
			get { return _config["Prefix"] ?? ""; }
		}

		private string _input = string.Empty;

		public MessageCommandModule(DiscordSocketClient client, IAudioService audioService, SocketUserMessage userMessage, IConfiguration config)
		{
			ArgumentNullException.ThrowIfNull(client);
			ArgumentNullException.ThrowIfNull(audioService);
			ArgumentNullException.ThrowIfNull(userMessage);
			ArgumentNullException.ThrowIfNull(config);

			_client = client;
			_userMessage = userMessage;
			_config = config;

			chatHandler = new AIChatHandler();
			musicHandler = new MessageMusicHandler(audioService, _userMessage, new SocketCommandContext(_client, _userMessage));
			hooheyhowHandler = new MessageH3Handler(_userMessage);
		}

		private Queue<string> MessageToQueue()
		{
			string input = Regex.Replace(_userMessage.Content, @"\s+", " ").Trim();
			Queue<string> queue = new Queue<string>();

			foreach (string word in input.Split(" "))
			{
				queue.Enqueue(word);
			}
			return queue;
		}

		public async Task ExecuteCommandAsync()
		{
			Queue<string> queueMsg = MessageToQueue();
			string startMsg = queueMsg.Dequeue();

			string cmdName;
			if (!startMsg.StartsWith(_prefix) && !startMsg.Contains(_client.CurrentUser.Id.ToString())) return; //Not handle if message not call bot

			//Get command name from message
			if (startMsg.StartsWith(_prefix)) cmdName = startMsg.Substring(_prefix.Length);
			else cmdName = queueMsg.Dequeue() ?? "";

			if (cmdName == "") return;
			_input = String.Join(" ", queueMsg);
			await _userMessage.Channel.TriggerTypingAsync().ConfigureAwait(false);

			MethodInfo? method = GetType().GetMethod(cmdName.ToLower() + "Async");
			if (method is not null)
			{
				_ = method.Invoke(this, null) as Task;
				return;
			}

			await _userMessage.Channel.SendMessageAsync(text: await chatHandler.GenerateContent(cmdName + " " + _input)).ConfigureAwait(false);
		}

		#region Music
		public async Task disconnectAsync() => await musicHandler.DisconnectAsync().ConfigureAwait(false);

		public async Task playAsync() => await musicHandler.PlayAsync(_input).ConfigureAwait(false);

		public async Task positionAsync() => await musicHandler.PositionAsync().ConfigureAwait(false);

		public async Task stopAsync() => await musicHandler.StopAsync().ConfigureAwait(false);

		public async Task volumeAsync() => await musicHandler.VolumeAsync(int.Parse(_input)).ConfigureAwait(false);

		public async Task skipAsync() => await musicHandler.SkipAsync().ConfigureAwait(false);

		public async Task loopAsync() => await musicHandler.LoopAsync().ConfigureAwait(false);

		public async Task shuffleAsync() => await musicHandler.ShuffleAsync().ConfigureAwait(false);

		public async Task seekAsync()
			=> await musicHandler.SeekAsync(TimeSpan.ParseExact(_input, @"hh\:mm\:ss", CultureInfo.InvariantCulture), true).ConfigureAwait(false);

		public async Task pauseAsync() => await musicHandler.PauseAsync().ConfigureAwait(false);

		public async Task resumeAsync() => await musicHandler.ResumeAsync().ConfigureAwait(false);

		public async Task queueAsync() => await musicHandler.QueueAsync().ConfigureAwait(false);
		#endregion

		#region HooHeyHow
		public async Task bcAsync() => await hooheyhowHandler.RespondAsync(_input).ConfigureAwait(false);

		public async Task baucuaAsync() => await hooheyhowHandler.RespondAsync(_input).ConfigureAwait(false);

		public async Task hooheyhowAsync() => await hooheyhowHandler.RespondAsync(_input).ConfigureAwait(false);
		#endregion
	}
}
