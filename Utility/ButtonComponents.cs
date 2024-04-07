using Discord;
using System.Reflection;

namespace TLDBot.Utility
{
	public class ButtonComponents
	{
		public ButtonComponents() { }

		public static readonly string PREFIX_ID = "button_";
		public static readonly string TYPE_MUSIC = "Music";

		/// <summary>
		/// Get button action style
		/// </summary>
		/// <param name="btnName">Button action name</param>
		/// <param name="type">Type action</param>
		/// <returns></returns>
		public ButtonBuilder? ExecuteButtonBuilder(string btnName, string type)
		{
			ArgumentNullException.ThrowIfNull(btnName);
			ArgumentNullException.ThrowIfNull(type);

			MethodInfo? method = GetType().GetMethod(type + btnName);
			if (method is not null) return method.Invoke(this, null) as ButtonBuilder;

			Console.WriteLine("Func not found: " + type + btnName);
			return new ButtonBuilder();
		}

		public ButtonBuilder MusicPause()
		{
			return new ButtonBuilder().WithEmote(Emote.Parse("<:pause:1131135530348855297>")).WithStyle(ButtonStyle.Secondary).WithCustomId(PREFIX_ID + Helper.ACTION_PAUSE);
		}

		public ButtonBuilder MusicResume()
		{
			return new ButtonBuilder().WithEmote(Emote.Parse("<:play:1131135527022768188>")).WithStyle(ButtonStyle.Secondary).WithCustomId(PREFIX_ID + Helper.ACTION_RESUME);
		}

		public ButtonBuilder MusicLoop()
		{
			return new ButtonBuilder().WithEmote(Emote.Parse("<:repeat:1131135523742826596>")).WithStyle(ButtonStyle.Secondary).WithCustomId(PREFIX_ID + Helper.ACTION_LOOP);
		}

		public ButtonBuilder MusicSkip()
		{
			return new ButtonBuilder().WithEmote(Emote.Parse("<:next:1131135532542480394>")).WithStyle(ButtonStyle.Secondary).WithCustomId(PREFIX_ID + Helper.ACTION_SKIP);
		}

		public ButtonBuilder MusicShuffle()
		{
			return new ButtonBuilder().WithEmote(Emote.Parse("<:shuffle:1131135518390894673>")).WithStyle(ButtonStyle.Secondary).WithCustomId(PREFIX_ID + Helper.ACTION_SHUFFLE);
		}

		public ButtonBuilder MusicSeekPrev10S()
		{
			return new ButtonBuilder().WithEmote(Emote.Parse("<:seekprev:1226454248557121547>")).WithStyle(ButtonStyle.Secondary).WithCustomId(PREFIX_ID + Helper.ACTION_SEEK_P10);
		}

		public ButtonBuilder MusicSeekNext10S()
		{
			return new ButtonBuilder().WithEmote(Emote.Parse("<:seeknext:1226454246543589386>")).WithStyle(ButtonStyle.Secondary).WithCustomId(PREFIX_ID + Helper.ACTION_SEEK_N10);
		}

		public ButtonBuilder MusicStop()
		{
			return new ButtonBuilder().WithEmote(Emote.Parse("<:stop:1131135513525502014>")).WithStyle(ButtonStyle.Danger).WithCustomId(PREFIX_ID + Helper.ACTION_STOP);
		}

		public ButtonBuilder MusicQueue()
		{
			return new ButtonBuilder().WithEmote(Emote.Parse("<:queue:1226454244052439082>")).WithStyle(ButtonStyle.Secondary).WithCustomId(PREFIX_ID + Helper.ACTION_QUEUE);
		}

		public ButtonBuilder MusicLyrics()
		{
			return new ButtonBuilder().WithEmote(Emote.Parse("<:lyrics:1226454241980186664>")).WithStyle(ButtonStyle.Secondary).WithCustomId(PREFIX_ID + Helper.ACTION_LYRICS);
		}
	}
}
