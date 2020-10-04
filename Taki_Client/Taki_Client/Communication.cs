using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.Http.Headers;

namespace sockets
{
    class Program
    {
        public string CommandHandler(string action, object[] args)
        {
            /*
             * Autor: Yuval Didi
             * About The Func:
             *  this func get the action of the bot and the required arguments
             *  and returns the message according to the protocol
             */
            return action;
        }
        public object DataAnalyzing(string action, string recv_str)
        {
            /*
             * Autor: Yuval Didi
             * About The Func:
             *  this func analyize the server's response
             */
            return action;
        }
        public object GameHandler(Socket serverSock, string action, object[] args)
        {
            /*
             * Autor: Yuval Didi
             * About The Func:
             *  this func manage the communication with the server
             */
            try
            {
                if (action == "CLOSE")
                {
                    serverSock.Shutdown(SocketShutdown.Both);
                    serverSock.Close();
                    return 0;
                }

                byte[] msg_bytes = Encoding.ASCII.GetBytes(CommandHandler(action, args));
                byte[] recv_bytes = new byte[1024];

                string recv_str;

                //send the data
                int bytesSent = serverSock.Send(msg_bytes);
                Console.WriteLine("[Communication] Sent\n{0}", msg_bytes.ToString());
                Console.WriteLine("[Communication] Waiting For Response...");

                //receiving the response from the server
                int bytesRec = serverSock.Receive(recv_bytes);
                recv_str = Encoding.ASCII.GetString(recv_bytes, 0, bytesRec);

                return DataAnalyzing(action, recv_str);
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                return 0;
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
                return 0;
            }


        }
        public static Socket StartClient()
        {
            /*
             * Autor: Yuval Didi
             * About The Func:
             *  this func return the socket obj
             *  of the server.
             * 
             */
            byte[] bytes = new byte[1024];

            try
            {
                // Establish the remote endpoint for the socket.  
                // This example uses port 11000 on the local computer.
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                //Create a TCP/IP socket
                Socket serverSock = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                //connect to the server
                try
                {
                    serverSock.Connect(remoteEP);
                    Console.WriteLine("[Communication] - connected\n{0}",
                        serverSock.RemoteEndPoint.ToString());
                    return serverSock;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return null;
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Amung Us?");
        }
    }
}
