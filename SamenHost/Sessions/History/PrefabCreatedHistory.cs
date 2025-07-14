using SamenHost.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace SamenHost.Sessions
{

    /// <summary>
    /// A history to keep track of prefabs created
    /// </summary>
    public class PrefabCreatedHistory : History
    {
        string assetPath;
        string[] ids;

        /// <summary>
        ///  A history to keep track of prefabs created
        /// </summary>
        /// <param name="author">The user who made the prefab</param>
        /// <param name="assetPath">The prefab file path</param>
        /// <param name="ids">The ids of the newly generated objects</param>
        public PrefabCreatedHistory(User author, string assetPath, string[] ids)
        {
            this.author = author;
            this.assetPath = assetPath;
            this.ids = ids;
        }

        /// <summary>
        /// Send this history as a packet
        /// </summary>
        /// <param name="connection"></param>
        public override void SendAsPacket(Connection connection)
        {
            OutgoingPacket packet = new OutgoingPacket(PacketType.PrefabCreated);
            packet.WriteString(assetPath);
            packet.WriteInt(ids.Length);
            for(int i = 0; i < ids.Length; i++)
            {
                packet.WriteString(ids[i]);
            }

            connection.SendPacket(packet);
        }
    }
}
