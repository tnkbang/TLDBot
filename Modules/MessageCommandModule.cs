using Discord.WebSocket;
using System.Reflection;
using System.Text.RegularExpressions;
using TLDBot.Handlers;
using TLDBot.Utility;

namespace TLDBot.Modules
{
	public class MessageCommandModule
	{
		private readonly SocketUserMessage _userMessage;
		private readonly DiscordSocketClient _client;
		private readonly IConfiguration _config;

		private string _prefix
		{
			get { return _config["Prefix"] ?? ""; }
		}

		private string _input = string.Empty;

		public MessageCommandModule(DiscordSocketClient client, SocketUserMessage userMessage, IConfiguration config)
		{
			_client = client;
			_userMessage = userMessage;
			_config = config;
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

			MethodInfo? method = GetType().GetMethod(cmdName.ToLower() + "Async");
			if (method is not null)
			{
				_ = method.Invoke(this, null) as Task;
				return;
			}

			await Task.CompletedTask;
		}

		public async Task bcAsync() => await _userMessage.Channel.SendMessageAsync(embed: Embeds.H3Start(_userMessage.Author, HooHeyHowHandler.StartDes),
			components: HooHeyHowHandler.Component).ConfigureAwait(false);

		public async Task baucuaAsync() => await _userMessage.Channel.SendMessageAsync(embed: Embeds.H3Start(_userMessage.Author, HooHeyHowHandler.StartDes),
			components: HooHeyHowHandler.Component).ConfigureAwait(false);

		public async Task hooheyhowAsync() => await _userMessage.Channel.SendMessageAsync(embed: Embeds.H3Start(_userMessage.Author, HooHeyHowHandler.StartDes),
			components: HooHeyHowHandler.Component).ConfigureAwait(false);
	}
}
