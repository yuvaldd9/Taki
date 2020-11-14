using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Taki_Client
{
    class CreateGamePanel : Panel
    {
        private TextBox lobbyNameInput;
        private TextBox playerNameInput;
        private TextBox passwordInput;
        private HomePanel homePanel;

        public CreateGamePanel(HomePanel homePanel) : base()
        {
            this.Dock = DockStyle.Fill;
            this.homePanel = homePanel;
        }

        public void Initialize()
        {
            this.Location = new System.Drawing.Point(0, 0);
            this.Size = this.Parent.Size;
            this.BackColor = System.Drawing.Color.Gray;

            Label titleLabel = new Label();
            titleLabel.Text = "Please enter the following details";
            titleLabel.Font = new System.Drawing.Font("Arial", 16);
            titleLabel.AutoSize = true;
            this.Controls.Add(titleLabel);
            titleLabel.Location = new System.Drawing.Point((this.Width - titleLabel.Width) / 2, 10);

            Label lobbyName = new Label();
            this.Controls.Add(lobbyName);
            lobbyName.Text = "Lobby name: ";
            lobbyName.Font = new Font("Arial", 14);
            lobbyName.AutoSize = true;
            lobbyName.Location = new Point(10, this.Height / 5);
            this.lobbyNameInput = new TextBox();
            this.Controls.Add(lobbyNameInput);
            this.lobbyNameInput.Text = "";
            this.lobbyNameInput.Font = new Font("Arial", 14);
            this.lobbyNameInput.BackColor = Color.White;
            this.lobbyNameInput.Multiline = true;
            this.lobbyNameInput.Size = new Size(200, (new Font("Arial", 14).Height) + 8);

            Label playerName = new Label();
            this.Controls.Add(playerName);
            playerName.Text = "Player name: ";
            playerName.Font = new Font("Arial", 14);
            playerName.AutoSize = true;
            playerName.Location = new Point(10, 2 * this.Height / 5);
            this.playerNameInput = new TextBox();
            this.Controls.Add(playerNameInput);
            this.playerNameInput.Text = "";
            this.playerNameInput.Font = new Font("Arial", 14);
            this.playerNameInput.BackColor = Color.White;
            this.playerNameInput.Multiline = true;
            this.playerNameInput.Size = new Size(200, (new Font("Arial", 14).Height) + 8);

            Label password = new Label();
            this.Controls.Add(password);
            password.Text = "Password: ";
            password.Font = new Font("Arial", 14);
            password.AutoSize = true;
            password.Location = new Point(10, 3 * this.Height / 5);
            this.passwordInput = new TextBox();
            this.Controls.Add(passwordInput);
            this.passwordInput.Text = "";
            this.passwordInput.Font = new Font("Arial", 14);
            this.passwordInput.BackColor = Color.White;
            this.passwordInput.Multiline = true;
            this.passwordInput.Size = new Size(200, (new Font("Arial", 14).Height) + 8);

            int maxLength = Math.Max(lobbyName.Width, Math.Max(playerName.Width, password.Width));
            this.lobbyNameInput.Location = new Point(10 + maxLength + 10, this.Height / 5 - 5);
            this.playerNameInput.Location = new Point(10 + maxLength + 10, 2 * this.Height / 5 - 5);
            this.passwordInput.Location = new Point(10 + maxLength + 10, 3 * this.Height / 5 - 5);

            PictureBox logo = new PictureBox();
            logo.SizeMode = PictureBoxSizeMode.StretchImage;
            Bitmap image = new Bitmap(@"..\\..\\Resources\\Taki Logo.jpg");
            logo.ClientSize = new Size(image.Width, image.Height);
            logo.Image = (Image)image;
            logo.Location = new Point(3 * this.Width / 5, this.Height / 4);
            this.Controls.Add(logo);

            Button createGame = new Button();
            createGame.Text = "Create";
            createGame.Font = new Font("Arial", 14);
            createGame.BackColor = Color.Green;
            createGame.TextAlign = ContentAlignment.MiddleCenter;
            createGame.Click += new EventHandler(createGame_Click);
            createGame.AutoSize = true;
            createGame.Location = new Point(20, this.Height - 40);
            this.Controls.Add(createGame);
        }

        void createGame_Click(object Sender, EventArgs e)
        {
            if (this.lobbyNameInput.Text == "" || this.playerNameInput.Text == "" || this.passwordInput.Text == "")
            {
                MessageBox.Show("You must fill the details before creating the game", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Socket socket = Communication.StartClient("127.0.0.1", 8080);
            if (socket == null)
                MessageBox.Show("An error occured while connecting to the server, please try again");
            else
            {
                string[] responseArray = Communication.GameHandler(socket, "create_game", new string[] { this.lobbyNameInput.Text, this.playerNameInput.Text, this.passwordInput.Text });
                dynamic json = JsonConvert.DeserializeObject(responseArray[0]);
                string status = json.status.ToString();
                string jsonArgs = json.args.ToString();
                if (status != "success")
                {
                    dynamic msg = JsonConvert.DeserializeObject(jsonArgs);
                    MessageBox.Show(msg.message.ToString(), status, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                dynamic args = JsonConvert.DeserializeObject(jsonArgs);
                string gameID = (string)args.game_id;
                string jwt = (string)args.jwt;
                GameLobbyAdmin panel = new GameLobbyAdmin(gameID, this.playerNameInput.Text, this.passwordInput.Text, jwt, socket, this.homePanel);
                panel.Parent = this.Parent;
                this.Parent.Controls.Add(panel);
                panel.Initialize();
                this.Parent.Controls.Remove(this);

            }
        }
    }
}
