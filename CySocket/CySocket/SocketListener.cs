﻿using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CySocket
{

    public class SocketListener
    {

        //the incoming data from the client
        public static string data = null;
        /// <summary>
        /// Synchronously starts the socket listener
        /// </summary>        
        static bool start = true;
        public static void Start()
        {
            Start(SocketType.Stream, ProtocolType.Tcp);
        }
        private static void Start(SocketType socketType, ProtocolType protocolType)
        {
            //data buffer
            byte[] bytes = new byte[1024];
            //establish the local endpoint for the socket
            //dns.GetHostName returns the name of the host running the app
            IPHostEntry iPHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            //IPAddress ipAddress = iPHostEntry.AddressList.Where(x => x.ToString().StartsWith("10.").);
            //IPAddress ipAddress = iPHostEntry.AddressList[0];//.Where(x => x.ToString().StartsWith("10.").);
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");//ipHostInfo.AddressList[0];
            IPEndPoint localEndpoint = new IPEndPoint(ipAddress, 8080);
            Console.WriteLine($"endpoint: {localEndpoint.ToString()}");
            //create the tcp/ip socket
            Socket listener = new Socket(ipAddress.AddressFamily, socketType, protocolType);

            // bind the socket to the local endpoint and listen for incoming connections
            try
            {
                listener.Bind(localEndpoint);
                listener.Listen(10);

                //start listening for connections

                while(start == true)
                {
                    Console.WriteLine("Awaiting Connection... ");
                    Socket handler = listener.Accept();

                    data = null;

                    // an incoming connection needs to be processed
                    while(true)
                    {
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);

                        if (data.IndexOf("<EOF>") > -1)
                            break;
                    }

                    //show the data on the console
                    Console.WriteLine("Text received : {0}", data);

                    byte[] msg = Encoding.ASCII.GetBytes(data);

                    handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();

                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine("\nPress any key to continue... ");
            Console.Read();
        }

    }
}
