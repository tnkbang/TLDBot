using Discord;
using Discord.WebSocket;
using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;
using Lavalink4NET.Players.Vote;
using Lavalink4NET.Tracks;
using TLDBot.Structs;

namespace TLDBot.Utility
{
	public class Embeds
	{
		public Embeds() { }

		private static readonly Music.Info Music = Helper.Music.Description;
		private static readonly HooHeyHow.Info HooHeyHow = Helper.HooHeyHow.Description;
		private static readonly TicTacToe.Info TicTacToe = Helper.TicTacToe.Description;

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

			if (track.Uri is null) return embed.Build();

			embed.WithAuthor(new EmbedAuthorBuilder { Name = track.Author });
			embed.WithTitle(track.Title).WithUrl(track.Uri.OriginalString);
			embed.WithThumbnailUrl(track.ArtworkUri?.OriginalString);
			embed.AddField(Music.Duration, track.Duration, inline: true);
			embed.AddField(Music.Loop.Title, Music.Loop.GetType(player.RepeatMode), inline: true);
			embed.AddField(Music.Shuffle.Title, Music.Shuffle.GetState(player.Shuffle), inline: true);
			embed.AddField(Music.Volume.Title, (player.Volume * 100) + "%", inline: true);
			embed.AddField(Music.Queue.Title, player.State is PlayerState.NotPlaying ? player.Queue.Count : player.Queue.Count + 1, inline: true);
			embed.AddField(Music.State.Title, Music.GetPlayerState(player.State), inline: true);
			embed.WithColor(Color.Red);
			embed.WithFooter(new EmbedFooterBuilder { Text = Music.GetAuthor(user.Username), IconUrl = user.GetAvatarUrl() });
			embed.WithCurrentTimestamp();

			return embed.Build();
		}

		public static Embed Queue(VoteLavalinkPlayer player)
		{
			EmbedBuilder embed = new EmbedBuilder();

			embed.WithTitle(Music.Queue.GetInfo(player.Queue.Count + 1));
			embed.AddField(Music.Queue.Play, player.CurrentTrack?.Title, inline: false);
			embed.AddField(Music.Queue.InQueue, GenerateListQueue(player.Queue), inline: false);
			embed.WithColor(Color.Red).WithCurrentTimestamp();

			return embed.Build();
		}

		private static string GenerateListQueue(ITrackQueue queue)
		{
			if (queue is null || queue.Count is 0)
			{
				return Music.Queue.IsNull;
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
			author.WithIconUrl(HooHeyHow.StartIcon);
			author.WithName(HooHeyHow.StartTitle);
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
			embed.AddField(HooHeyHow.Result, strResult);
			embed.WithFooter(H3Footer(user));
			embed.WithColor(color).WithCurrentTimestamp();

			return embed.Build();
		}
		#endregion

		#region Game TicTacToe
		private static EmbedAuthorBuilder T3Author()
		{
			EmbedAuthorBuilder author = new EmbedAuthorBuilder();
			author.WithIconUrl(HooHeyHow.StartIcon);
			author.WithName("Tic Tac Toe");
			return author;
		}

		private static EmbedFooterBuilder T3Footer(SocketUser user)
		{
			EmbedFooterBuilder footer = new EmbedFooterBuilder();
			footer.WithText(user.GlobalName);
			footer.WithIconUrl(user.GetAvatarUrl());
			return footer;
		}

		public static Embed T3Start(SocketUser user, string description, bool isTitle)
		{
			EmbedBuilder embed = new EmbedBuilder();
			embed.WithAuthor(T3Author());
			embed.WithThumbnailUrl(TicTacToe.ThumbnailUrl);
			embed.WithTitle(isTitle ? TicTacToe.Title : string.Empty);
			embed.WithDescription(description);
			embed.WithFooter(T3Footer(user));
			embed.WithColor(Color.LightOrange).WithCurrentTimestamp();

			return embed.Build();
		}

		public static Embed T3StartDuet(SocketUser user, string description, string duetState, bool isTitle)
		{
			EmbedBuilder embed = new EmbedBuilder();
			embed.WithAuthor(T3Author());
			embed.WithThumbnailUrl(TicTacToe.ThumbnailUrl);
			embed.WithTitle(isTitle ? TicTacToe.Title : string.Empty);
			embed.WithDescription(description);
			embed.AddField(TicTacToe.TitleField, duetState);
			embed.WithFooter(T3Footer(user));
			embed.WithColor(Color.LightOrange).WithCurrentTimestamp();

			return embed.Build();
		}
		#endregion
	}
}
