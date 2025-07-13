using SamenHost.Core;
using SamenHost.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamenHost.Chat
{
    /// <summary>
    /// A chat linked to a specific session
    /// </summary>
    public class SessionChat
    {
        private Session session;

        /// <summary>
        /// A chat linked to a specific session
        /// </summary>
        /// <param name="session">The session to link the chat with</param>
        public SessionChat(Session session)
        {
            this.session = session;
        }

        /// <summary>
        /// Add a new command to this chat
        /// </summary>
        /// <param name="command"></param>
        public void RegisterCommand(ChatCommand command)
        {
            chatCommands.Add(command);
        }

        private List<ChatCommand> chatCommands = new List<ChatCommand>();

        /// <summary>
        /// Send a message to the chat
        /// </summary>
        /// <param name="chatMessage">The message to send</param>
        public void SendMessage(ChatMessage chatMessage)
        {
            OnMessageSend?.Invoke(chatMessage);

            if (chatMessage.GetContent().StartsWith("/"))
            {
                string full = chatMessage.GetContent().Substring(1);
                string[] parts = full.Split(' ');

                string command = parts[0];
                string[] arguments = new string[parts.Length - 1];
                Array.Copy(parts, 1, arguments, 0, arguments.Length);

                foreach(ChatCommand chatCommand in chatCommands)
                {
                    if (chatCommand.GetCommand().ToLower() == command.ToLower())
                    {
                        User user = session.GetUserByName(chatMessage.GetAuthor());
                        chatCommand.OnExecute?.Invoke(user, arguments);
                    }
                }
            }

            session.WriteHistory(new ChatMessageHistory(chatMessage));
        }

        /// <summary>
        /// Action gets called whenever a message has been send to the chat
        /// </summary>
        public Action<ChatMessage> OnMessageSend;

        /// <summary>
        /// Send a message with the default 'System' username.
        /// </summary>
        /// <param name="content">The content of the message</param>
        public void SendMessage(string content)
        {
            SendMessage("System", content);
        }

        /// <summary>
        /// Send a message to the chat
        /// </summary>
        /// <param name="username">The username of the message author</param>
        /// <param name="content">The message contents</param>
        public void SendMessage(string username, string content)
        {
            SendMessage(new ChatMessage(username, content));
        }
    }
}
