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
    /// <summary>
    /// The Server that handles all incoming connections
    /// </summary>
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
                Logging.Log("Network", "Closing old server...", LogType.IMPORTANT);
                listener.Stop();
                listener = null;
            }

            Logging.Log("Network", $"Starting server on port {port}...", LogType.INFO);
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
