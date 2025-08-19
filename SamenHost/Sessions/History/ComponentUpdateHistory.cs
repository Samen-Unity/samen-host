using SamenHost.Chat;
using SamenHost.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamenHost.Sessions
{
    internal class ComponentUpdateHistory : History
    {

        string objectId;
        string component;
        string json;

        /// <summary>
        /// A history that a message was send
        /// </summary>
        /// <param name="chatMessage"></param>
        public ComponentUpdateHistory(User author, string objectId, string component, string json)
        {
            this.author = author;

            this.objectId = objectId;
            this.component = component;
            this.json = json;
        }

        /// <summary>
        /// Send the history as a packet to a user
        /// </summary>
        /// <param name="connection"></param>
        public override void SendAsPacket(Connection connection)
        {
            connection.SendPacket(new OutgoingPacket(PacketType.ComponentUpdated)
                .WriteString(objectId)
                .WriteString(component)
                .WriteString(json));
        }
        
    }
}
