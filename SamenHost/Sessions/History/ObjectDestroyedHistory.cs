using SamenHost.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamenHost.Sessions
{
    /// <summary>
    /// ObjectDestroyedHistory is a history of an object being deleted
    /// </summary>
    public class ObjectDestroyedHistory : History
    {
        /// <summary>
        /// The object that was deleted
        /// </summary>
        string objectId;

        /// <summary>
        /// A history where an object got deleted
        /// </summary>
        /// <param name="author">The user that deleted the object</param>
        /// <param name="objectId">The object that was deleted</param>
        public ObjectDestroyedHistory(User author, string objectId)
        {
            this.objectId = objectId;
            this.author = author;
        }
        

        /// <summary>
        /// Send the change to a specific connection.
        /// </summary>
        /// <param name="connection"></param>
        public override void SendAsPacket(Connection connection)
        {
            OutgoingPacket packet = new OutgoingPacket(PacketType.ObjectDestroyed)
                .WriteString(objectId); // The id of the object

            connection.SendPacket(packet);
        }
    }
}
