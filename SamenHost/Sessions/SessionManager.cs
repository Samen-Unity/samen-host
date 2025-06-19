using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamenHost.Sessions
{
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
        /// Add a session to the manager.
        /// </summary>
        /// <param name="session"></param>
        public static void AddSession(Session session)
        {
            sessions.Add(session.GetAssetPath(), session);
        }

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
