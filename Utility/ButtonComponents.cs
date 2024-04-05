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

		public ButtonBuilder MusicPlay()
		{
			return new ButtonBuilder().WithLabel(Helper.ACTION_PLAY).WithStyle(ButtonStyle.Primary).WithCustomId(PREFIX_ID + Helper.ACTION_PLAY);
		}

		public ButtonBuilder MusicPause()
		{
			return new ButtonBuilder().WithLabel("❚❚").WithStyle(ButtonStyle.Secondary).WithCustomId(PREFIX_ID + Helper.ACTION_PAUSE);
		}

		public ButtonBuilder MusicResume()
		{
			return new ButtonBuilder().WithLabel("▶").WithStyle(ButtonStyle.Secondary).WithCustomId(PREFIX_ID + Helper.ACTION_RESUME);
		}

		public ButtonBuilder MusicLoop()
		{
			return new ButtonBuilder().WithLabel("⇄").WithStyle(ButtonStyle.Secondary).WithCustomId(PREFIX_ID + Helper.ACTION_LOOP);
		}

		public ButtonBuilder MusicSkip()
		{
			return new ButtonBuilder().WithLabel("⯮").WithStyle(ButtonStyle.Secondary).WithCustomId(PREFIX_ID + Helper.ACTION_SKIP);
		}

		public ButtonBuilder MusicShuffle()
		{
			return new ButtonBuilder().WithLabel("⤨").WithStyle(ButtonStyle.Secondary).WithCustomId(PREFIX_ID + Helper.ACTION_SHUFFLE);
		}

		public ButtonBuilder MusicSeekPrev5S()
		{
			return new ButtonBuilder().WithLabel("⟲5").WithStyle(ButtonStyle.Secondary).WithCustomId(PREFIX_ID + Helper.ACTION_SEEK_P5);
		}

		public ButtonBuilder MusicSeekPrev15S()
		{
			return new ButtonBuilder().WithLabel("⟲15").WithStyle(ButtonStyle.Secondary).WithCustomId(PREFIX_ID + Helper.ACTION_SEEK_P15);
		}

		public ButtonBuilder MusicSeekNext5S()
		{
			return new ButtonBuilder().WithLabel("5⟳").WithStyle(ButtonStyle.Secondary).WithCustomId(PREFIX_ID + Helper.ACTION_SEEK_N5);
		}

		public ButtonBuilder MusicSeekNext15S()
		{
			return new ButtonBuilder().WithLabel("15⟳").WithStyle(ButtonStyle.Secondary).WithCustomId(PREFIX_ID + Helper.ACTION_SEEK_N15);
		}

		public ButtonBuilder MusicStop()
		{
			return new ButtonBuilder().WithLabel("◼︎").WithStyle(ButtonStyle.Danger).WithCustomId(PREFIX_ID + Helper.ACTION_STOP);
		}

		public ButtonBuilder MusicQueue()
		{
			return new ButtonBuilder().WithLabel("☰").WithStyle(ButtonStyle.Secondary).WithCustomId(PREFIX_ID + Helper.ACTION_QUEUE);
		}
	}
}
