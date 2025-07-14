using SamenHost.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamenHost.Sessions
{

    /// <summary>
    /// History is the base of a change.
    /// </summary>
    public abstract class History
    {
        /// <summary>
        /// The user that made the history
        /// </summary>
        public User author;

        /// <summary>
        /// Send the history as a packet to a user
        /// </summary>
        /// <param name="connection"></param>
        public abstract void SendAsPacket(Connection connection);

        /// <summary>
        /// Broadcast the change to everyone in a specific session
        /// </summary>
        /// <param name="session">The session to broadcast from</param>
        /// <param name="excludeAuthor">If the author should be excluded from the packet
        /// This is almost always the case, as we dont want to send the update that they themselfs just made.
        /// That would cause stuttering as the server is always behind.</param>
        public void Broadcast(Session session, bool excludeAuthor = true)
        {
            foreach (User user in session.GetUsers())
            {
                if (author != null && user.Username == author.Username && excludeAuthor)
                    continue;

                SendAsPacket(user.GetConnection());
            }
        }

    }
}
