﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
namespace SamenHost
{


    /// <summary>
    /// Types of packets
    /// </summary>
    public enum PacketType
    {
        /// <summary>
        /// Authentication packet for username and accept
        /// </summary>
        Authenticate,

        /// <summary>
        /// A packet to check if a spesific session exists
        /// </summary>
        SessionExists,

        /// <summary>
        /// A packet to create a session
        /// </summary>
        CreateSession,

        /// <summary>
        /// A packet to join a session
        /// </summary>
        /// 
        JoinSession,

        /// <summary>
        /// A packet send whenever an object transform was changed
        /// </summary>
        ObjectChange,

        /// <summary>
        /// A packet send whenever the client is ready to receive a session's history
        /// </summary>
        RequestSync,

        /// <summary>
        /// A packet send whenever an object was destroyed
        /// </summary>
        ObjectDestroyed,

        /// <summary>
        /// A packet send whenever an object was duplicated
        /// </summary>
        ObjectDuplicated,

        /// <summary>
        /// A packet for chat messages
        /// </summary>
        ChatMessage,

        /// <summary>
        /// A parent of an object was changed.
        /// </summary>
        ParentChange,

        /// <summary>
        /// A prefab was added into the scene
        /// </summary>
        PrefabCreated,

        /// <summary>
        /// A packet for a user-ping
        /// </summary>
        Ping
    }
    
    /// <summary>
    /// An connection with a user
    /// </summary>
    public class Connection
    {
        private TcpClient client;

        /// <summary>
        /// The ip of the connection
        /// </summary>
        /// <returns></returns>
        public string GetRemoteIp()
        {
            return client.Client.RemoteEndPoint.ToString();
        }

        /// <summary>
        /// Create a managed connection from a TCP client
        /// </summary>
        /// <param name="client"></param>
        public Connection(TcpClient client)
        {
            this.client = client;
        }
        /// <summary>
        /// Add a length header to the packet and
        /// Send a packet to the other connection.
        /// </summary>
        /// <param name="outgoingPacket"></param>
        public void SendPacket(OutgoingPacket outgoingPacket)
        {
            if (Dead)
                return;

            byte[] data = outgoingPacket.GetBytes();
            byte[] size = BitConverter.GetBytes(data.Length);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(size);

            try
            {
                client.GetStream().Write(size, 0, 4);
                client.GetStream().Write(data, 0, data.Length);
            } catch
            {
                Dead = true;
            }
        }

        /// <summary>
        /// If the connection has ended
        /// </summary>
        public bool Dead;

        private int expected = -1;

        /// <summary>
        /// Reads any incoming packets. This should be called in your update loop!
        /// </summary>
        /// <param name="maxRead">The maximum amount of packets to read before stopping.</param>
        public void ReadPackets(int maxRead = 1000)
        {
            if (Dead)
                return;
         
            int read = 0;
            while (read < maxRead)
            {
                if (Dead)
                    return;

                if (expected == -1 && client.Available >= 4)
                {
                    byte[] buffer = new byte[4];
                    client.GetStream().Read(buffer, 0, buffer.Length);

                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(buffer);

                    expected = BitConverter.ToInt32(buffer, 0);
                }
                else if (expected != -1 && client.Available >= expected)
                {
                    byte[] packet = new byte[(int)expected];
                    client.GetStream().Read(packet, 0, packet.Length);

                    IncomingPacket incomingPacket = IncomingPacket.FromBuffer(packet);
                    expected = -1;;

                    Call(incomingPacket);

                    if (Dead)
                        return;
                }
                else return;

                read++;
            }
        }


        /// <summary>
        /// Executes all functions listening to this packet
        /// </summary>
        /// <param name="incomingPacket"></param>
        public void Call(IncomingPacket incomingPacket)
        {
            foreach (IncomingPacketListener listener in listeners.ToList())
            {
                if (listener.type == incomingPacket.type)
                {
                    listener.@delegate.Invoke(incomingPacket);
                }
            }
        }

        private List<IncomingPacketListener> listeners = new List<IncomingPacketListener>();

        /// <summary>
        /// Register a new packet listener
        /// </summary>
        /// <param name="type">The packet type to listen for</param>
        /// <param name="action">The function that should be ran whenever a packet was received</param>
        public void Listen(PacketType type, Action<IncomingPacket> action)
        {
            if (action == null)
            {
                throw new Exception("No action was provided.");
            }

            listeners.Add(new IncomingPacketListener(type, action));
        }
    }

    class IncomingPacketListener
    {
        public PacketType type;
        public Action<IncomingPacket> @delegate;

        public IncomingPacketListener(PacketType type, Action<IncomingPacket> @delegate)
        {
            this.type = type;
            this.@delegate = @delegate;
        }
    }

    /// <summary>
    /// A packet from the client
    /// </summary>
    public class IncomingPacket
    {
        /// <summary>
        /// Create an IncomingPacket from a byte array.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static IncomingPacket FromBuffer(byte[] buffer)
        {
            byte[] typeBuffer = new byte[4];
            Array.Copy(buffer, typeBuffer, 4);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(typeBuffer);

            int type = BitConverter.ToInt32(typeBuffer, 0);

            IncomingPacket incomingPacket = new IncomingPacket((PacketType)type);
            int index = 4;

            while (index < buffer.Length)
            {
                byte[] sizeBuffer = new byte[4];
                Array.Copy(buffer, index, sizeBuffer, 0, 4);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(sizeBuffer);

                index += 4;
                int size = BitConverter.ToInt32(sizeBuffer, 0);

                byte[] contentBuffer = new byte[size];
                Array.Copy(buffer, index, contentBuffer, 0, size);

                incomingPacket.AddData(contentBuffer);

                index += contentBuffer.Length;
            }

            return incomingPacket;
        }

        /// <summary>
        /// The type of the incoming packet
        /// </summary>
        public PacketType type;

        /// <summary>
        /// A packet received from the client
        /// </summary>
        /// <param name="type"></param>
        public IncomingPacket(PacketType type)
        {
            this.type = type;
            data = new List<byte[]>();
        }

        private List<byte[]> data;

        /// <summary>
        /// Internal use only.
        /// </summary>
        /// <param name="buffer"></param>
        protected void AddData(byte[] buffer)
        {
            data.Add(buffer);
        }

        /// <summary>
        /// Get an int from a specific field
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public int GetInt(int field)
        {
            byte[] buffer = data[field];
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);

            return BitConverter.ToInt32(buffer, 0);
        }



        /// <summary>
        /// Get an string from a specific field
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public string GetString(int field)
        {
            byte[] buffer = data[field];
            return Encoding.UTF8.GetString(buffer);
        }

        /// <summary>
        /// Get a float from a spesific field
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public float GetFloat(int field)
        {
            byte[] buffer = data[field];
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);

            return BitConverter.ToSingle(buffer, 0);
        }

        /// <summary>
        /// Get an byte from a specific field
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public byte GetByte(int field)
        {
            byte[] buffer = data[field];
            return buffer[0];
        }

        /// <summary>
        /// Get an bool from a specific field
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool GetBool(int field)
        {
            byte[] buffer = data[field];

            byte b = buffer[0];

            return b == 0x01;
        }
    }

    /// <summary>
    /// A packet send to the client
    /// </summary>
    public class OutgoingPacket
    {

        /// <summary>
        /// The type of the packet
        /// </summary>
        public PacketType packetType { private set; get; }
        /// <summary>
        /// Create a packet of a spesific type
        /// </summary>
        /// <param name="packetType"></param>
        public OutgoingPacket(PacketType packetType)
        {
            this.packetType = packetType;
            int type = (int)packetType;

            byte[] buffer = new byte[4];
            buffer = BitConverter.GetBytes(type);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);

            stream = new MemoryStream();
            stream.Write(buffer, 0, buffer.Length);
        }

        MemoryStream stream;

        /// <summary>
        /// Write an int without a length header.
        /// </summary>
        /// <param name="val"></param>
        public OutgoingPacket WriteIntRaw(int val)
        {
            byte[] buffer = new byte[4];
            buffer = BitConverter.GetBytes(val);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);


            stream.Write(buffer, 0, buffer.Length);
            return this;
        }

        /// <summary>
        /// Write a float to the packet
        /// </summary>
        /// <param name="val"></param>
        /// <returns>The current packet for building</returns>
        public OutgoingPacket WriteFloat(float val)
        {
            WriteIntRaw(4);
            byte[] buffer = new byte[4];
            buffer = BitConverter.GetBytes(val);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);


            stream.Write(buffer, 0, 4);
            return this;
        }

        /// <summary>
        /// Create a new field and write val to it.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public OutgoingPacket WriteInt(int val)
        {
            WriteIntRaw(4);
            WriteIntRaw(val);

            return this;
        }

        /// <summary>
        /// Create a new field and write val to it.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public OutgoingPacket WriteString(string val)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(val);

            WriteIntRaw(buffer.Length);
            stream.Write(buffer, 0, buffer.Length);


            return this;
        }

        /// <summary>
        /// Create a new field and write val to it.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public OutgoingPacket WriteByte(byte val)
        {
            WriteIntRaw(1);
            stream.WriteByte(val);

            return this;
        }

        /// <summary>
        /// Create a new field and write val to it.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public OutgoingPacket WriteBool(bool val)
        {
            byte a = 0x00;

            if (val)
            {
                a = 0x01;
            }

            WriteByte(a);

            return this;
        }

        /// <summary>
        /// Returns a formatted packet.
        /// Without its lenght header.
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            byte[] buffer = stream.ToArray();

            return buffer;
        }
    }
}