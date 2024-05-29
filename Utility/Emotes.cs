using System.Reflection;

namespace TLDBot.Utility
{
	public class Emotes
	{
		//Music
		public static readonly string Pause			= "<:pause:1131135530348855297>";
		public static readonly string Play			= "<:play:1131135527022768188>";
		public static readonly string Loop			= "<:repeat:1131135523742826596>";
		public static readonly string Next			= "<:next:1131135532542480394>";
		public static readonly string Shuffle		= "<:shuffle:1131135518390894673>";
		public static readonly string SeekPrev		= "<:seekprev:1226454248557121547>";
		public static readonly string SeekNext		= "<:seeknext:1226454246543589386>";
		public static readonly string Stop			= "<:stop:1131135513525502014>";
		public static readonly string Queue			= "<:queue:1226454244052439082>";
		public static readonly string Lyrics		= "<:lyrics:1226454241980186664>";
		public static readonly string Position		= "<:position:1233316227674275860>";

		//Game HooHeyHow
		public static readonly string HooHeyHow		= "<a:hooheyhow:1238668578421608458>";
		public static readonly string Deer			= "<:deer:1237917794780057701>";
		public static readonly string Calabash		= "<:calabash:1237917803563057244>";
		public static readonly string Chicken		= "<:chicken:1237917788702773301>";
		public static readonly string Fish			= "<:fish:1237917798295146596>";
		public static readonly string Crab			= "<:crab:1237917791827525722>";
		public static readonly string Lobster		= "<:lobster:1237917801293942834>";

		//Game TicTacToe
		public static readonly string CaroX			= "<:CaroX:1244945889693536286>";
		public static readonly string CaroO			= "<:CaroO:1244945887495585793>";
		public static readonly string CaroBlank		= ":black_small_square:";

		public static string GetByName(string name)
		{
			FieldInfo? field = typeof(Emotes).GetField(name);
			return field?.GetValue(null)?.ToString() ?? throw new ArgumentException("Not found emote.");
		}
	}
}
