using SamenHost.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SamenHost.Internet
{
    public class Server
    {
        /// <summary>
        /// Global instance of the tcp server
        /// </summary>
        static TcpListener listener;

        /// <summary>
        /// Start the server on a specific port.
        /// If the server was already running, restart the connection.
        /// </summary>
        /// <param name="port"></param>
        public static void Start(int port)
        {
            if(listener != null)
            {
                Console.WriteLine("Closing server...");
                listener.Stop();
                listener = null;
            }

            Console.WriteLine("Starting Server...");
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            connections = new List<Connection>();
        }

        /// <summary>
        /// Accept any pending connections
        /// </summary>
        internal static void AcceptIncomingConnection()
        {
            while (listener.Pending())
            {
                TcpClient tcpClient = listener.AcceptTcpClient();

                Connection connection = new Connection(tcpClient);
                User user = new User(connection);
                OnUserConnect?.Invoke(user);

                connections.Add(connection);
            }
        }

        /// <summary>
        /// List of all connections on the server
        /// </summary>
        private static List<Connection> connections = new List<Connection>();


        /// <summary>
        /// Accept incoming connections and read incoming packets.
        /// </summary>
        public static void Update()
        {
            AcceptIncomingConnection();
            foreach(Connection connection in connections)
            {
                connection.ReadPackets();
            }
        }

        /// <summary>
        /// Executes when a new user connects to the server.
        /// </summary>
        public static Action<User> OnUserConnect;
    }
}
