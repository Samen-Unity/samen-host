using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamenHost.Chat
{
    /// <summary>
    /// A message send in a session chat
    /// </summary>
    public class ChatMessage
    {
        /// <summary>
        /// Create a message that was send 'now'
        /// </summary>
        /// <param name="author">The author of the message</param>
        /// <param name="content">The content of the message</param>
        public ChatMessage(string author, string content)
            : this(author, content, DateTime.Now)
        {
           
        }

        string author;
        string content;
        DateTime sendTime;

        /// <summary>
        /// Create a chatmessage instance
        /// </summary>
        /// <param name="author">The creator of the message</param>
        /// <param name="content">The content of the message</param>
        /// <param name="sendTime">The time the message was send</param>
        public ChatMessage(string author, string content, DateTime sendTime)
        {
            this.author = author;
            this.content = content;
            this.sendTime = sendTime;
        }


        /// <summary>
        /// The creator of the message (Not a user persee, can also be 'System' or anything else
        /// </summary>
        /// <returns></returns>
        public string GetAuthor()
        {
            return author;
        }


        /// <summary>
        /// The content of the message
        /// </summary>
        /// <returns></returns>
        public string GetContent()
        {
            return content;
        }


        /// <summary>
        /// The time the message was send
        /// </summary>
        /// <returns></returns>
        public DateTime GetSendTime()
        {
            return sendTime;
        }
    }
}
