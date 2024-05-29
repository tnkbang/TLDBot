using Discord;
using Discord.WebSocket;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Players.Vote;
using Lavalink4NET.Tracks;

namespace TLDBot.Utility
{
	public class Embeds
	{
		public Embeds() { }

		#region Music
		/// <summary>
		/// Create embed message playing
		/// </summary>
		/// <param name="player"></param>
		/// <param name="track"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		public static Embed Playing(VoteLavalinkPlayer player, LavalinkTrack track, SocketUser user)
		{
			EmbedBuilder embed = new EmbedBuilder();

			embed.WithAuthor(new EmbedAuthorBuilder { Name = track.Author });
			embed.WithTitle(track.Title).WithUrl(track.Uri!.OriginalString);
			embed.WithThumbnailUrl(track.ArtworkUri?.OriginalString);
			embed.AddField("Duration", track.Duration, inline: true);
			embed.AddField("Loop", player.RepeatMode, inline: true);
			embed.AddField("Shuffle", player.Shuffle, inline: true);
			embed.AddField("Volume", (player.Volume * 100) + "%", inline: true);
			embed.AddField("Queue", player.State is PlayerState.NotPlaying ? player.Queue.Count : player.Queue.Count + 1, inline: true);
			embed.AddField("Player Status", player.State, inline: true);
			embed.WithColor(Color.Red);
			embed.WithFooter(new EmbedFooterBuilder { Text = "Author by " + user.Username, IconUrl = user.GetAvatarUrl() });
			embed.WithCurrentTimestamp();

			return embed.Build();
		}

		public static Embed Queue(VoteLavalinkPlayer player)
		{
			EmbedBuilder embed = new EmbedBuilder();

			embed.WithTitle("Queue: " + (player.Queue.Count + 1) + " track");
			embed.AddField("Track playing", player.CurrentTrack?.Title, inline: false);
			embed.AddField("Track in queue", GenerateListQueue(player.Queue), inline: false);
			embed.WithColor(Color.Red).WithCurrentTimestamp();

			return embed.Build();
		}

		private static string GenerateListQueue(ITrackQueue queue)
		{
			if (queue is null || queue.Count is 0)
			{
				return "No track in queue.";
			}

			string rsl = "";
			int count = 1;
			foreach(var item in queue)
			{
				if (rsl is not "") rsl += Environment.NewLine;
				rsl += count + ". " + item.Track?.Title;
				count++;
			}

			return rsl;
		}

		public static Embed Info(string? title = null, string? description = null)
		{
			EmbedBuilder embed = new EmbedBuilder();

			embed.WithTitle(title).WithDescription(description).WithColor(Color.Green);

			return embed.Build();
		}
		#endregion

		#region Game HooHeyHow
		public static Embed H3Start(SocketUser user, string description)
		{
			EmbedBuilder embed = new EmbedBuilder();
			embed.WithAuthor(H3Author());
			embed.WithDescription(description);
			embed.WithFooter(H3Footer(user));
			embed.WithColor(Color.LightOrange).WithCurrentTimestamp();

			return embed.Build();
		}

		private static EmbedAuthorBuilder H3Author()
		{
			EmbedAuthorBuilder author = new EmbedAuthorBuilder();
			author.WithIconUrl(Helper.HooHeyHow.StartIcon);
			author.WithName(Helper.HooHeyHow.StartTitle);
			return author;
		}

		private static EmbedFooterBuilder H3Footer(SocketUser user)
		{
			EmbedFooterBuilder footer = new EmbedFooterBuilder();
			footer.WithText(user.GlobalName);
			footer.WithIconUrl(user.GetAvatarUrl());
			return footer;
		}

		public static Embed H3Process(string title, string thumbnail, string description, string strResult, Color color, SocketUser user)
		{
			EmbedBuilder embed = new EmbedBuilder();
			embed.WithTitle(title);
			embed.WithThumbnailUrl(thumbnail);
			embed.WithDescription(description);
			embed.AddField(Helper.HooHeyHow.Result, strResult);
			embed.WithFooter(H3Footer(user));
			embed.WithColor(color).WithCurrentTimestamp();

			return embed.Build();
		}
		#endregion

		#region Game TicTacToe
		private static EmbedAuthorBuilder T3Author()
		{
			EmbedAuthorBuilder author = new EmbedAuthorBuilder();
			author.WithIconUrl(Helper.HooHeyHow.StartIcon);
			author.WithName("Tic Tac Toe");
			return author;
		}
		public static Embed T3Start(SocketUser user, string description)
		{
			EmbedBuilder embed = new EmbedBuilder();
			embed.WithAuthor(T3Author());
			embed.WithDescription(description);
			embed.WithFooter(H3Footer(user));
			embed.WithColor(Color.LightOrange).WithCurrentTimestamp();

			return embed.Build();
		}
		#endregion
	}
}
