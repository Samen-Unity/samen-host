﻿using SamenHost.Internet;
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
        private UserAccount account;
        /// <summary>
        /// Get the username of the user
        /// </summary>
        /// <returns></returns>
        public string GetUsername()
        {
            return account.username;
        }
        

        /// <summary>
        /// Create a user instance from a connection
        /// </summary>
        /// <param name="connection"></param>
        public User(Connection connection)
        {
            this.connection = connection;

            // Listen for authentication
            connection.Listen(PacketType.Authenticate, (packet) =>
            {
                string username = packet.GetString(0);
                string password = packet.GetString(1);

                UserAccount validAccount = Authentication.GetAccount(username, password);
                connection.SendPacket(new OutgoingPacket(PacketType.Authenticate).WriteBool(validAccount != null));

                if (validAccount != null)
                {
                    account = validAccount;
                    Logging.Log("USER", $"{GetUsername()} connected to the server.", LogType.INFO); 
                    Server.OnUserConnect?.Invoke(this);
                    GiveAccess();
                }
                else
                {
                    // Garbage collection will get your ass
                    connection.Dead = true;
                }
            });
        }

        /// <summary>
        /// Check if the user has a spesific permission
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public bool HasPermission(string permission)
        {
            return account.permissions.Contains(permission);
        }

        private void GiveAccess()
        {
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
                SessionManager.RegisterSession(session);

                Logging.Log("SESSION", $"{GetUsername()} created a session for {session.GetAssetPath()}", LogType.IMPORTANT);
            });

            connection.Listen(PacketType.Ping, (packet) =>
            {
                this.GetSession().BroadcastPacket(new OutgoingPacket(PacketType.Ping)
                    .WriteFloat(packet.GetFloat(0))
                    .WriteFloat(packet.GetFloat(1))
                    .WriteFloat(packet.GetFloat(2))
                    );
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
                foreach (Session s in SessionManager.GetSessions())
                {
                    s.GetUsers().Remove(this);
                }

                session.GetUsers().Add(this);
                Logging.Log("SESSION", $"{GetUsername()} has entered session '{session.GetAssetPath()}'", LogType.INFO);
                session.GetChat().SendMessage(GetUsername() + " has entered the session.");
            });


            // Listen for request sync
            connection.Listen(PacketType.RequestSync, (packet) =>
            {
                // Send a debug message
                Logging.Log("SESSION", $"Send {GetUsername()} {this.GetSession().GetAllHistory().Count} changes to catch up.", LogType.INFO);

                // Send all the history
                this.GetSession().SendAllHistory(GetConnection());
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
                TransformChangeHistory change = new TransformChangeHistory(this, packet.GetString(0), (TransformChangeType)packet.GetInt(1), values);
                this.GetSession().WriteHistory(change);
            });

            connection.Listen(PacketType.ObjectDuplicated, (packet) =>
            {
                ObjectDuplicatedHistory objectDuplicatedHistory = new ObjectDuplicatedHistory(this, packet.GetString(0), packet.GetString(1));
                this.GetSession().WriteHistory(objectDuplicatedHistory);
            });

            // Listen for object destroyed
            connection.Listen(PacketType.ObjectDestroyed, (packet) =>
            {
                // Create a new history entry
                ObjectDestroyedHistory objectDestroyedHistory = new ObjectDestroyedHistory(this, packet.GetString(0));
                this.GetSession().WriteHistory(objectDestroyedHistory);
            });

            connection.Listen(PacketType.ParentChange, (packet) =>
            {
                // Create a new history entry
                ParentChangeHistory parentChangeHistory = new ParentChangeHistory(this, packet.GetString(0), packet.GetString(1));
                this.GetSession().WriteHistory(parentChangeHistory);
            });

            // Listen for chat messages
            connection.Listen(PacketType.ChatMessage, (packet) =>
            {
                this.GetSession().GetChat().SendMessage(GetUsername(), packet.GetString(0));
            });

            connection.Listen(PacketType.PrefabCreated, (packet) =>
            {
                string assetPath = packet.GetString(0);
                int idCount = packet.GetInt(1);

                string[] ids = new string[idCount];
                for (int i = 0; i < idCount; i++)
                {
                    ids[i] = packet.GetString(2 + i);
                }

                PrefabCreatedHistory prefabCreatedHistory = new PrefabCreatedHistory(this, assetPath, ids);
                this.GetSession().WriteHistory(prefabCreatedHistory);
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
