using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Sockets;
using System.Windows.Forms;
namespace Taki_Client
{
    class GameManager
    {
        private Bot bot;
        private GamePanel panel;
        private List<Enemy> enemies;
        private Deck deck;
        private string playerName;
        private Card topCard;
        private Socket sock;
        private string jwt;
        private string[] leaderboard;
        public GameManager(Deck deck, List<Enemy> enemies, string playerName, Card topCard, Socket sock, string jwt, Control parent, GamePanel panel)
        {
            this.jwt = jwt;
            this.deck = deck;
            this.enemies = enemies;
            this.playerName = playerName;
            this.topCard = topCard;
            this.bot = new Bot(topCard, deck, enemies);
            this.panel = panel;
            this.panel.Initialize(topCard);
            this.sock = sock;
            this.leaderboard = new string[enemies.Count + 1];
            for (int i = 0; i < this.leaderboard.Length; i++)
            {
                this.leaderboard[i] = "";
            }
        }

        public void Run(string startingPlayer="")
        {
            string[] jsonArr;
            dynamic args;
            string type, playerName;
            int amount;
            List<Card> action, taken = null;
            JArray cards;
            dynamic jCards;
            bool waitingForCards = false;
            if(startingPlayer == this.playerName)
            {
                action = bot.ChooseAction();
                if (action != null && action.Count > 0)
                {


                    Communication.SendMsg(this.sock, "place_cards", new object[] { action.ToArray(), this.jwt });
                    foreach (Card card in action)
                        this.bot.deck.RemoveCard(card);
                }
                else
                {
                    Communication.SendMsg(this.sock, "take_cards", new string[] { this.jwt });
                    waitingForCards = true;
                }
            }
            
            string[] data = Communication.GameHandler(sock, "NOT MY TURN", null);
            while (true)
            {
                foreach (string json in data)
                {
                    Console.WriteLine(json);
                    jsonArr = Communication.JsonAnalyzer(json);
                    args = JsonConvert.DeserializeObject(jsonArr[1]);
                    switch (jsonArr[0])
                    {
                        case "update_turn":

                            playerName = args.current_player;

                            if (playerName == this.playerName)
                            {
                                action = bot.ChooseAction();
                                if (action != null && action.Count > 0)
                                {
                                        

                                    Communication.SendMsg(this.sock, "place_cards", new object[] { action.ToArray(), this.jwt });
                                    foreach (Card card in action)
                                        this.bot.deck.RemoveCard(card);
                                }
                                else
                                {
                                    Communication.SendMsg(this.sock, "take_cards", new string[] { this.jwt });
                                    waitingForCards = true;
                                }

                            }

                            panel.UpdateCurrentPlayer((string)playerName);
                            break;
                        case "move_done":
                            type = (string)args.type;
                            playerName = (string)args.player_name;
                            if (type == "cards_taken")
                            {
                                if (playerName == this.playerName && waitingForCards)
                                {
                                }
                                else
                                {
                                    amount = int.Parse((string)args.amount);
                                    List<Card> cards_taken = new List<Card>();
                                    for (int i = 0; i < amount; i++)
                                        cards_taken.Add(null);
                                    panel.TakeCards(playerName, cards_taken);
                                    System.Threading.Thread.Sleep(7000);
                                    foreach (Enemy enemy in this.enemies)
                                    {
                                        if (enemy.name == playerName)
                                            enemy.UpdateState(-amount);
                                    }
                                }
                                this.bot.isTopCardActive = false;
                            }
                            else
                            {

                                cards = args.cards;

                                action = new List<Card>();
                                foreach (object card in cards)
                                {
                                    jCards = JsonConvert.DeserializeObject(card.ToString());
                                    Card c = new Card((string)jCards.type, (string)jCards.color, (string)jCards.value);
                                    action.Add(c);
                                }
                                panel.PlayCards(playerName, action);
                                System.Threading.Thread.Sleep(7000);
                                bot.UpdateTopCard(new Card(action.ElementAt(action.Count - 1)));
                                this.bot.isTopCardActive = true;
                                foreach (Enemy enemy in this.enemies)
                                {
                                    if (enemy.name == playerName)
                                        enemy.UpdateState(action.Count);
                                }
                            }
                            break;
                        case "success":
                            if (waitingForCards && args.cards != null)
                            {
                                cards = args.cards;

                                taken = new List<Card>();
                                foreach (object card in cards)
                                {
                                    jCards = JsonConvert.DeserializeObject(card.ToString());
                                    Card c = new Card((string)jCards.type, (string)jCards.color, (string)jCards.value);
                                    taken.Add(c);
                                    this.deck.AddCard(c);
                                }
                                panel.TakeCards(this.playerName, taken);
                                taken = null;
                                System.Threading.Thread.Sleep(7000);
                                waitingForCards = false;
                            }
                            break;
                        case "player_finished":
                            for (int i = 0; i < this.leaderboard.Length; i++)
                            {
                                if (this.leaderboard[i] == "")
                                {
                                    this.leaderboard[i] = args.player_name.ToString();
                                    break;
                                }
                            }

                            break;

                        case "player_left":
                            for (int i = this.leaderboard.Length - 1; i >= 0; i--)
                            {
                                if (this.leaderboard[i] == "")
                                {
                                    this.leaderboard[i] = args.player_name.ToString();
                                    break;
                                }

                            }

                            break;
                        case "game_ended":
                            LeaderboardPanel leaderboardPanel = new LeaderboardPanel(this.leaderboard);
                            this.panel.Parent.Invoke(new MethodInvoker(delegate () { this.panel.Parent.Controls.Add(leaderboardPanel); }));
                            leaderboardPanel.Initialize();
                            this.panel.Parent.Invoke(new MethodInvoker(delegate () { this.panel.Parent.Controls.Remove(this.panel); }));
                            break;

                        case "bad_request":
                            if (args.message != null && args.message.ToString() == "Invalid move done.")
                            {
                                Communication.SendMsg(this.sock, "take_cards", new string[] { this.jwt });
                                waitingForCards = true;
                            }
                            break;
                        default:
                            Console.WriteLine(jsonArr[0]);
                            Console.WriteLine(jsonArr[1]);
                            break;
                    }

                }
                data = Communication.GameHandler(sock, "NOT MY TURN", null);

            }
         
        }
    }
}