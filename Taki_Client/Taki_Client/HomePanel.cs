﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Taki_Client
{
    class HomePanel : Panel
    {
        public HomePanel() : base()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.Gray;
        }

        public void Initialize()
        {
            PictureBox logo = new PictureBox();
            logo.SizeMode = PictureBoxSizeMode.StretchImage;
            Bitmap logoImage = new Bitmap(@"..\\..\\Resources\\Taki Logo Horizontal.jpg");
            logo.ClientSize = new Size(logoImage.Width, logoImage.Height);
            logo.Image = (Image)logoImage;
            logo.Location = new Point((this.Width - logo.Width) / 2, 30);
            this.Controls.Add(logo);

            Button createGame = new Button();
            createGame.Text = "Create Game";
            createGame.Font = new Font("Arial", 16);
            createGame.AutoSize = true;
            createGame.BackColor = Color.Green;
            createGame.Click += new EventHandler(this.createButton_Click);
            createGame.Location = new Point(logo.Left, this.Height * 3 / 4);
            this.Controls.Add(createGame);

            Button joinGame = new Button();
            joinGame.Text = "Join Game";
            joinGame.Font = new Font("Arial", 16);
            joinGame.AutoSize = true;
            joinGame.Click += new EventHandler(this.joinButton_Click);
            joinGame.BackColor = Color.Yellow;
            this.Controls.Add(joinGame);
            joinGame.Location = new Point(logo.Right - joinGame.Width, this.Height * 3 / 4);
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            CreateGamePanel panel = new CreateGamePanel();
            this.Parent.Controls.Add(panel);
            this.Parent.Controls.Remove(this);
            panel.Initialize();
        }

        private void joinButton_Click(object sender, EventArgs e)
        {
            JoinGamePanel panel = new JoinGamePanel();
            this.Parent.Controls.Add(panel);
            this.Parent.Controls.Remove(this);
            panel.Initialize();
        }
    }
}
