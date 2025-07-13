using SamenHost.Core;
using SamenHost.Internet;
using SamenHost.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            Logging.Log("Samen", $"Starting samen version {SamenVersion}", LogType.INFO);

            DateTime loadStartTime = DateTime.Now;

            Logging.Log("Startup", $"Loading Plugins...", LogType.INFO);
            PluginLoader.LoadPlugins();

            Logging.Log("Startup", $"Loading Network...", LogType.INFO);
            Server.Start(4041);

            Logging.Log("Samen", $"Startup completed in {Math.Ceiling(DateTime.Now.Subtract(loadStartTime).TotalMilliseconds)}ms!", LogType.IMPORTANT);

            Console.CancelKeyPress += (sender, e) =>
            {
                Console.Clear();
                Console.WriteLine("!!! Stopping Samen will remove all session data !!!");
                Console.WriteLine("!!! MAKE SURE YOU HAVE YOUR SCENES EXPORTED !!!");

                Console.WriteLine("");
                Console.WriteLine("Are you sure? Y/N (N)");

                var key = Console.ReadKey();
                if (key.Key != ConsoleKey.Y)
                {
                    e.Cancel = true;
                    Console.Clear();
                    Logging.Log("CLOSE", "Prevented shutdown", LogType.IMPORTANT);
                }
                else
                {
                    Console.Clear();
                    Logging.Log("CLOSE", "Goodbye!", LogType.IMPORTANT);
                    Environment.Exit(-1);
                }
            };

            while (true)
            {
                Server.Update();
            }
        }
    }
}
