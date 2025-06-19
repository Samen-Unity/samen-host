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

        private List<User> users = new List<User>();
        private List<History> history = new List<History>();

        public List<User> GetUsers()
        {
            return users;
        }

        public void AddHistory(History history)
        {
            OptimizeHistory(history);
            this.history.Add(history);
        }

        public void SendHistory(Connection connection)
        {
            foreach (History history in GetHistory())
            {
                history.SendAsPacket(connection);
            }
        }

        public void OptimizeHistory(History next)
        {
            if (history.Count == 0)
                return;

            History last = history.Last();

            if (next.GetType() == typeof(TransformChangeHistory))
            {
                if (last.GetType() == next.GetType())
                {
                    TransformChangeHistory transformLast = (TransformChangeHistory) last;
                    TransformChangeHistory transformNext = (TransformChangeHistory) next;

                    if(transformLast.GetObjectId() == transformNext.GetObjectId() && transformLast.GetTransformChangeType() == transformNext.GetTransformChangeType())
                    {
                        history.Remove(last);
                    }
                }
            }
        }

        public List<History> GetHistory()
        {
            return history;
        }
    }
}
