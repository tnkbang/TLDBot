using Lavalink4NET.Players;
using Lavalink4NET.Players.Queued;

namespace TLDBot.Structs
{
	public class Music
	{
		public Info Description = new Info();

		public class Info
		{
			public string Nothing = string.Empty;
			public string Duration = string.Empty;
			public string Author = string.Empty;
			public State State = new State();
			public Status Status = new Status();
			public Disconnect Disconnect = new Disconnect();
			public Play Play = new Play();
			public Search Search = new Search();
			public Position Position = new Position();
			public Stop Stop = new Stop();
			public Volume Volume = new Volume();
			public Skip Skip = new Skip();
			public Loop Loop = new Loop();
			public Shuffle Shuffle = new Shuffle();
			public Seek Seek = new Seek();
			public Pause Pause = new Pause();
			public Resume Resume = new Resume();
			public Queue Queue = new Queue();

			public string GetAuthor(string username)
			{
				return Author.Replace("{username}", username);
			}

			public string GetPlayerState(PlayerState state)
			{
				return state switch
				{
					PlayerState.Playing => State.Playing,
					PlayerState.Paused => State.Paused,
					PlayerState.NotPlaying => State.NotPlaying,
					PlayerState.Destroyed => State.Destroyed,
					_ => Status.Unknown,
				};
			}
		}

		public class State
		{
			public string Title = string.Empty;
			public string On = string.Empty;
			public string Off = string.Empty;
			public string Track = string.Empty;
			public string Queue = string.Empty;
			public string Destroyed = string.Empty;
			public string NotPlaying = string.Empty;
			public string Playing = string.Empty;
			public string Paused = string.Empty;
		}

		public class Status
		{
			public string NotInVoice = string.Empty;
			public string NotSameVoice = string.Empty;
			public string BotNotConnect = string.Empty;
			public string Unknown = string.Empty;
		}

		public class Disconnect
		{
			public string Body = string.Empty;
		}

		public class Play
		{
			public string Title = string.Empty;
			public string Body = string.Empty;
			public string AddTitle = string.Empty;
			public string AddBody = string.Empty;
			public string ErrTitle = string.Empty;
			public string ErrBody = string.Empty;

			public string GetBody(string name)
			{
				return Body.Replace("{track}", name);
			}

			public string GetAddBody(string name)
			{
				return AddBody.Replace("{track}", name);
			}

			public string GetErrBody(string query)
			{
				return ErrBody.Replace("{query}", query);
			}
		}

		public class Search
		{
			public string Title = string.Empty;
			public string Body = string.Empty;
			public string Info = string.Empty;

			public string GetBody(int count)
			{
				return Body.Replace("{count}", count.ToString());
			}
		}

		public class Position
		{
			public string Title = string.Empty;
			public string Body = string.Empty;

			public string GetBody(string state)
			{
				return Body.Replace("{state}", state);
			}
		}

		public class Stop
		{
			public string Title = string.Empty;
			public string Body = string.Empty;
		}

		public class Volume
		{
			public string Title = string.Empty;
			public string Body = string.Empty;
			public string ErrTitle = string.Empty;
			public string ErrBody = string.Empty;

			public string GetBody(int volume)
			{
				return Body.Replace("{volume}", volume.ToString());
			}
		}

		public class Skip
		{
			public string Title = string.Empty;
			public string Body = string.Empty;
			public string Empty = string.Empty;

			public string GetBody(string name)
			{
				return Body.Replace("{track}", name);
			}
		}

		public class Loop
		{
			public string Title = string.Empty;
			public string Track = string.Empty;
			public string Queue = string.Empty;
			public string None = string.Empty;
			public Types Type = new Types();

			public string GetType(TrackRepeatMode type)
			{
				return type switch
				{
					TrackRepeatMode.None => Type.None,
					TrackRepeatMode.Track => Type.Track,
					TrackRepeatMode.Queue => Type.Queue,
					_ => Type.None,
				};
			}

			public class Types
			{
				public string Track = string.Empty;
				public string Queue = string.Empty;
				public string None = string.Empty;
			}
		}

		public class Shuffle
		{
			public string Title = string.Empty;
			public string Body = string.Empty;
			public string On = string.Empty;
			public string Off = string.Empty;

			public string GetBody(bool state)
			{
				return Body.Replace("{state}", state ? On : Off);
			}

			public string GetState(bool state)
			{
				return state ? On : Off;
			}
		}

		public class Seek
		{
			public string Title = string.Empty;
			public string Body = string.Empty;

			public string GetBody(TimeSpan time)
			{
				return Body.Replace("{time}", time.ToString(@"hh\:mm\:ss"));
			}
		}

		public class Pause
		{
			public string Title = string.Empty;
			public string Already = string.Empty;
			public string Done = string.Empty;
		}

		public class Resume
		{
			public string Title = string.Empty;
			public string Already = string.Empty;
			public string Done = string.Empty;
		}

		public class Queue
		{
			public string Title = string.Empty;
			public string Info = string.Empty;
			public string Play = string.Empty;
			public string InQueue = string.Empty;
			public string IsNull = string.Empty;

			public string GetInfo(int count)
			{
				return Info.Replace("{count}", count.ToString());
			}
		}
	}
}
