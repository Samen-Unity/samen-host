using SamenHost.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamenHost.Core
{
    /// <summary>
    /// TransformChangeHistory is a change done to a specific object.
    /// This can be the object's rotation, position or scale.
    /// </summary>
    public class TransformChangeHistory : History
    {

        /// <summary>
        /// The object that was edited
        /// </summary>
        private string objectId;

        /// <summary>
        /// The type of change that was done
        /// </summary>
        private TransformChangeType type;

        /// <summary>
        /// Argumentative values.
        /// Length of 4 for type  rotation
        /// Otherwise 3
        /// </summary>
        public float[] values;

        /// <summary>
        /// A change made in the transform of a specific object
        /// </summary>
        /// <param name="author">The user that made the change</param>
        /// <param name="objectId">The object that was changed</param>
        /// <param name="type">The type of change</param>
        /// <param name="values">Arguments, 
        /// Length of 4 for type rotation
        /// Otherwise 3.</param></param>
        public TransformChangeHistory(User author, string objectId, TransformChangeType type, float[] values)
        {
            this.author = author;
            this.objectId = objectId;
            this.type = type;
            this.values = values;
        }

        /// <summary>
        /// Send this transform change as a packet to a specific connection.
        /// </summary>
        /// <param name="connection"></param>
        public override void SendAsPacket(Connection connection)
        {
            OutgoingPacket packet = new OutgoingPacket(PacketType.ObjectChange);

            packet
                .WriteString(objectId) // The id of the object
                .WriteInt((int)type) // The type of change (cast as string)
                .WriteInt((int)values.Length); // The amount of arguments

            foreach (float value in values)
            {
                packet.WriteFloat(value); // The arguments
            }

            connection.SendPacket(packet);
        }

        /// <summary>
        /// Get the id of the object that was changed
        /// </summary>
        /// <returns></returns>
        public string GetObjectId()
        {
            return objectId;
        }

        /// <summary>
        /// Returns the type of change.
        /// </summary>
        /// <returns></returns>
        public TransformChangeType GetTransformChangeType()
        {
            return type;
        }
    }

    /// <summary>
    /// A type of transform change
    /// </summary>
    public enum TransformChangeType
    {
        Position,
        Rotation,
        Scale
    }
}
