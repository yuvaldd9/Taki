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
        private string[] leaderboard; //The names of the players, ordered by the time they finished the game
        public LeaderboardPanel(string[] leaderboard)
        {
            this.leaderboard = leaderboard;
            this.BackColor = Color.Gray;
            this.Dock = DockStyle.Fill;
        }

        public void Initialize()
        {
            //Add a title
            Label title = new Label();
            title.Text = "Leaderboard";
            title.Font = new Font("Arial", 20);
            title.AutoSize = true;
            this.PlaceControl(title, new Point((this.Width - title.Width) / 2, 30));

            //Present the players' names with their rank and decorate them with a color according to their rank
            Label firstPlace = new Label();
            firstPlace.Text = "First: " + this.leaderboard[0];
            firstPlace.Font = new Font("Arial", 14);
            firstPlace.AutoSize = true;
            firstPlace.BackColor = Color.Gold;
            this.PlaceControl(firstPlace, new Point((this.Width - firstPlace.Width) / 2, 200));

            Label secondPlace = new Label();
            secondPlace.Text = "Second: " + this.leaderboard[1];
            secondPlace.Font = new Font("Arial", 14);
            secondPlace.AutoSize = true;
            secondPlace.BackColor = Color.Silver;
            this.PlaceControl(secondPlace, new Point((this.Width - firstPlace.Width) / 2, 300));

            Label thirdPlace = new Label();
            thirdPlace.Text = "Third: " + this.leaderboard[2];
            thirdPlace.Font = new Font("Arial", 14);
            thirdPlace.AutoSize = true;
            thirdPlace.BackColor = Color.Chocolate;
            this.PlaceControl(thirdPlace, new Point((this.Width - firstPlace.Width) / 2, 400));
            
            Label forthPlace = new Label();
            forthPlace.Text = "Forth: " + this.leaderboard[3];
            forthPlace.Font = new Font("Arial", 14);
            forthPlace.AutoSize = true;
            forthPlace.BackColor = Color.Transparent;
            this.PlaceControl(forthPlace, new Point((this.Width - firstPlace.Width) / 2, 500));

            //Add a button to return to the home window
            Button homeButton = new Button();
            homeButton.Text = "Main Menu";
            homeButton.Font = new Font("Arial", 12);
            homeButton.AutoSize = true;
            homeButton.BackColor = Color.Green;
            homeButton.Click += new EventHandler(this.homeButton_Click);
            this.PlaceControl(homeButton, new Point((this.Width - firstPlace.Width) / 2, this.Height - homeButton.Height - 40));
           
        }

        private void PlaceControl(Control control, Point location)
        {
            //Place a control on the panel
            this.Invoke(new MethodInvoker(delegate () { this.Controls.Add(control); }));
            this.Invoke(new MethodInvoker(delegate () { control.Location = location; }));
        }

        private void homeButton_Click(object sender, EventArgs e)
        {
            //Remove the cuurent panel and load the home panel
            HomePanel panel = new HomePanel();
            this.Parent.Invoke(new MethodInvoker(delegate () { this.Parent.Controls.Add(panel); }));
            this.Parent.Invoke(new MethodInvoker(delegate () { this.Parent.Controls.Remove(this); }));
            panel.Initialize();
        }
    }
}
