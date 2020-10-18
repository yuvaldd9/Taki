
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;


namespace sockets
{

    class CreatedGame
    {
        public string lobby_name { get; set; }
        public string player_name { get; set; }
        public string password { get; set; }
    }
    class Game
    {
        public string game_id { get; set; }
        public string player_name { get; set; }
        public string password { get; set; }
    }
    class Json
    {
        public string code { get; set; }
        public object args { get; set; }
    }
    class JsonWithJwt : Json
    {
        public string jwt { get; set; }
    }
    class Turn
    {
        public Card[] cards { get; set; }
    }

    class Program
    {

        public static Card[] strToCardArray(string Carsstr)
        {
            Card[] a = new Card[2];
            return a;
        }
        public static string CommandHandler(string action, string[] args)
        {
            /*
             * Autor: Yuval Didi
             * About The Func:
             *  this func get the action of the bot and the required arguments
             *  and returns the message according to the protocol
             */


            string json = "";
            Json jsonObj;
            switch (action)
            {
                case "create_game":
                    CreatedGame createdGame = new CreatedGame()
                    {

                        lobby_name = args[0],
                        player_name = args[1],
                        password = args[2]
                    };
                    jsonObj = new Json()
                    {
                        code = action,
                        args = createdGame
                    };
                    break;
                case "join_game":
                    Game game = new Game()
                    {

                        game_id = args[0],
                        player_name = args[1],
                        password = args[2]
                    };
                    jsonObj = new Json()
                    {
                        code = action,
                        args = game
                    };
                    break;
                case "place_cards":
                    Turn turn = new Turn()
                    {
                        //we should think about something here
                        cards = strToCardArray(args[1])
                    };
                    jsonObj = new JsonWithJwt()
                    {
                        code = action,
                        args = turn,
                        jwt = args[0]
                    };
                    break;
                default:
                    // when the action is: leave_game or start_game
                    jsonObj = new JsonWithJwt()
                    {
                        code = action,
                        args = null,
                        jwt = args[0]
                    };
                    break;
            }
            json = JsonConvert.SerializeObject(jsonObj);
            return json;

        }
        public object[] DataAnalyzing(string action, string recv_str)
        {
            /*
             * Autor: Yuval Didi
             * About The Func:
             *  this func analyize the server's response
             */

            dynamic respondJson = JsonConvert.DeserializeObject(recv_str);
            string[] jsonArgs = new string[2];
            jsonArgs[0] = respondJson.code;
            jsonArgs[1] = respondJson.args;
            return jsonArgs;
        }
        public object GameHandler(Socket serverSock, string action, string[] args)
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
                else if (action != "NOT MY TURN")
                {
                    //send the data
                    byte[] msg_bytes = Encoding.ASCII.GetBytes(CommandHandler(action, args));
                    int bytesSent = serverSock.Send(msg_bytes);
                    Console.WriteLine("[Communication] Sent\n{0}", msg_bytes.ToString());
                    Console.WriteLine("[Communication] Waiting For Response...");
                }


                //receiving the response from the server
                string recv_str;
                byte[] recv_bytes = new byte[1024];
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
            string action = "create";
            string[] args1 = new string[] { "name", "palyer", "3" };
            Console.WriteLine(CommandHandler(action, args1));
            Console.WriteLine("Amung Us?");
        }
    }
}
