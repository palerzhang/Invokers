using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System;

using UnityEngine;

namespace NetworkService.NetworkMessage
{
    public class NetServer
    {
        /// <summary>
        /// message queues
        /// </summary>
        public Queue msg_from_client;
        public Queue msg_to_client;
        /// <summary>
        /// looping
        /// </summary>
        private Thread loop_thread = null;
        /// <summary>
        /// buffer size
        /// </summary>
        const int buffer_size = 2048;
        /// <summary>
        /// port
        /// </summary>
        const int port = 14932;
        /// <summary>
        /// server
        /// </summary>
        private Socket server = null;
        /// <summary>
        /// end point
        /// </summary>
        private IPEndPoint end_point;

        private HashSet<Socket> clients;

        public NetServer()
        {
            IPHostEntry ip_host_info = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress address = ip_host_info.AddressList[0];
            end_point = new IPEndPoint(address, port);
            server = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        ~NetServer()
        {
            Shutdown();
        }

        public void Shutdown()
        {
            if (server != null)
                server.Close();

            if (loop_thread != null && loop_thread.IsAlive)
                loop_thread.Abort();
        }
        /// <summary>
        /// start server
        /// </summary>
        public void StartServer()
        {
            try
            {
                if (!server.IsBound)
                {
                    server.Bind(end_point);
                    server.Listen(10);

                    loop_thread = new Thread(ServerLoop);
                    loop_thread.IsBackground = true;
                    loop_thread.Start();

                    Debug.Log("server started.");
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        private void ServerLoop()
        {
            try
            {
                while (true)
                {
                    Debug.Log("wait connection");
                    // try start accept
                    server.BeginAccept(new AsyncCallback(AcceptCallback), server);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        protected void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            //allDone.Set();

            // Get the socket that handles the client request.  
            Socket the_server = (Socket)ar.AsyncState;
            if (the_server == server)
            {
                int x = 10;
            }
            Socket client = the_server.EndAccept(ar);
            if (!clients.Contains(client))
            {
                clients.Add(client);
            }

            Debug.Log(client.AddressFamily.ToString() + " is connect.");

            //// Create the state object.  
            //StateObject state = new StateObject();
            //state.workSocket = handler;
            //handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
            //    new AsyncCallback(ReadCallback), state);
        }
    }
}
