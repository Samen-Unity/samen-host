using Microsoft.Win32;
using SamenHost.Chat;
using SamenHost.Core;
using SamenHost.Internet;
using SamenHost.Plugins;
using SamenHost.ServerCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SamenHost
{
    internal class Program
    {
        public static int SamenVersion = 100;
        static void Main(string[] args)
        {
            /*
             *  Temp code, should be replace with a launcher at some point.
             */

            BuildInCommands.Register();
            Logging.Log("Samen", $"Starting samen version {SamenVersion}", LogType.INFO);

            Authentication.LoadFile();

            DateTime loadStartTime = DateTime.Now;

            Logging.Log("Startup", $"Loading Plugins...", LogType.INFO);
            PluginLoader.LoadPlugins();

            Logging.Log("Startup", $"Loading Network...", LogType.INFO);
            Server.Start(4041);
            ServerCLI.Begin();

            Logging.Log("Samen", $"Startup completed in {Math.Ceiling(DateTime.Now.Subtract(loadStartTime).TotalMilliseconds)}ms!", LogType.IMPORTANT);

            if (Authentication.AccountCount() == 0)
            {
                Thread.Sleep(100);
                Logging.Log("Samen", $"You seem to have no accounts setup, do 'AddUser [username] [password]' to add one!", LogType.IMPORTANT);
            }

            while (true)
            {
                Server.Update();
            }
        }
    }
}
