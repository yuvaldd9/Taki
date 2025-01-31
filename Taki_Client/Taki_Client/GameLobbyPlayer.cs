﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace Taki_Client
{
    class GameLobbyPlayer : Panel
    {
        private string gameID;
        private string password;
        private string jwt;
        private Socket sock;
        private ListBox players;
        private bool waiting;
        private bool inGame;
        private string name;
        private HomePanel homePanel;

        public GameLobbyPlayer(string gameID, string password, string jwt, Socket sock, string name, HomePanel homePanel) : base()
        {
            this.gameID = gameID;
            this.password = password;
            this.jwt = jwt;
            this.sock = sock;
            this.Dock = DockStyle.Fill;
            this.waiting = true;
            this.inGame = false;
            this.name = name;
            this.homePanel = homePanel;
        }

        public void Initialize(List<string> playersList)
        {
            this.Location = new System.Drawing.Point(0, 0);
            this.Size = this.Parent.Size;
            this.BackColor = System.Drawing.Color.Gray;

            Label titleLabel = new Label();
            titleLabel.Text = "Waiting for players to join...";
            titleLabel.Font = new Font("Arial", 18);
            titleLabel.AutoSize = true;
            titleLabel.Location = new Point(10, 10);
            this.Controls.Add(titleLabel);

            Label gameIDLabel = new Label();
            gameIDLabel.Text = "Game ID: " + this.gameID;
            gameIDLabel.Font = new Font("Arial", 14);
            gameIDLabel.AutoSize = true;
            gameIDLabel.Location = new Point(10, this.Height / 6);
            this.Controls.Add(gameIDLabel);

            Label passwordLabel = new Label();
            passwordLabel.Text = "Password: " + this.password;
            passwordLabel.Font = new Font("Arial", 14);
            passwordLabel.AutoSize = true;
            passwordLabel.Location = new Point(10, gameIDLabel.Bottom + 10);
            this.Controls.Add(passwordLabel);

            Label listTitle = new Label();
            listTitle.Text = "Players joined:";
            listTitle.Font = new Font("Arial", 14);
            listTitle.AutoSize = true;
            listTitle.Location = new Point(10, 3 * this.Height / 6);
            this.Controls.Add(listTitle);

            this.players = new ListBox();
            this.players.Font = new Font("Arial", 12);
            this.players.Location = new Point(10, listTitle.Bottom + 5);
            this.players.Size = new Size(this.Width / 2, 2 * this.Height / 6 - 10);
            this.Controls.Add(this.players);
            foreach (string player in playersList)
                this.players.Items.Add(player);

            PictureBox logo = new PictureBox();
            logo.SizeMode = PictureBoxSizeMode.StretchImage;
            Bitmap image = new Bitmap(@"..\\..\\Resources\\Taki Logo.jpg");
            logo.ClientSize = new Size(image.Width, image.Height);
            logo.Image = (Image)image;
            logo.Location = new Point(3 * this.Width / 5, this.Height / 4);
            this.Controls.Add(logo);

            Button closeGame = new Button();
            closeGame.Text = "Leave";
            closeGame.Font = new Font("Arial", 14);
            closeGame.BackColor = Color.DarkRed;
            closeGame.AutoSize = true;
            closeGame.Location = new Point(10, this.Height - 10 - closeGame.Height);
            closeGame.Click += new EventHandler(closeGame_Click);
            this.Controls.Add(closeGame);

            Thread wait = new Thread(new ThreadStart(this.WaitForPlayers));
            wait.Start();
        }

        private void WaitForPlayers()
        {
            string[] messages;
            Deck deck = new Deck(new List<Card>());
            string player_name, code, currentPlayer = "";
            List<Enemy> enemies = new List<Enemy>();
            JArray players, cards;
            dynamic json;
            while (this.waiting)
            {
                messages = Communication.GameHandler(this.sock, "NOT MY TURN", new string[] { });
                foreach (string msg in messages)
                {
                    json = JsonConvert.DeserializeObject(msg);
                    if (json.code == null)
                        code = json.status.ToString();
                    else
                        code = json.code.ToString();
                    if (code == "player_joined")
                    {
                        dynamic args = JsonConvert.DeserializeObject(json.args.ToString());
                        player_name = args.player_name.ToString();
                        this.Invoke(new MethodInvoker(delegate () { this.players.Items.Add(player_name); }));
                        continue;
                    }

                    else if (code == "player_left")
                    {
                        dynamic args = JsonConvert.DeserializeObject(json.args.ToString());
                        player_name = args.player_name.ToString();
                        this.Invoke(new MethodInvoker(delegate () { this.players.Items.Remove(player_name); }));
                        continue;
                    }

                    else if (code == "game_starting")
                    {
                        dynamic args = JsonConvert.DeserializeObject(json.args.ToString());
                        players = args.players;
                        cards = args.cards;
                        int index = 1;
                        foreach (string player in players)
                        {
                            if (player != "" && player != this.name)
                            {
                                enemies.Add(new Enemy(player, index));
                                index++;
                            }
                        }

                        foreach (object card in cards)
                        {
                            dynamic jsonCard = JsonConvert.DeserializeObject(card.ToString());
                            deck.AddCard(new Card((string)jsonCard.type, (string)jsonCard.color, (string)jsonCard.value));
                        }
                        this.waiting = false;
                        this.inGame = true;
                        continue;

                    }
                    else if (code == "update_turn")
                    {
                        dynamic args = JsonConvert.DeserializeObject(json.args.ToString());
                        currentPlayer = (string)args.current_player;
                        continue;
                    }
                    else if (json.status != "success" && json.code != "success")
                    {
                        dynamic args = JsonConvert.DeserializeObject(json.args.ToString());
                        if (args.message != null)
                            MessageBox.Show(args.message.ToString(), (string)json.status, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }
                }
            }
            if (this.inGame)
            {
                GamePanel panel = new GamePanel(deck, enemies, this.name);
                panel.Size = this.Size;
                GameManager gameManager = new GameManager(deck, enemies, this.name, null, this.sock, this.jwt, this.Parent, panel);
                this.Invoke(new MethodInvoker(delegate () { this.Parent.Controls.Add(panel); }));
                this.Invoke(new MethodInvoker(delegate () { this.Parent.Controls.Remove(this); }));
                gameManager.Run(currentPlayer);
            }
            else
            {
                this.Parent.Invoke(new MethodInvoker(delegate () { this.Parent.Controls.Add(this.homePanel); }));
                this.Parent.Invoke(new MethodInvoker(delegate () { this.Parent.Controls.Remove(this); }));
            }

        }

        void closeGame_Click(object Sender, EventArgs e)
        {
            Communication.SendMsg(this.sock, "leave_game", new string[] { this.jwt });
            this.inGame = false;
            this.waiting = false;

        }
    }
}
