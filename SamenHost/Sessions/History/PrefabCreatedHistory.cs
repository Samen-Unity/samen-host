using SamenHost.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace SamenHost.Sessions
{
    public class PrefabCreatedHistory : History
    {
        string assetPath;
        string[] ids;
        public PrefabCreatedHistory(User author, string assetPath, string[] ids)
        {
            this.author = author;
            this.assetPath = assetPath;
            this.ids = ids;

        }

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
