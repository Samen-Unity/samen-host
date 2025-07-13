using SamenHost.Core;
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
        public Action<User, string[]> OnExecute;
    }
}
