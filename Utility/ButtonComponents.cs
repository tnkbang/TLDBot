using Discord;
using System.Reflection;
using TLDBot.Handlers;

namespace TLDBot.Utility
{
	public class ButtonComponents
	{
		public ButtonComponents() { }

		public static readonly string TYPE_MUSIC = "Music";
		public static readonly string TYPE_GAME = "Game";

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

		#region Music
		public ButtonBuilder MusicPause()
		{
			return new ButtonBuilder().WithEmote(Emote.Parse(Emotes.Pause)).WithStyle(ButtonStyle.Secondary).WithCustomId(MusicHandler.PAUSE);
		}

		public ButtonBuilder MusicResume()
		{
			return new ButtonBuilder().WithEmote(Emote.Parse(Emotes.Play)).WithStyle(ButtonStyle.Secondary).WithCustomId(MusicHandler.RESUME);
		}

		public ButtonBuilder MusicLoop()
		{
			return new ButtonBuilder().WithEmote(Emote.Parse(Emotes.Loop)).WithStyle(ButtonStyle.Secondary).WithCustomId(MusicHandler.LOOP);
		}

		public ButtonBuilder MusicSkip()
		{
			return new ButtonBuilder().WithEmote(Emote.Parse(Emotes.Next)).WithStyle(ButtonStyle.Secondary).WithCustomId(MusicHandler.SKIP);
		}

		public ButtonBuilder MusicShuffle()
		{
			return new ButtonBuilder().WithEmote(Emote.Parse(Emotes.Shuffle)).WithStyle(ButtonStyle.Secondary).WithCustomId(MusicHandler.SHUFFLE);
		}

		public ButtonBuilder MusicSeekPrev10S()
		{
			return new ButtonBuilder().WithEmote(Emote.Parse(Emotes.SeekPrev)).WithStyle(ButtonStyle.Secondary).WithCustomId(MusicHandler.SEEK_P10);
		}

		public ButtonBuilder MusicSeekNext10S()
		{
			return new ButtonBuilder().WithEmote(Emote.Parse(Emotes.SeekNext)).WithStyle(ButtonStyle.Secondary).WithCustomId(MusicHandler.SEEK_N10);
		}

		public ButtonBuilder MusicStop()
		{
			return new ButtonBuilder().WithEmote(Emote.Parse(Emotes.Stop)).WithStyle(ButtonStyle.Danger).WithCustomId(MusicHandler.STOP);
		}

		public ButtonBuilder MusicQueue()
		{
			return new ButtonBuilder().WithEmote(Emote.Parse(Emotes.Queue)).WithStyle(ButtonStyle.Secondary).WithCustomId(MusicHandler.QUEUE);
		}

		public ButtonBuilder MusicLyrics()
		{
			return new ButtonBuilder().WithEmote(Emote.Parse(Emotes.Lyrics)).WithStyle(ButtonStyle.Secondary).WithCustomId(MusicHandler.LYRICS);
		}
		
		public ButtonBuilder MusicPosition()
		{
			return new ButtonBuilder().WithEmote(Emote.Parse(Emotes.Position)).WithStyle(ButtonStyle.Secondary).WithCustomId(MusicHandler.POSITION);
		}
		#endregion

		#region Game HooHeyHow
		public ButtonBuilder GameDeer()
		{
			return new ButtonBuilder().WithEmote(Emote.Parse(Emotes.Deer)).WithStyle(ButtonStyle.Secondary).WithCustomId(HooHeyHowHandler.DEER);
		}

		public ButtonBuilder GameCalabash()
		{
			return new ButtonBuilder().WithEmote(Emote.Parse(Emotes.Calabash)).WithStyle(ButtonStyle.Secondary).WithCustomId(HooHeyHowHandler.CALABASH);
		}

		public ButtonBuilder GameChicken()
		{
			return new ButtonBuilder().WithEmote(Emote.Parse(Emotes.Chicken)).WithStyle(ButtonStyle.Secondary).WithCustomId(HooHeyHowHandler.CHICKEN);
		}

		public ButtonBuilder GameFish()
		{
			return new ButtonBuilder().WithEmote(Emote.Parse(Emotes.Fish)).WithStyle(ButtonStyle.Secondary).WithCustomId(HooHeyHowHandler.FISH);
		}

		public ButtonBuilder GameCrab()
		{
			return new ButtonBuilder().WithEmote(Emote.Parse(Emotes.Crab)).WithStyle(ButtonStyle.Secondary).WithCustomId(HooHeyHowHandler.CRAB);
		}

		public ButtonBuilder GameLobster()
		{
			return new ButtonBuilder().WithEmote(Emote.Parse(Emotes.Lobster)).WithStyle(ButtonStyle.Secondary).WithCustomId(HooHeyHowHandler.LOBSTER);
		}
		#endregion
	}
}
