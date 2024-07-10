using Discord.WebSocket;
using Discord;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Tracks;
using Lavalink4NET.Players;
using Newtonsoft.Json;
using TLDBot.Handlers;
using TLDBot.Structs;
using System.Text.RegularExpressions;

namespace TLDBot.Utility
{
	public class Helper
	{
		/// <summary>
		/// Static discord socket client
		/// </summary>
		public static DiscordSocketClient Client
		{
			get
			{
				return new DiscordSocketClient(new DiscordSocketConfig
				{
					GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent,
					LogLevel = LogSeverity.Info
				});
			}
		}

		public static dynamic CommandInfo
		{
			get
			{
				using (StreamReader r = new StreamReader("Json/CommandInfo.json"))
				{
					string json = r.ReadToEnd();
					dynamic? data = JsonConvert.DeserializeObject(json);

					if (data is null) return "Load json err!";
					return data;
				}
			}
		}

		public static Music Music
		{
			get
			{
				using (StreamReader r = new StreamReader("Json/Music.json"))
				{
					string json = r.ReadToEnd();
					Music? data = JsonConvert.DeserializeObject<Music>(json);

					if(data is null) return new Music();
					return data;
				}
			}
		}

		public static HooHeyHow HooHeyHow
		{
			get
			{
				using (StreamReader r = new StreamReader("Json/HooHeyHow.json"))
				{
					string json = r.ReadToEnd();
					HooHeyHow? data = JsonConvert.DeserializeObject<HooHeyHow>(json);
					
					if (data is null) return new HooHeyHow();
					return data;
				}
			}
		}

		public static TicTacToe TicTacToe
		{
			get
			{
				using (StreamReader r = new StreamReader("Json/TicTacToe.json"))
				{
					string json = r.ReadToEnd();
					TicTacToe? data = JsonConvert.DeserializeObject<TicTacToe>(json);
					
					if (data is null) return new TicTacToe();
					return data;
				}
			}
		}

		public static string GetDescription(string cmd)
		{
			return CommandInfo.description[cmd] ?? "Command not description.";
		}

		/// <summary>
		/// Convert text removes special characters
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string SanitizeText(string input)
		{
			var pattern = @"[^\u0000-\u007F\x00-\x7F\u0080-\u024F\u1E00-\u1EF9]";
			Regex regex = new Regex(pattern);
			return regex.Replace(input, "");
		}

		/// <summary>
		/// Create button
		/// </summary>
		/// <param name="lstAction">List button action type</param>
		/// <returns></returns>
		public static ComponentBuilder CreateButtons(ComponentBuilder builder, string[] lstAction,string type,  int row = 0)
		{
			foreach (string action in lstAction)
			{
				Buttons btnC = new Buttons();
				builder.WithButton(btnC.ExecuteButtonBuilder(action, type), row);
			}

			return builder;
		}

		/// <summary>
		/// Catch the track start event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		/// <returns></returns>
		public static async Task TrackStartedAsync(object sender, TrackStartedEventArgs eventArgs)
		{
			await Task.Delay(TimeSpan.FromSeconds(0.2)).ConfigureAwait(false); // await 0.2s for player running
			await UpdatePlayingAsync(eventArgs.Player, eventArgs.Track, isUpdateEmbed: true, isUpdateComponent: true).ConfigureAwait(false);
		}

		public static async Task TrackEndedAsync(object sender, TrackEndedEventArgs eventArgs)
		{
			GuildPlayerMessage? playerMessage;
			MusicHandler.GuildPlayer.TryGetValue(eventArgs.Player.GuildId, out playerMessage);

			if (playerMessage is not null)
			{
				if (eventArgs.Player.State is not PlayerState.NotPlaying) return;

				await playerMessage.Channel.ModifyMessageAsync(playerMessage.MessageId, msg => {
					msg.Embed = Embeds.Playing(playerMessage.VotePlayer, eventArgs.Track, playerMessage.User);
					msg.Components = new ComponentBuilder().Build();
				}).ConfigureAwait(false);
			}
		}

		public static async Task UpdatePlayingAsync(ILavalinkPlayer player, LavalinkTrack track, bool isUpdateEmbed = false, bool isUpdateComponent = false)
		{
			GuildPlayerMessage? playerMessage;
			MusicHandler.GuildPlayer.TryGetValue(player.GuildId, out playerMessage);

			if (playerMessage is not null)
			{
				await playerMessage.Channel.ModifyMessageAsync(playerMessage.MessageId, msg => {
					msg.Embed = isUpdateEmbed ? Embeds.Playing(playerMessage.VotePlayer, track, playerMessage.User) : msg.Embed;
					msg.Components = isUpdateComponent ? MusicHandler.GetComponent(isPause: player.State is PlayerState.Paused) : msg.Components;
				}).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Catch the player destroy event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArgs"></param>
		/// <returns></returns>
		public static async Task PlayerDestroyedAsync(object sender, PlayerDestroyedEventArgs eventArgs)
		{
			GuildPlayerMessage? playerMessage;
			MusicHandler.GuildPlayer.TryGetValue(eventArgs.Player.GuildId, out playerMessage);

			if(playerMessage is not null)
			{
				MusicHandler.GuildPlayer.Remove(eventArgs.Player.GuildId);

				if (eventArgs.Player.CurrentTrack is null)
				{
					await playerMessage.Channel.DeleteMessageAsync(playerMessage.MessageId).ConfigureAwait(false);
					return;
				}

				await playerMessage.Channel.ModifyMessageAsync(playerMessage.MessageId, msg => {
					msg.Embed = Embeds.Playing(playerMessage.VotePlayer, eventArgs.Player.CurrentTrack, playerMessage.User);
					msg.Components = new ComponentBuilder().Build();
				}).ConfigureAwait(false);
			}
		}
	}
}
