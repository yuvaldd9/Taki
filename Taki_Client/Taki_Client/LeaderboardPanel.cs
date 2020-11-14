using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Taki_Client
{
    class LeaderboardPanel : Panel
    {
        private string[] leaderboard;
        public LeaderboardPanel(string[] leaderboard)
        {
            this.leaderboard = leaderboard;
            this.BackColor = Color.Gray;
            this.Dock = DockStyle.Fill;
        }

        public void Initialize()
        {
            Label title = new Label();
            title.Text = "Leaderboard";
            title.Font = new Font("Arial", 20);
            title.AutoSize = true;
            this.Invoke(new MethodInvoker(delegate () { this.Controls.Add(title); }));
            this.Invoke(new MethodInvoker(delegate () { title.Location = new Point((this.Width - title.Width) / 2, 30); }));

            Label firstPlace = new Label();
            firstPlace.Text = "First: " + this.leaderboard[0];
            firstPlace.Font = new Font("Arial", 14);
            firstPlace.AutoSize = true;
            firstPlace.BackColor = Color.Gold;
            this.Invoke(new MethodInvoker(delegate () { this.Controls.Add(firstPlace); }));
            this.Invoke(new MethodInvoker(delegate () { firstPlace.Location = new Point((this.Width - firstPlace.Width) / 2, 200); }));


            Label secondPlace = new Label();
            secondPlace.Text = "Second: " + this.leaderboard[1];
            secondPlace.Font = new Font("Arial", 14);
            secondPlace.AutoSize = true;
            secondPlace.BackColor = Color.Silver;
            this.Invoke(new MethodInvoker(delegate () { this.Controls.Add(secondPlace); }));
            this.Invoke(new MethodInvoker(delegate () { secondPlace.Location = new Point((this.Width - firstPlace.Width) / 2, 300); }));


            Label thirdPlace = new Label();
            thirdPlace.Text = "Third: " + this.leaderboard[2];
            thirdPlace.Font = new Font("Arial", 14);
            thirdPlace.AutoSize = true;
            thirdPlace.BackColor = Color.Chocolate;
            this.Invoke(new MethodInvoker(delegate () { this.Controls.Add(thirdPlace); }));
            this.Invoke(new MethodInvoker(delegate () { thirdPlace.Location = new Point((this.Width - firstPlace.Width) / 2, 400); }));


            Label forthPlace = new Label();
            forthPlace.Text = "Forth: " + this.leaderboard[3];
            forthPlace.Font = new Font("Arial", 14);
            forthPlace.AutoSize = true;
            forthPlace.BackColor = Color.Transparent;
            this.Invoke(new MethodInvoker(delegate () { this.Controls.Add(forthPlace); }));
            this.Invoke(new MethodInvoker(delegate () { forthPlace.Location = new Point((this.Width - firstPlace.Width) / 2, 500); }));


            Button homeButton = new Button();
            homeButton.Text = "Main Menu";
            homeButton.Font = new Font("Arial", 12);
            homeButton.AutoSize = true;
            homeButton.BackColor = Color.Green;
            homeButton.Click += new EventHandler(this.homeButton_Click);
            this.Invoke(new MethodInvoker(delegate () { this.Controls.Add(homeButton); }));
            this.Invoke(new MethodInvoker(delegate () { homeButton.Location = new Point((this.Width - firstPlace.Width) / 2, this.Height - homeButton.Height - 40); }));



        }

        private void homeButton_Click(object sender, EventArgs e)
        {
            HomePanel panel = new HomePanel();
            this.Parent.Invoke(new MethodInvoker(delegate () { this.Parent.Controls.Add(panel); }));
            this.Parent.Invoke(new MethodInvoker(delegate () { this.Parent.Controls.Remove(this); }));
            panel.Initialize();
        }
    }
}
