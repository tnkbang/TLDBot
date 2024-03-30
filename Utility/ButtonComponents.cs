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
			if (method != null) return method.Invoke(this, null) as ButtonBuilder;

			Console.WriteLine("Func not found: " + type + btnName);
			return new ButtonBuilder();
		}

		public ButtonBuilder MusicPlay()
		{
			return new ButtonBuilder().WithLabel(Helper.ACTION_PLAY).WithStyle(ButtonStyle.Primary).WithCustomId(PREFIX_ID + Helper.ACTION_PLAY);
		}

		public ButtonBuilder MusicPause()
		{
			return new ButtonBuilder().WithLabel(Helper.ACTION_PAUSE).WithStyle(ButtonStyle.Primary).WithCustomId(PREFIX_ID + Helper.ACTION_PAUSE);
		}

		public ButtonBuilder MusicResume()
		{
			return new ButtonBuilder().WithLabel(Helper.ACTION_RESUME).WithStyle(ButtonStyle.Primary).WithCustomId(PREFIX_ID + Helper.ACTION_RESUME);
		}

		public ButtonBuilder MusicLoop()
		{
			return new ButtonBuilder().WithLabel(Helper.ACTION_LOOP).WithStyle(ButtonStyle.Success).WithCustomId(PREFIX_ID + Helper.ACTION_LOOP);
		}

		public ButtonBuilder MusicSkip()
		{
			return new ButtonBuilder().WithLabel(Helper.ACTION_SKIP).WithStyle(ButtonStyle.Success).WithCustomId(PREFIX_ID + Helper.ACTION_SKIP);
		}

		public ButtonBuilder MusicStop()
		{
			return new ButtonBuilder().WithLabel(Helper.ACTION_STOP).WithStyle(ButtonStyle.Danger).WithCustomId(PREFIX_ID + Helper.ACTION_STOP);
		}
	}
}
