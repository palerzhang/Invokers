using System.Collections;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System;

namespace Anonymous
{
    namespace Network
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
            private Thread loop_thread;
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
            private Socket server;
            /// <summary>
            /// end point
            /// </summary>
            private IPEndPoint end_point;
                        
            public NetServer()
            {
                IPHostEntry ip_host_info = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress address = ip_host_info.AddressList[0];
                end_point = new IPEndPoint(address, port);
                server = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
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
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            private void ServerLoop()
            {
                try
                {
                    while (true)
                    {
                        listener.BeginAccept(
                       new AsyncCallback(AcceptCallback),
                       listener);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    } // end network
} // end anonymous
