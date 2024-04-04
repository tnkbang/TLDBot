using Discord.Rest;
using Discord.WebSocket;
using Lavalink4NET.Players.Vote;

namespace TLDBot.Utility
{
	public class GuildPlayerMessage
	{
		public RestFollowupMessage restFollowup;
		public VoteLavalinkPlayer votePlayer;
		public SocketUser user;

		public GuildPlayerMessage(RestFollowupMessage restFollowup, VoteLavalinkPlayer votePlayer, SocketUser user)
		{
			this.restFollowup = restFollowup;
			this.votePlayer = votePlayer;
			this.user = user;
		}
	}
}
