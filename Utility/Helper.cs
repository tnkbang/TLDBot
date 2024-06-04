using Discord.WebSocket;
using Discord;
using Lavalink4NET.Events.Players;
using Lavalink4NET.Tracks;
using Lavalink4NET.Players;
using Newtonsoft.Json;
using TLDBot.Handlers;
using TLDBot.Structs;

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
					dynamic data = JsonConvert.DeserializeObject(json)!;
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
					HooHeyHow data = JsonConvert.DeserializeObject<HooHeyHow>(json)!;
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
					TicTacToe data = JsonConvert.DeserializeObject<TicTacToe>(json)!;
					return data;
				}
			}
		}

		public static string GetDescription(string cmd)
		{
			return CommandInfo.description[cmd] ?? "Command not description.";
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
			await UpdatePlayingAsync(eventArgs.Player, eventArgs.Track, isUpdateEmbed: true).ConfigureAwait(false);
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
				await playerMessage.Channel.DeleteMessageAsync(playerMessage.MessageId).ConfigureAwait(false);
				MusicHandler.GuildPlayer.Remove(eventArgs.Player.GuildId);
			}
		}
	}
}
