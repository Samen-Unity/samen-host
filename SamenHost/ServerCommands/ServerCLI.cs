using SamenHost.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SamenHost.ServerCommands
{
    internal class ServerCLI
    {
        static string space = "ServerCLI";
        System.Threading.Thread Thread;
        static ServerCLI instance = null;
        /// <summary>
        /// Create the ServerCLI instance
        /// </summary>
        internal static void Begin()
        {
            if (instance == null) instance = new ServerCLI();
        }
        /// <summary>
        /// Constructor for ServerCLI, creates and runs the thread that processes the messages
        /// </summary>
        private ServerCLI()
        {
            Thread = new System.Threading.Thread(new System.Threading.ThreadStart(this.Handler));
            Thread.Start();
        }
        /// <summary>
        /// Reads a line from the console, breaks it up and calls a method in this class to process the call
        /// </summary>
        private void Handler()
        {
            while (true)
            {
                string? input = Console.ReadLine();
                if (input != null)
                {
                    string[] parts = Regex.Matches(input, @"""[^""]+|\b\w+\b").Cast<Match>().Select(b=>b.Value.Trim('"')).ToArray();
                    System.Reflection.MethodInfo? method = this.GetType().GetMethod(parts[0]);

                    if (method != null)
                    {
                        object[] arguments = new object[parts.Length - 1];
                        for (int cx = 0; cx < parts.Length - 1; cx++)
                            arguments[cx] = parts[cx + 1];
                        try
                        {
                            method.Invoke(this, arguments);
                        }
                        catch (Exception e)
                        {
                            Logging.Log(space, e.Message, LogType.ERROR);
                        }
                    }
                    else
                        Logging.Log(space, "Command not found", LogType.ERROR);
                }
            }
        }
        #region Callable function from the command line

        /// <summary>
        /// Add a user
        /// </summary>
        /// <param name="name">name of the user</param>
        /// <param name="password">unencrypted password for the user</param>
        public void AddUser(string name, string password)
        {
            string pass = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password)));
            Internet.Authentication.AddAccount(name, pass);
        }
        /// <summary>
        /// Quit the program
        /// </summary>
        public void Quit()
        {
            Environment.Exit(0);
        }
        /// <summary>
        /// Broadcast a chat message to all sessions in this server
        /// </summary>
        /// <param name="message">The message to broadcast</param>
        public void Broadcast(string message)
        {
            Logging.Log(space, $"Broadcast: {message}", LogType.INFO);
            foreach (Session session in SessionManager.GetSessions())
                session.GetChat().SendMessage(message);
        }
        #endregion
    }
}
