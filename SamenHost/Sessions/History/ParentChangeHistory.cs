using SamenHost.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamenHost.Sessions
{
    internal class ParentChangeHistory : History
    {

        string objectId;
        string newParentId;
        public ParentChangeHistory(User author, string objectId, string newParentId)
        {
            this.author = author;
            this.objectId = objectId;
            this.newParentId = newParentId;
        }

        public override void SendAsPacket(Connection connection)
        {
            connection.SendPacket(new OutgoingPacket(PacketType.ParentChange)
                .WriteString(objectId)
                .WriteString(newParentId));
        }
    }
}
