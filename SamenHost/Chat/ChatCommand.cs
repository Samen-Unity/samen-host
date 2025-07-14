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
    /// A command listener that can be used to create a command that users active via the chat.
    /// </summary>
    public class ChatCommand
    {
        string command;

        /// <summary>
        /// Create an instance of a chat command
        /// </summary>
        /// <param name="command">The command the user needs to use</param>
        public ChatCommand(string command)
        {
            this.command = command;
        }

        /// <summary>
        /// Get the command the user needs to use
        /// </summary>
        /// <returns></returns>
        public string GetCommand()
        {
            return command;
        }


        /// <summary>
        /// User, Arguments. User can be null.
        /// </summary>
        public Action<CommandContext> OnExecute;
    }

    /// <summary>
    /// Context provided with a command
    /// </summary>
    public class CommandContext
    {
        /// <summary>
        /// The user that ran the command
        /// </summary>
        public User user { private set; get; }

        /// <summary>
        /// The arguments provided with the command
        /// </summary>
        public string[] arguments { private set; get; }

        /// <summary>
        /// The current session the user is in
        /// </summary>
        public Session session { private set; get; }

        /// <summary>
        /// Minimum command context
        /// </summary>
        /// <param name="user"></param>
        /// <param name="arguments"></param>
        public CommandContext(User user, string[] arguments)
        {
            this.user = user;
            this.arguments = arguments;
            this.session = user.GetSession();

        }
    }
}
