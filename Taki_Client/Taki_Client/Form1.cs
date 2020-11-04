using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Taki_Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CreateGamePanel panel = new CreateGamePanel();
            //GameLobbyAdmin panel = new GameLobbyAdmin("1234", "abcde1234", "hstebryjhny", null);
            panel.Parent = this;
            this.Controls.Add(panel);
            panel.Initialize();
        }
    }
}
