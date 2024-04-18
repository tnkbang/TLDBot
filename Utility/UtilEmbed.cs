using Discord;
using Discord.WebSocket;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Players.Vote;
using Lavalink4NET.Tracks;

namespace TLDBot.Utility
{
	public class UtilEmbed
	{
		public UtilEmbed() { }

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

			embed.WithAuthor(new EmbedAuthorBuilder { Name = track.Author })
				.WithTitle(track.Title).WithUrl(track.Uri!.OriginalString)
				.WithThumbnailUrl(track.ArtworkUri?.OriginalString)
				.AddField("Duration", track.Duration, inline: true)
				.AddField("Loop", player.RepeatMode, inline: true)
				.AddField("Shuffle", player.Shuffle, inline: true)
				.AddField("Volume", (player.Volume * 100) + "%", inline: true)
				.AddField("Queue", player.Queue.Count + 1, inline: true)
				.AddField("Player Status", player.State, inline: true)
				.WithColor(Color.Red)
				.WithFooter(new EmbedFooterBuilder { Text = "Author by " + user.Username, IconUrl = user.GetAvatarUrl() })
				.WithCurrentTimestamp();

			return embed.Build();
		}

		public static Embed Queue(VoteLavalinkPlayer player)
		{
			EmbedBuilder embed = new EmbedBuilder();

			embed.WithTitle("Queue: " + (player.Queue.Count + 1) + " track")
				.AddField("Track playing", player.CurrentTrack?.Title, inline: false)
				.AddField("Track in queue", GenerateListQueue(player.Queue), inline: false)
				.WithColor(Color.Red).WithCurrentTimestamp();

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
	}
}
