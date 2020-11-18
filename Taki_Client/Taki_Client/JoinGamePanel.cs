using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;

namespace Taki_Client
{
    class JoinGamePanel : Panel
    {
        private TextBox gameIDInput;
        private TextBox playerNameInput;
        private TextBox passwordInput;
        private HomePanel homepanel;

        public JoinGamePanel(HomePanel homepanel)
        {
            this.Dock = DockStyle.Fill;
            this.homepanel = homepanel;
        }

        public void Initialize()
        {
            this.Location = new System.Drawing.Point(0, 0);
            this.Size = this.Parent.Size;
            this.BackColor = System.Drawing.Color.Gray;

            Label titleLabel = new Label();
            titleLabel.Text = "Please enter the following details";
            titleLabel.Font = new Font("Arial", 16);
            titleLabel.AutoSize = true;
            this.Controls.Add(titleLabel);
            titleLabel.Location = new Point((this.Width - titleLabel.Width) / 2, 10);

            Label gameIDLabel = new Label();
            gameIDLabel.Text = "Game ID: ";
            gameIDLabel.Font = new Font("Arial", 14);
            gameIDLabel.AutoSize = true;
            this.Controls.Add(gameIDLabel);
            gameIDLabel.Location = new Point(10, this.Height / 5);
            this.gameIDInput = new TextBox();
            this.gameIDInput.Text = "";
            this.gameIDInput.Font = new Font("Arial", 14);
            this.gameIDInput.BackColor = Color.White;
            this.gameIDInput.Multiline = false;
            this.gameIDInput.Size = new Size(200, this.gameIDInput.Font.Height + 8);
            this.Controls.Add(this.gameIDInput);

            Label playerNameLabel = new Label();
            playerNameLabel.Text = "Player Name: ";
            playerNameLabel.Font = new Font("Arial", 14);
            playerNameLabel.AutoSize = true;
            this.Controls.Add(playerNameLabel);
            playerNameLabel.Location = new Point(10, 2 * this.Height / 5);
            this.playerNameInput = new TextBox();
            this.playerNameInput.Text = "";
            this.playerNameInput.Font = new Font("Arial", 14);
            this.playerNameInput.BackColor = Color.White;
            this.playerNameInput.Multiline = false;
            this.playerNameInput.Size = new Size(200, this.playerNameInput.Font.Height + 8);
            this.Controls.Add(this.playerNameInput);

            Label passwordLabel = new Label();
            passwordLabel.Text = "Password: ";
            passwordLabel.Font = new Font("Arial", 14);
            passwordLabel.AutoSize = true;
            this.Controls.Add(passwordLabel);
            passwordLabel.Location = new Point(10, 3 * this.Height / 5);
            this.passwordInput = new TextBox();
            this.passwordInput.Text = "";
            this.passwordInput.Font = new Font("Arial", 14);
            this.passwordInput.BackColor = Color.White;
            this.passwordInput.Multiline = false;
            this.passwordInput.Size = new Size(200, this.passwordInput.Font.Height + 8);
            this.Controls.Add(this.passwordInput);

            int maxLength = Math.Max(gameIDLabel.Width, Math.Max(playerNameLabel.Width, passwordLabel.Width));
            this.gameIDInput.Location = new Point(10 + maxLength + 10, this.Height / 5 - 5);
            this.playerNameInput.Location = new Point(10 + maxLength + 10, 2 * this.Height / 5);
            this.passwordInput.Location = new Point(10 + maxLength + 10, 3 * this.Height / 5);

            PictureBox logo = new PictureBox();
            logo.SizeMode = PictureBoxSizeMode.StretchImage;
            Bitmap image = new Bitmap(@"..\\..\\Resources\\Taki Logo.jpg");
            logo.ClientSize = new Size(image.Width, image.Height);
            logo.Image = (Image)image;
            logo.Location = new Point(3 * this.Width / 5, this.Height / 4);
            this.Controls.Add(logo);

            Button joinGame = new Button();
            joinGame.Text = "Join";
            joinGame.Font = new Font("Arial", 14);
            joinGame.BackColor = Color.Green;
            joinGame.TextAlign = ContentAlignment.MiddleCenter;
            joinGame.AutoSize = true;
            joinGame.Click += new EventHandler(this.joinGame_Click);
            joinGame.Location = new Point(20, this.Height - 40);
            this.Controls.Add(joinGame);
        }

        void joinGame_Click(object sender, EventArgs e)
        {
            if (this.gameIDInput.Text == "" || this.playerNameInput.Text == "" || this.passwordInput.Text == "")
            {
                MessageBox.Show("You must fill the details before joining the game", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Socket socket = Communication.StartClient("104.156.225.184", 8080);
            if (socket == null)
                MessageBox.Show("An error occured while connecting to the server, please try again");
            else
            {
                string[] responseArray = Communication.GameHandler(socket, "join_game", new string[] { this.gameIDInput.Text, this.playerNameInput.Text, this.passwordInput.Text });
                dynamic json = JsonConvert.DeserializeObject(responseArray[0]);
                string status = json.status;
                string jsonArgs = json.args.ToString();
                if (status != "success")
                {
                    dynamic msg = JsonConvert.DeserializeObject(jsonArgs);
                    MessageBox.Show(msg.message.ToString(), status, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                dynamic args = JsonConvert.DeserializeObject(jsonArgs);
                string jwt = args.jwt;
                JArray players = args.players;
                List<string> playersList = new List<string>();
                foreach (string player in players)
                    playersList.Add(player);
                GameLobbyPlayer panel = new GameLobbyPlayer(this.gameIDInput.Text, this.passwordInput.Text, jwt, socket, this.playerNameInput.Text, this.homepanel);
                panel.Parent = this.Parent;
                this.Parent.Controls.Add(panel);
                panel.Initialize(playersList);
                this.Parent.Controls.Remove(this);

            }
        }
    }
}
