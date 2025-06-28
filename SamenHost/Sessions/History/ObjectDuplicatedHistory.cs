using SamenHost.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamenHost.Sessions
{
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

        public ObjectDuplicatedHistory(User author, string sourceId, string duplicatedId)
        {
            this.author = author;

            this.sourceId = sourceId;
            this.duplicatedId = duplicatedId;

            if(this.sourceId == this.duplicatedId)
            {
                Console.WriteLine("Corruption: sourceId and duplicatedId can not be the same!");
            }
        }

        public override void SendAsPacket(Connection connection)
        {
            connection.SendPacket(new OutgoingPacket(PacketType.ObjectDuplicated)
                .WriteString(sourceId)
                .WriteString(duplicatedId));
        }
    }
}
