using SamenHost.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamenHost.Sessions
{
    /// <summary>
    /// A history that a message was send
    /// </summary>
    public class ChatMessageHistory : History
    {
        ChatMessage chatMessage;

        /// <summary>
        /// A history that a message was send
        /// </summary>
        /// <param name="chatMessage"></param>
        public ChatMessageHistory(ChatMessage chatMessage)
        {
            this.chatMessage = chatMessage;
        }

        /// <summary>
        /// Send the history as a packet to a user
        /// </summary>
        /// <param name="connection"></param>
        public override void SendAsPacket(Connection connection)
        {
            connection.SendPacket(new OutgoingPacket(PacketType.ChatMessage)
                .WriteString(chatMessage.GetAuthor())
                .WriteString(chatMessage.GetContent())
                .WriteString(chatMessage.GetSendTime().ToString()));
        }
    }
}
