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
            //Resize thw window to be a square
            this.Size = new Size(this.Width, this.Width);
            
            //Load the home window
            HomePanel panel = new HomePanel();
            panel.Parent = this;
            this.Controls.Add(panel);
            panel.Initialize();
        }
    }
}
