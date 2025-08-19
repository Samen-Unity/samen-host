using SamenHost.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamenHost.Sessions
{
    internal class ComponentAddedHistory : History
    {

        private string objectId;
        private string componentType;
        public ComponentAddedHistory(User author, string objectId, string componentType)
        {
            this.author = author;

            this.objectId = objectId;
            this.componentType = componentType; 
        }

        public override void SendAsPacket(Connection connection)
        {
            OutgoingPacket packet = new OutgoingPacket(PacketType.ComponentAdded);
            packet.WriteString(objectId);
            packet.WriteString(componentType);
            connection.SendPacket(packet);
        }
    }
}
