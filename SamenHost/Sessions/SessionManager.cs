using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamenHost.Sessions
{
    /// <summary>
    /// Manager that handles all active sessions
    /// </summary>
    public class SessionManager
    {
        /// <summary>
        /// All sessions
        /// </summary>
        private static Dictionary<string, Session> sessions = new Dictionary<string, Session>();


        /// <summary>
        /// Check if a session exists from a specific scene path
        /// </summary>
        /// <param name="path">The path of the scene on the client side.</param>
        /// <returns></returns>
        public static bool SessionExists(string path)
        {
            bool exists = sessions.ContainsKey(path);
            return exists;
        }

        /// <summary>
        /// Return all sessions
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, Session>.ValueCollection GetSessions()
        {
            return sessions.Values;
        }

        /// <summary>
        /// Registers a session to the manager.
        /// </summary>
        /// <param name="session"></param>
        public static void RegisterSession(Session session)
        {
            sessions.Add(session.GetAssetPath(), session);
            OnSessionAdded?.Invoke(session);
        }

        /// <summary>
        /// Called whenever a new session is created
        /// </summary>
        public static Action<Session> OnSessionAdded;

        /// <summary>
        /// Get a session from a specific scene file path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Session GetSession(string path)
        {
            return sessions[path];
        }
    }
}
