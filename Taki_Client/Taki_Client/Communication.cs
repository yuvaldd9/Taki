
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CSharp.RuntimeBinder;
using System.Text.RegularExpressions;


namespace Taki_Client
{

    class CreatedGame
    {
        public string jwt { get; set; }
        public string lobby_name { get; set; }
        public string player_name { get; set; }
        public string password { get; set; }
    }
    class Game
    {
        public int game_id { get; set; }
        public string player_name { get; set; }
        public string password { get; set; }
    }

    class Json
    {


        public string code { get; set; }
        public object args { get; set; }
    }
    class Turn
    {
        public string jwt { get; set; }
        public Card[] cards { get; set; }
    }

    class token
    {
        public string jwt { get; set; }
    }
    class Communication
    {

        public static string CommandHandler(string action, object[] args)
        {
            /*
             * Autor: Yuval Didi
             * About The Func:
             *  this func get the action of the bot and the required arguments
             *  and returns the message according to the protocol
             */


            Json jsonObj;
            switch (action)
            {
                case "create_game":
                    CreatedGame createdGame = new CreatedGame()
                    {

                        lobby_name = (string)args[0],
                        player_name = (string)args[1],
                        password = (string)args[2]
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

                        game_id = int.Parse((string)args[0]),
                        player_name = (string)args[1],
                        password = (string)args[2]
                    };
                    jsonObj = new Json()
                    {
                        code = action,
                        args = game
                    };
                    break;
                case "start_game":
                    jsonObj = new Json()
                    {
                        code = action,
                        args = new token() { jwt = (string)args[0] }
                    };
                    break;
                case "place_cards":
                    Turn turn = new Turn()
                    {

                        cards = (Card[])args[0],
                        jwt = (string)args[1]
                    };
                    jsonObj = new Json()
                    {
                        code = action,
                        args = turn
                    };
                    break;
                case "leave_game":
                    jsonObj = new Json()
                    {
                        code = action,
                        args = new token() { jwt = (string)args[0] }
                    };
                    break;
                case "take_cards":
                    jsonObj = new Json()
                    {
                        code = action,
                        args = new token() { jwt = (string)args[0] }
                    };
                    break;
                default:
                    // when the action is: leave_game or start_game
                    jsonObj = new Json()
                    {
                        code = action,
                        args = null
                    };
                    break;
            }

            return JsonConvert.SerializeObject(jsonObj);

        }

        public static string[] JsonAnalyzer(string data)
        {
            dynamic json = JsonConvert.DeserializeObject(data);
            string[] response = new string[2];
            if (json.code != null)
                response[0] = (string)json.code;
            else
                response[0] = (string)json.status;
            if (json.args != null)
                response[1] = json.args.ToString();
            else
                response[1] = "";
            return response;
        }
        public static string[] DataAnalyzing(string action, string message)
        {
            /*
             * Autor: Yuval Didi
             * About The Func:
             *  this func analyize the server's response
             */

            List<string> responses = new List<string>();
            int counter = 0;
            int lastIndex = 0;
            bool isName = false;
            for (int i = 0; i < message.Length; i++)
            {
                if (message[i] == '\"')
                    isName = !isName;
                if (isName)
                    continue;
                if (message[i] == '{')
                    counter += 1;
                if (message[i] == '}')
                    counter -= 1;
                if (counter == 0)
                {
                    responses.Add(message.Substring(lastIndex, i - lastIndex + 1));
                    lastIndex = i + 1;
                }
            }
            string[] messagesArray = new string[responses.Count];
            for (int i = 0; i < messagesArray.Length; i++)
            {
                messagesArray[i] = responses.ElementAt(i);
            }
            return messagesArray;
        }
        public static string[] GameHandler(Socket serverSock, string action, string[] args)
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
                    return null;
                }
                else if (action != "NOT MY TURN")
                {
                    //send the data
                    string message = CommandHandler(action, args);
                    message = message.Replace("Value", "value");
                    message = message.Replace("Type", "type");
                    message = message.Replace("Color", "color");
                    byte[] msg_bytes = Encoding.ASCII.GetBytes(message);
                    int bytesSent = serverSock.Send(msg_bytes);
                    Console.WriteLine(CommandHandler(action, args));
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
                return null;
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
                return null;
            }
        }

        public static void SendMsg(Socket serverSock, string action, object[] args)
        {
            try
            {
                if (action == "CLOSE")
                {
                    serverSock.Shutdown(SocketShutdown.Both);
                    serverSock.Close();
                }
                else
                {
                    //send the data
                    string message = CommandHandler(action, args);
                    message = message.Replace("Value", "value");
                    message = message.Replace("Type", "type");
                    message = message.Replace("Color", "color");
                    Regex regex = new Regex("\"[1-9]\"");
                    MatchCollection matches = regex.Matches(message);
                    foreach (Match match in matches)
                    {
                        message = message.Replace(match.Value, match.Value[1].ToString());
                    }
                    byte[] msg_bytes = Encoding.ASCII.GetBytes(message);
                    int bytesSent = serverSock.Send(msg_bytes);
                    Console.WriteLine("[Communication] Sent\n{0}", message);
                    Console.WriteLine("[Communication] Waiting For Response...");
                }
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
        }
        public static Socket StartClient(string serverIpStr, int serverPort)
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
                IPAddress serverIp = IPAddress.Parse(serverIpStr);
                IPEndPoint remoteEP = new IPEndPoint(serverIp, serverPort);

                //Create a TCP/IP socket
                Socket serverSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

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
    }
}
