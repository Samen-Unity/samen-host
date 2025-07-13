using SamenHost.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamenHost.Sessions
{
    /// <summary>
    /// A object duplication change
    /// </summary>
    public class ObjectDuplicatedHistory : History
    {
        /// <summary>
        /// The id of the object that was duplicated
        /// </summary>
        string sourceId;

        /// <summary>
        /// The id of the new object that was created
        /// </summary>
        string duplicatedId;

        /// <summary>
        /// A history that records an object being duplicated
        /// </summary>
        /// <param name="author">The author of the change</param>
        /// <param name="sourceId">The original source of the duplication</param>
        /// <param name="duplicatedId">The newly created object ID</param>
        public ObjectDuplicatedHistory(User author, string sourceId, string duplicatedId)
        {
            this.author = author;

            this.sourceId = sourceId;
            this.duplicatedId = duplicatedId;

            if(this.sourceId == this.duplicatedId)
            {
                Logging.Log("HISTORY", $"Duplication in history was corrupt. source and duplicated ID can not be the same!", LogType.ERROR);
                return;
            }
        }


        /// <summary>
        /// Send the history as a packet
        /// </summary>
        /// <param name="connection">The connection to send it to</param>
        public override void SendAsPacket(Connection connection)
        {
            connection.SendPacket(new OutgoingPacket(PacketType.ObjectDuplicated)
                .WriteString(sourceId)
                .WriteString(duplicatedId));
        }
    }
}
