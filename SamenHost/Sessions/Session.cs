using SamenHost.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamenHost.Sessions
{
    public class Session
    {

        /// <summary>
        /// The path to the scene file.
        /// </summary>
        private string assetPath;

        /// <summary>
        /// The data that was part of the scene when the session was created.
        /// </summary>
        private string initialSceneData;

        /// <summary>
        /// Returns the path to the scene file on the client side. 
        /// Also used as an ID.
        /// </summary>
        /// <returns></returns>
        public string GetAssetPath()
        {
            return assetPath;
        }

        /// <summary>
        /// Returns the data that was part of the scene when the session was created.
        /// </summary>
        /// <returns></returns>
        public string GetInitialSceneData()
        {
            return initialSceneData;
        }

        /// <summary>
        /// Create a session.
        /// </summary>
        /// <param name="assetPath">The path to the scene file on the client (Also the ID)</param>
        /// <param name="initialSceneData">The data that was part of the initial scene</param>
        public Session(string assetPath, string initialSceneData)
        {
            this.assetPath = assetPath;
            this.initialSceneData = initialSceneData;
        }

        /// <summary>
        /// List of users in the current session
        /// </summary>
        private List<User> users = new List<User>();

        /// <summary>
        /// List of history of the current session
        /// </summary>
        private List<History> history = new List<History>();

        /// <summary>
        /// Get a list of all users in this session
        /// </summary>
        /// <returns></returns>
        public List<User> GetUsers()
        {
            return users;
        }

        /// <summary>
        /// Add history to this session
        /// This does NOT broadcast it to the users
        /// But any new users WILL receive it.
        /// </summary>
        /// <param name="history"></param>
        public void RememberHistory(History history)
        {
            OptimizeHistory(history);
            this.history.Add(history);
        }

        /// <summary>
        /// Add the history to the session, AND broadcast the history to everyone.
        /// </summary>
        /// <param name="history"></param>
        /// <param name="excludeAuthor"></param>
        public void WriteHistory(History history, bool excludeAuthor = true)
        {
            RememberHistory(history);
            history.Broadcast(this, excludeAuthor);
        }


        public void SendAllHistory(Connection connection)
        {
            foreach (History history in GetAllHistory())
            {
                history.SendAsPacket(connection);
            }
        }


        /// <summary>
        /// Optimize the history, while keeping in mind what is coming next.
        /// </summary>
        /// <param name="next"></param>
        private void OptimizeHistory(History next)
        {

            // Future idea, When an object is destroyed in 'next' remove any changes done to it from history.

            if (history.Count == 0)
                return;

            History last = history.Last();

            if (next.GetType() == typeof(TransformChangeHistory))
            {
                if (last.GetType() == next.GetType())
                {
                    TransformChangeHistory transformLast = (TransformChangeHistory)last;
                    TransformChangeHistory transformNext = (TransformChangeHistory)next;

                    if (transformLast.GetObjectId() == transformNext.GetObjectId() && transformLast.GetTransformChangeType() == transformNext.GetTransformChangeType())
                    {
                        history.Remove(last);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a list of all the current history
        /// </summary>
        /// <returns></returns>
        public List<History> GetAllHistory()
        {
            return history;
        }
    }
}
