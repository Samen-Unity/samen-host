using SamenHost.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamenHost.Core
{
    /// <summary>
    /// A user is someone that is connect with the server.
    /// </summary>
    public class User
    {
        /// <summary>
        /// The username the user entered.
        /// </summary>
        public string Username;

        
        public User(Connection connection)
        {
            this.connection = connection;

            // Listen for authentication
            connection.Listen(PacketType.Authenticate, (packet) =>
            {
                Username = packet.GetString(0);
                Console.WriteLine("User authenticated as " + Username);

                // Response with a sucsess!
                connection.SendPacket(new OutgoingPacket(PacketType.Authenticate));
            });

            // Listen for session exists status
            connection.Listen(PacketType.SessionExists, (packet) =>
            {
                // Get the information about the requested session
                string assetName = packet.GetString(0);
                bool sessionExists = SessionManager.SessionExists(assetName);

                // Give the response
                connection.SendPacket(
                    new OutgoingPacket(PacketType.SessionExists)
                    .WriteBool(sessionExists)
                    );
            });

            // Listen for a create session request
            connection.Listen(PacketType.CreateSession, (packet) =>
            {
                // Create a new session
                Session session = new Session(
                    assetPath: packet.GetString(0),
                    initialSceneData: packet.GetString(1));

                // Register the session.
                SessionManager.AddSession(session);
            });

            // Listen for a join session
            connection.Listen(PacketType.JoinSession, (packet) =>
            {
                // The path to the scene file
                string sessionPath = packet.GetString(0);

                // Find the session 
                Session session = SessionManager.GetSession(sessionPath);

                // Send the initial scene file
                OutgoingPacket response = new OutgoingPacket(PacketType.JoinSession)
                    .WriteString(session.GetInitialSceneData());

                this.session = session;
                connection.SendPacket(response);

                // Assign the user to this new session
                foreach(Session s in SessionManager.GetSessions())
                {
                    s.GetUsers().Remove(this);
                }
                session.GetUsers().Add(this);
            });


            // Listen for request sync
            connection.Listen(PacketType.RequestSync, (packet) =>
            {
                // Send all the changes 
                Console.WriteLine("[Join] Catching up " + Username + " with " + this.GetSession().GetHistory().Count + " changes.");

                // Send all the history
                this.GetSession().SendHistory(GetConnection());
            });

            // Listen for a object change
            connection.Listen(PacketType.ObjectChange, (packet) =>
            {
                // Extract the changes data
                float[] values = new float[packet.GetInt(2)];
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = packet.GetFloat(i + 3);
                }

                // Create a new history entry
                TransformChangeHistory change = new TransformChangeHistory(this, packet.GetString(0), (TransformChangeType) packet.GetInt(1), values);
                this.GetSession().AddHistory(change);

                // Send the change to everyone
                change.Broadcast(this.GetSession(), true);
            });


            // Listen for object destroyed
            connection.Listen(PacketType.ObjectDestroyed, (packet) =>
            {
                // Create a new history entry
                ObjectDestroyedHistory objectDestroyedHistory = new ObjectDestroyedHistory(this, packet.GetString(0));
                this.GetSession().AddHistory(objectDestroyedHistory);

                // Send the change to everyone
                objectDestroyedHistory.Broadcast(this.GetSession(), true);
            });
        }

        private Connection connection;
        private Session session;

        /// <summary>
        /// Get the connection of the user
        /// </summary>
        /// <returns></returns>
        public Connection GetConnection()
        {
            return connection;
        }

        /// <summary>
        /// Get the session of the user
        /// </summary>
        /// <returns></returns>
        public Session GetSession()
        {
            return session;
        }
    }
}
