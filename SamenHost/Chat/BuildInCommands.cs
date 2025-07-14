using SamenHost.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamenHost.Chat
{
    internal class BuildInCommands
    {
        internal static void Register()
        {
            SessionManager.OnSessionAdded += (Session session) =>
            {
                ChatCommand statsCommand = new ChatCommand("stats");
                session.GetChat().RegisterCommand(statsCommand);

                statsCommand.OnExecute += (CommandContext context) => {
                    int historySize = context.GetSession().GetAllHistory().Count;
                    context.GetChat().SendMessage($"History Size: {historySize}");
                };
            };
        }
    }
}
