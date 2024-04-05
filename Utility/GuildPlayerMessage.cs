using Discord.Rest;
using Discord.WebSocket;
using Lavalink4NET.Players.Vote;

namespace TLDBot.Utility
{
	public class GuildPlayerMessage
	{
		public ISocketMessageChannel Channel;
		public ulong MessageId;
		public VoteLavalinkPlayer VotePlayer;
		public SocketUser User;

		public GuildPlayerMessage(ISocketMessageChannel channel, ulong messageId, VoteLavalinkPlayer votePlayer, SocketUser user)
		{
			this.Channel = channel;
			this.MessageId = messageId;
			this.VotePlayer = votePlayer;
			this.User = user;
		}
	}
}
