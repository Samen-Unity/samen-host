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
        public static int SamenVersion = 101;
        public static Configuration configuration;
        static void Main(string[] args)
        {
            DateTime loadStartTime = DateTime.Now;

            Logging.Log("Samen", $"Loading configuration...", LogType.INFO);
            configuration = Configuration.LoadAndSaveDefaults("config.json");

            Logging.Log("Samen", $"Starting samen version {SamenVersion}", LogType.INFO);
            BuildInCommands.Register();

            if (configuration.EnableAuthentication ?? true)
            {
                Authentication.LoadFile();
            }
            else
            {
                Logging.Log("Authentication", $"You do not have authentication enabled.", LogType.IMPORTANT);
                Logging.Log("Authentication", "Anyone on your network can enter, edit and corrupt your session.", LogType.IMPORTANT);
                Logging.Log("Authentication", "Please be carefull!", LogType.IMPORTANT);
            }

            Logging.Log("Startup", $"Loading Plugins...", LogType.INFO);
            PluginLoader.LoadPlugins();

            Logging.Log("Startup", $"Loading Network...", LogType.INFO);
            Server.Start(configuration.ServerPort ?? 4041);
            ServerCLI.Begin();

            Logging.Log("Samen", $"Startup completed in {Math.Ceiling(DateTime.Now.Subtract(loadStartTime).TotalMilliseconds)}ms!", LogType.IMPORTANT);

            if (Authentication.AccountCount() == 0 && (configuration.EnableAuthentication ?? true))
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
