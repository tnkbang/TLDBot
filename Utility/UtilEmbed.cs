using Discord;
using Discord.WebSocket;
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
				.WithImageUrl(track.ArtworkUri!.OriginalString)
				.AddField("Duration", track.Duration, inline: true)
				.AddField("Loop", player.RepeatMode, inline: true)
				.AddField("Shuffle", player.Shuffle, inline: true)
				.WithColor(Color.Red)
				.WithFooter(new EmbedFooterBuilder { Text = "Author by " + user.Username, IconUrl = user.GetAvatarUrl() })
				.WithCurrentTimestamp();

			return embed.Build();
		}

		public static Embed Info(string? title = null, string? description = null)
		{
			EmbedBuilder embed = new EmbedBuilder();

			embed.WithTitle(title).WithDescription(description).WithColor(Color.Green);

			return embed.Build();
		}
	}
}
